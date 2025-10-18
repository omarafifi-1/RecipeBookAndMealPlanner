using Microsoft.EntityFrameworkCore;
using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Models;

public class IngredientRepository : IIngredientRepository
{
    private readonly AppDbContext _context;

    public IngredientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Ingredient> FindOrCreateAsync(string ingredientName)
    {
        string normalizedName = ingredientName.Trim().ToLower();

        var existingIngredient = await _context.Ingredients
            .FirstOrDefaultAsync(i => i.Name == normalizedName);

        if (existingIngredient != null)
        {
            return existingIngredient;
        }

        var newIngredient = new Ingredient { Name = normalizedName };
        await _context.Ingredients.AddAsync(newIngredient);
        await _context.SaveChangesAsync(); 
        return newIngredient;
    }
}