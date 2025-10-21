using RecipeBookAndMealPlanner.Models;

public class MealPlannerVM
{
    public List<Recipe> UserRecipes { get; set; }
    public List<MealPlan> WeeklyPlan { get; set; }
    public DateTime StartOfWeek { get; set; }
}   