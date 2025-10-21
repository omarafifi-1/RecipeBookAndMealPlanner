using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Interfaces;

namespace RecipeBookAndMealPlanner.Controllers
{
    [Authorize]
    public class PlannerController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IPlannerService _mealPlanService;
        private readonly UserManager<AppUser> _userManager;

        public PlannerController(IRecipeService recipeService, IPlannerService plannerService, UserManager<AppUser> userManager)
        {
            _recipeService = recipeService;
            _mealPlanService = plannerService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Today;
            
            // Get the start of the week (Monday)
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            
            // If today is Sunday, go to next Monday
            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = startOfWeek.AddDays(7);
            }

            var viewModel = new MealPlannerVM
            {
                UserRecipes = (List<Models.Recipe>)await _recipeService.GetRecipesForUserAsync(userId),
                WeeklyPlan = await _mealPlanService.GetPlanForWeekAsync(userId, startOfWeek),
                StartOfWeek = startOfWeek
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> AddToPlan(int recipeId, DateTime date, MealType mealType)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                await _mealPlanService.AddRecipeToPlanAsync(recipeId, userId, date, mealType);
                TempData["SuccessMessage"] = "Recipe added to your meal plan!";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromPlan(int mealPlanId)
        {
            var userId = _userManager.GetUserId(User);

            try
            {
                await _mealPlanService.RemoveFromPlanAsync(mealPlanId, userId);
                TempData["SuccessMessage"] = "Recipe removed from your meal plan!";
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ShoppingList()
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            
            if (today.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = startOfWeek.AddDays(7);
            }

            var shoppingList = await _mealPlanService.GenerateShoppingListAsync(userId, startOfWeek);
            ViewBag.StartOfWeek = startOfWeek;
            
            return View(shoppingList);
        }
    }
}
