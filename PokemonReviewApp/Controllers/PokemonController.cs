using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IMapper mapper) {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons() {
            var pokemons = _mapper.Map<List<PokemonDTO>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(pokemons);
        }


        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId) {

            if (!_pokemonRepository.PokemonExists(pokeId)) {
                return NotFound();
            }

            var pokemon = _mapper.Map<PokemonDTO>(_pokemonRepository.GetPokemon(pokeId));
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId) {
            if (!_pokemonRepository.PokemonExists(pokeId)) {
                return NotFound();
            }
            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDTO pokemonCreate) {
            if (pokemonCreate == null)
            {
                return BadRequest(ModelState);
            }

            var pokemons = _pokemonRepository
                .GetPokemons()
                .FirstOrDefault(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper());
            if (pokemons != null) {
                ModelState.AddModelError("", "Pokemon already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap)) {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon successfully created!");
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokemonId, [FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDTO updatedPokemon)
        {
            if (updatedPokemon == null) {
                return BadRequest(ModelState);
            }

            if (pokemonId != updatedPokemon.Id) {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.PokemonExists(pokemonId)) {
                return NotFound();
            }

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId) {
            if (!_pokemonRepository.PokemonExists(pokemonId)) {
                return NotFound();
            }

            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId); 
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList())) {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }


            if (!_pokemonRepository.DeletePokemon(pokemonToDelete)) {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
