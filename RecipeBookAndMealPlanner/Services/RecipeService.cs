using RecipeBookAndMealPlanner.Models;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IEnumerable<Recipe>> GetRecipesForUserAsync(string userId)
    {
        return await _recipeRepository.GetAllByUserIdAsync(userId);
    }

    public async Task<Recipe?> GetRecipeDetailsAsync(int id, string userId)
    {
        return await _recipeRepository.GetByIdWithIngredientsAsync(id, userId);
    }

    public async Task CreateRecipeAsync(RecipesVM viewModel, string userId)
    {
        var recipe = new Recipe
        {
            Title = viewModel.Title,
            Instructions = viewModel.Instructions,
            UserId = userId,
            RecipeIngredients = new List<RecipeIngredient>()
        };

        foreach (var ingredientString in viewModel.Ingredients)
        {
            if (string.IsNullOrWhiteSpace(ingredientString)) continue;
            var parts = ingredientString.Split(' ');
            if (parts.Length < 3)
            {
                continue;
            }

            string quantity = parts[0];
            string unit = parts[1];
            string name = string.Join(" ", parts.Skip(2));

            var ingredient = await _ingredientRepository.FindOrCreateAsync(name);

            var recipeIngredient = new RecipeIngredient
            {
                Recipe = recipe,
                Ingredient = ingredient,
                Quantity = quantity,
                Unit = unit
            };
            recipe.RecipeIngredients.Add(recipeIngredient);
        }

        await _recipeRepository.AddAsync(recipe);
    }

    public async Task DeleteRecipeAsync(int id, string userId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id, userId);
        if (recipe == null)
        {
            throw new Exception("Recipe not found or you do not have permission.");
        }
        await _recipeRepository.DeleteAsync(recipe);
    }

    public async Task UpdateRecipeAsync(RecipeEditViewModel viewModel, string userId)
    {
        var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(viewModel.Id, userId);

        if (recipe == null)
        {
            throw new Exception("Recipe not found or user not authorized.");
        }
        recipe.Title = viewModel.Title;
        recipe.Instructions = viewModel.Instructions;

        await _recipeRepository.RemoveRecipeIngredientsAsync(recipe.RecipeIngredients);

        recipe.RecipeIngredients = new List<RecipeIngredient>();
        foreach (var ingredientString in viewModel.Ingredients)
        {
            if (string.IsNullOrWhiteSpace(ingredientString)) continue;

            var parts = ingredientString.Split(' ');
            if (parts.Length < 3) continue;

            string quantity = parts[0];
            string unit = parts[1];
            string name = string.Join(" ", parts.Skip(2));

            var ingredient = await _ingredientRepository.FindOrCreateAsync(name);

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                Ingredient = ingredient,
                Quantity = quantity,
                Unit = unit
            });
        }
        await _recipeRepository.UpdateAsync(recipe);
    }
}