using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;

        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper) {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews() {
            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }


        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReview(int reviewId) {

            if (!_reviewRepository.ReviewExists(reviewId)) {
                return NotFound();
            }

            var review = _mapper.Map<ReviewDTO>(_reviewRepository.GetReview(reviewId));
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            return Ok(review);
        }

        [HttpGet("pokemon/{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsForAPokemon(int pokemonId) {
            var reviews = _mapper.Map<List<ReviewDTO>>(_reviewRepository.GetReviewsOfAPokemon(pokemonId));

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            return Ok(reviews);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDTO reviewCreate) {
            if (reviewCreate == null) {
                return BadRequest(ModelState);
            }

            var review = _reviewRepository
                .GetReviews()
                .FirstOrDefault(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper());
            if (review != null) {
                ModelState.AddModelError("", "Review already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var reviewMap = _mapper.Map<Review>(reviewCreate);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokemonId);

            if (!_reviewRepository.CreateReview(reviewMap)) {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Review successfully created!");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReviewer(int reviewId, [FromBody] ReviewDTO updateReview) {
            if (reviewId == null) {
                return BadRequest(ModelState);
            }

            if (reviewId != updateReview.Id) {
                return BadRequest(ModelState);
            }
            
            if (!_reviewRepository.ReviewExists(reviewId)) {
                return NotFound();
            }

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var reviewMap = _mapper.Map<Review>(updateReview);

            if (!_reviewRepository.UpdateReview(reviewMap)) {
                ModelState.AddModelError("", "Something went wrong while updating!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId) {
            if (!_reviewRepository.ReviewExists(reviewId)) {
                return NotFound();
            }

            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            if (!_reviewRepository.DeleteReview(reviewToDelete)) {
                ModelState.AddModelError("", "Something went wrong while deleting!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
