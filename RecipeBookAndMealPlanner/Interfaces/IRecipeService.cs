using RecipeBookAndMealPlanner.Models;

public interface IRecipeService
{
    Task<IEnumerable<Recipe>> GetRecipesForUserAsync(string userId);

    Task<Recipe?> GetRecipeDetailsAsync(int id, string userId);

    Task CreateRecipeAsync(RecipesVM viewModel, string userId);

    Task DeleteRecipeAsync(int id, string userId);

    Task UpdateRecipeAsync(RecipeEditViewModel viewModel, string userId);
}