namespace RecipeBookAndMealPlanner.Interfaces
{
    public interface IPlannerService
    {
        Task<List<MealPlan>> GetPlanForWeekAsync(string userId, DateTime startOfWeek);
        Task AddRecipeToPlanAsync(int recipeId, string userId, DateTime date, MealType mealType);
        Task RemoveFromPlanAsync(int mealPlanId, string userId);
        Task<Dictionary<string, List<string>>> GenerateShoppingListAsync(string userId, DateTime startOfWeek);
    }
}
