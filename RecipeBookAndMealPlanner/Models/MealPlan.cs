using RecipeBookAndMealPlanner.Data;
using RecipeBookAndMealPlanner.Models;
using System.ComponentModel.DataAnnotations;

public enum MealType { Breakfast, Lunch, Dinner }

public class MealPlan
{
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public MealType MealType { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
}