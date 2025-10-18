using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Models;

namespace RecipeBookAndMealPlanner.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly UserManager<AppUser> _userManager;

        public RecipeController(IRecipeService recipeService, UserManager<AppUser> userManager) 
        {
            _recipeService = recipeService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var VM = new RecipesVM();
            return View(VM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipesVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            await _recipeService.CreateRecipeAsync(model, user.Id);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var recipes = await _recipeService.GetRecipesForUserAsync(userId);
            return View(recipes);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            if(userId == null)
            {
                return Unauthorized();
            }
            var recipe = await _recipeService.GetRecipeDetailsAsync(id, userId);

            if (recipe == null || recipe.UserId != userId)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var recipe = await _recipeService.GetRecipeDetailsAsync(id, userId);

            if (recipe == null)
            {
                return NotFound();
            }
            var viewModel = new RecipeEditViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Instructions = recipe.Instructions,
                Ingredients = recipe.RecipeIngredients.Select(ri =>
                    $"{ri.Quantity} {ri.Unit} {ri.Ingredient.Name}"
                ).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    await _recipeService.UpdateRecipeAsync(viewModel, userId);
                    return RedirectToAction(nameof(Details), new { id = viewModel.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var recipe = await _recipeService.GetRecipeDetailsAsync(id, userId);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _recipeService.DeleteRecipeAsync(id, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}
