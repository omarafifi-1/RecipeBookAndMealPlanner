using Microsoft.EntityFrameworkCore;
using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Models;

namespace RecipeBookAndMealPlanner.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Title)
                .ToListAsync();
        }

        public async Task<Recipe?> GetByIdWithIngredientsAsync(int id, string userId)
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        }

        public async Task<Recipe?> GetByIdAsync(int id, string userId)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRecipeIngredientsAsync(IEnumerable<RecipeIngredient> ingredients)
        {
            _context.RecipeIngredients.RemoveRange(ingredients);
        }
    }
}