using RecipeBookAndMealPlanner.Models;

namespace RecipeBookAndMealPlanner.Interfaces
{
    public interface IPlannerRepository
    {
        Task<List<MealPlan>> GetMealPlansForWeekAsync(string userId, DateTime startDate, DateTime endDate);
        Task<MealPlan?> GetMealPlanAsync(int id, string userId);
        Task<MealPlan?> GetMealPlanByDetailsAsync(string userId, DateTime date, MealType mealType);
        Task AddMealPlanAsync(MealPlan mealPlan);
        Task UpdateMealPlanAsync(MealPlan mealPlan);
        Task DeleteMealPlanAsync(MealPlan mealPlan);
    }
}
