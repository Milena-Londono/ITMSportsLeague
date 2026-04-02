using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.Net.Mail;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(ISponsorRepository sponsorRepository, ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);

            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            await ValidateSponsorAsync(sponsor);

            sponsor.CreatedAt = DateTime.UtcNow;

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");
            }

            await ValidateSponsorAsync(sponsor, id);

            existingSponsor.Name = sponsor.Name;
            existingSponsor.ContactEmail = sponsor.ContactEmail;
            existingSponsor.Phone = sponsor.Phone;
            existingSponsor.WebsiteUrl = sponsor.WebsiteUrl;
            existingSponsor.Category = sponsor.Category;
            existingSponsor.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existingSponsor);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);

            if (!exists)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");
            }

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        private async Task ValidateSponsorAsync(Sponsor sponsor, int? sponsorId = null)
        {
            if (string.IsNullOrWhiteSpace(sponsor.Name))
                throw new InvalidOperationException("El nombre del sponsor es obligatorio.");

            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
                throw new InvalidOperationException("El email de contacto es obligatorio.");

            if (!IsValidEmail(sponsor.ContactEmail))
                throw new InvalidOperationException("El formato del email no es válido.");

            bool exists = sponsorId.HasValue
                ? await _sponsorRepository.ExistsByNameAsync(sponsor.Name, sponsorId.Value)
                : await _sponsorRepository.ExistsByNameAsync(sponsor.Name);

            if (exists)
                throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
