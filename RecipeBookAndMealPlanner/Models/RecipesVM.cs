using System.ComponentModel.DataAnnotations;

namespace RecipeBookAndMealPlanner.Models
{
    public class RecipesVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Instructions { get; set; }

        public List<string> Ingredients { get; set; } = new List<string>();
    }
}
