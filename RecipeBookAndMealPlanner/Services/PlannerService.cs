using RecipeBookAndMealPlanner.Interfaces;
using RecipeBookAndMealPlanner.Models;

namespace RecipeBookAndMealPlanner.Services
{
    public class PlannerService : IPlannerService
    {
        private readonly IPlannerRepository _plannerRepository;
        private readonly IRecipeRepository _recipeRepository;

        public PlannerService(IPlannerRepository plannerRepository, IRecipeRepository recipeRepository)
        {
            _plannerRepository = plannerRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<List<MealPlan>> GetPlanForWeekAsync(string userId, DateTime startOfWeek)
        {
            var endOfWeek = startOfWeek.AddDays(6);
            return await _plannerRepository.GetMealPlansForWeekAsync(userId, startOfWeek, endOfWeek);
        }

        public async Task AddRecipeToPlanAsync(int recipeId, string userId, DateTime date, MealType mealType)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId, userId);
            if (recipe == null)
            {
                throw new UnauthorizedAccessException("Recipe not found or you do not have permission to use it.");
            }

            var existingPlan = await _plannerRepository.GetMealPlanByDetailsAsync(userId, date, mealType);
            
            if (existingPlan != null)
            {
                existingPlan.RecipeId = recipeId;
                await _plannerRepository.UpdateMealPlanAsync(existingPlan);
            }
            else
            {
                var mealPlan = new MealPlan
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Date = date.Date,
                    MealType = mealType
                };
                await _plannerRepository.AddMealPlanAsync(mealPlan);
            }
        }

        public async Task RemoveFromPlanAsync(int mealPlanId, string userId)
        {
            var mealPlan = await _plannerRepository.GetMealPlanAsync(mealPlanId, userId);
            
            if (mealPlan == null)
            {
                throw new UnauthorizedAccessException("Meal plan not found or you do not have permission.");
            }

            await _plannerRepository.DeleteMealPlanAsync(mealPlan);
        }

        public async Task<Dictionary<string, List<string>>> GenerateShoppingListAsync(string userId, DateTime startOfWeek)
        {
            var endOfWeek = startOfWeek.AddDays(6);
            var mealPlans = await _plannerRepository.GetMealPlansForWeekAsync(userId, startOfWeek, endOfWeek);

            var ingredientGroups = mealPlans
                .Where(mp => mp.Recipe?.RecipeIngredients != null)
                .SelectMany(mp => mp.Recipe.RecipeIngredients)
                .GroupBy(ri => ri.Ingredient.Name.ToLower())
                .OrderBy(g => g.Key);

            var shoppingList = new Dictionary<string, List<string>>();

            foreach (var group in ingredientGroups)
            {
                var ingredientName = group.First().Ingredient.Name;
                var quantities = new List<string>();

                foreach (var ri in group)
                {
                    quantities.Add($"{ri.Quantity} {ri.Unit}");
                }

                shoppingList[ingredientName] = quantities;
            }

            return shoppingList;
        }
    }
}
