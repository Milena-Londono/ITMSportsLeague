using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var response = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);

            if (sponsor == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<SponsorResponseDTO>(sponsor);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SponsorRequestDTO request)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(request);
                var createdSponsor = await _sponsorService.CreateAsync(sponsor);
                var response = _mapper.Map<SponsorResponseDTO>(createdSponsor);

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SponsorRequestDTO request)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(request);
                await _sponsorService.UpdateAsync(id, sponsor);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
