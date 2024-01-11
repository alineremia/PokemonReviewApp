using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository {
    public class CategoryRepository : ICategoryRepository {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context) {
            _context = context;
        }
        public ICollection<Category> GetCategories() {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id) {
            return _context.Categories.FirstOrDefault(c => c.Id == id);

        }

        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
            return _context.PokemonCategories.Where(pc => pc.CategoryId == categoryId).Select(p => p.Pokemon).ToList();
        }

        public bool CategoryExists(int id) {
           return _context.Categories.Any(c => c.Id == id);
        }

        public bool CreateCategory(Category category)
        {
           //Change Tracker
           // add, update, modifying
           // connected or disconnected (EntityState.Added)
           _context.Add(category);
           return Save();
        }

        public bool UpdateCategory(Category category) {
           _context.Update(category);
           return Save();
        }

        public bool DeleteCategory(Category category) {
            _context.Remove(category);
            return Save();
        }

        public bool Save() {
            var saved = _context.SaveChanges();
            return saved > 0;

        }
    }
}
