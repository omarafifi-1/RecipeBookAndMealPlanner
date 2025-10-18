using RecipeBookAndMealPlanner.Models;

public interface IRecipeRepository
{
    Task<IEnumerable<Recipe>> GetAllByUserIdAsync(string userId);

    Task<Recipe?> GetByIdWithIngredientsAsync(int id, string userId);

    Task<Recipe?> GetByIdAsync(int id, string userId);

    Task AddAsync(Recipe recipe);

    Task RemoveRecipeIngredientsAsync(IEnumerable<RecipeIngredient> ingredients);

    Task UpdateAsync(Recipe recipe);

    Task DeleteAsync(Recipe recipe);
}