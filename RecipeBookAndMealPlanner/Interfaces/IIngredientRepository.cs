using RecipeBookAndMealPlanner.Models;

public interface IIngredientRepository
{
    Task<Ingredient> FindOrCreateAsync(string ingredientName);
}