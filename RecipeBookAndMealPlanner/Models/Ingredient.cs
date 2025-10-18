namespace RecipeBookAndMealPlanner.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
