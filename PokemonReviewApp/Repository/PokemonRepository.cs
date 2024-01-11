using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository  : IPokemonRepository{
        private readonly DataContext _context;
        public PokemonRepository(DataContext context) {
            _context = context;
        }

       
        public ICollection<Pokemon> GetPokemons() {
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemon(int id) {
            return _context.Pokemon.FirstOrDefault(p => p.Id == id);
        }

        public Pokemon GetPokemon(string name) {
            return _context.Pokemon.FirstOrDefault(p => p.Name == name);
        }

        public decimal GetPokemonRating(int pokeId) {
            var review = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);
            if (review.Count() <= 0) {
                return 0;
            }

            return ((decimal) review.Sum(r => r.Rating) / review.Count()); 
        }

        public bool PokemonExists(int pokeId) {
           return  _context.Pokemon.Any(p => p.Id == pokeId);
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.FirstOrDefault(o => o.Id == ownerId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            var pokemonOwner = new PokemonOwner {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon
            };
            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory {
                Category = category,
                Pokemon = pokemon
            };
            _context.Add(pokemonCategory);
            _context.Add(pokemon);
            return Save();
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon) {
           // var pokemonOwner = _context.PokemonOwners.FirstOrDefault(o => o.OwnerId == ownerId);
           // _context.Update(pokemonOwner);
           // var category = _context.PokemonCategories.FirstOrDefault(c => c.CategoryId == categoryId);
           // _context.Update(category);

            _context.Update(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon) {
            _context.Remove(pokemon);
            return Save();
        }

        public bool Save() {
           var saved = _context.SaveChanges();
           return saved > 0;
        }
    }
}
