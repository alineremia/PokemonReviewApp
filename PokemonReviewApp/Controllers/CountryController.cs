using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            var countries = _mapper.Map<List<CountryDTO>>(_countryRepository.GetCountries());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {

            if (!_countryRepository.CountryExits(countryId))
            {
                return NotFound();
            }

            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        [HttpGet("/owners/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            var country = _mapper.Map<CountryDTO>(_countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(country);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CountryDTO? countryCreate) {
            if (countryCreate == null) {
                return BadRequest(ModelState);
            }

            var country = _countryRepository
                .GetCountries()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper());
            if (country != null) {
                ModelState.AddModelError("", "Country already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var countryMap = _mapper.Map<Country>(countryCreate);
            if (!_countryRepository.CreateCountry(countryMap)) {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Country successfully created!");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCountry(int countryId, [FromBody] CountryDTO updatedCountry)
        {
            if (updatedCountry == null) {
                return BadRequest(ModelState);
            }

            if (countryId != updatedCountry.Id) {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExits(countryId)) {
                return NotFound();
            }

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var countryMap = _mapper.Map<Country>(updatedCountry);
            if (!_countryRepository.UpdateCountry(countryMap)) {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId) {
            if (!_countryRepository.CountryExits(countryId)) {
                return NotFound();
            }

            var countryToDelete = _countryRepository.GetCountry(countryId);
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (!_countryRepository.DeleteCountry(countryToDelete)) {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
