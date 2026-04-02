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
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ITournamentRepository tournamentRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
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
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            }

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            await ValidateSponsorAsync(sponsor);

            sponsor.CreatedAt = DateTime.UtcNow;
            sponsor.UpdatedAt = null;

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);

            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existingSponsor = await _sponsorRepository.GetByIdAsync(id);

            if (existingSponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
                throw new KeyNotFoundException($"Sponsor with ID {id} was not found.");
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
            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
                throw new KeyNotFoundException($"Sponsor with ID {id} was not found.");
            }

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);

            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetSponsorTournamentsAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found when listing tournaments", sponsorId);
                throw new KeyNotFoundException($"Sponsor with ID {sponsorId} was not found.");
            }

            _logger.LogInformation("Retrieving tournaments for sponsor ID: {SponsorId}", sponsorId);

            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }

        public async Task<TournamentSponsor> LinkTournamentAsync(int sponsorId, TournamentSponsor tournamentSponsor)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found for linking", sponsorId);
                throw new KeyNotFoundException($"Sponsor with ID {sponsorId} was not found.");
            }

            var tournament = await _tournamentRepository.GetByIdAsync(tournamentSponsor.TournamentId);
            if (tournament == null)
            {
                _logger.LogWarning("Tournament with ID {TournamentId} not found for linking", tournamentSponsor.TournamentId);
                throw new KeyNotFoundException($"Tournament with ID {tournamentSponsor.TournamentId} was not found.");
            }

            if (tournamentSponsor.ContractAmount <= 0)
            {
                throw new InvalidOperationException("Contract amount must be greater than 0.");
            }

            var existingLink = await _tournamentSponsorRepository
                .GetBySponsorAndTournamentAsync(sponsorId, tournamentSponsor.TournamentId);

            if (existingLink != null)
            {
                throw new InvalidOperationException("This sponsor is already linked to the selected tournament.");
            }

            tournamentSponsor.SponsorId = sponsorId;
            tournamentSponsor.JoinedAt = DateTime.UtcNow;
            tournamentSponsor.CreatedAt = DateTime.UtcNow;
            tournamentSponsor.UpdatedAt = null;

            _logger.LogInformation(
                "Linking sponsor ID {SponsorId} to tournament ID {TournamentId}",
                sponsorId,
                tournamentSponsor.TournamentId);

            return await _tournamentSponsorRepository.CreateAsync(tournamentSponsor);
        }

        public async Task UnlinkTournamentAsync(int sponsorId, int tournamentId)
        {
            var existingLink = await _tournamentSponsorRepository
                .GetBySponsorAndTournamentAsync(sponsorId, tournamentId);

            if (existingLink == null)
            {
                _logger.LogWarning(
                    "Link between sponsor ID {SponsorId} and tournament ID {TournamentId} was not found",
                    sponsorId,
                    tournamentId);

                throw new KeyNotFoundException(
                    $"The link between sponsor {sponsorId} and tournament {tournamentId} was not found.");
            }

            _logger.LogInformation(
                "Removing link between sponsor ID {SponsorId} and tournament ID {TournamentId}",
                sponsorId,
                tournamentId);

            await _tournamentSponsorRepository.DeleteAsync(existingLink.Id);
        }

        private async Task ValidateSponsorAsync(Sponsor sponsor, int? sponsorId = null)
        {
            if (string.IsNullOrWhiteSpace(sponsor.Name))
            {
                throw new InvalidOperationException("Sponsor name is required.");
            }

            if (string.IsNullOrWhiteSpace(sponsor.ContactEmail))
            {
                throw new InvalidOperationException("Contact email is required.");
            }

            if (!IsValidEmail(sponsor.ContactEmail))
            {
                throw new InvalidOperationException("Contact email format is invalid.");
            }

            bool nameExists = sponsorId.HasValue
                ? await _sponsorRepository.ExistsByNameAsync(sponsor.Name, sponsorId.Value)
                : await _sponsorRepository.ExistsByNameAsync(sponsor.Name);

            if (nameExists)
            {
                throw new InvalidOperationException("A sponsor with the same name already exists.");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
