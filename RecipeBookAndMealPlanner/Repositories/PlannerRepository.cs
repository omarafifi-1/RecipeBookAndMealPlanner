using Microsoft.EntityFrameworkCore;
using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Interfaces;
using RecipeBookAndMealPlanner.Models;

namespace RecipeBookAndMealPlanner.Repositories
{
    public class PlannerRepository : IPlannerRepository
    {
        private readonly AppDbContext _context;

        public PlannerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MealPlan>> GetMealPlansForWeekAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.MealPlans
                .Include(mp => mp.Recipe)
                    .ThenInclude(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                .Where(mp => mp.UserId == userId && mp.Date >= startDate && mp.Date <= endDate)
                .OrderBy(mp => mp.Date)
                .ThenBy(mp => mp.MealType)
                .ToListAsync();
        }

        public async Task<MealPlan?> GetMealPlanAsync(int id, string userId)
        {
            return await _context.MealPlans
                .Include(mp => mp.Recipe)
                .FirstOrDefaultAsync(mp => mp.Id == id && mp.UserId == userId);
        }

        public async Task<MealPlan?> GetMealPlanByDetailsAsync(string userId, DateTime date, MealType mealType)
        {
            return await _context.MealPlans
                .FirstOrDefaultAsync(mp => mp.UserId == userId && mp.Date.Date == date.Date && mp.MealType == mealType);
        }

        public async Task AddMealPlanAsync(MealPlan mealPlan)
        {
            await _context.MealPlans.AddAsync(mealPlan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMealPlanAsync(MealPlan mealPlan)
        {
            _context.MealPlans.Update(mealPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMealPlanAsync(MealPlan mealPlan)
        {
            _context.MealPlans.Remove(mealPlan);
            await _context.SaveChangesAsync();
        }
    }
}
