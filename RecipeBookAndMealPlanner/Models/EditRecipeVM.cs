using System.ComponentModel.DataAnnotations;

public class RecipeEditViewModel
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }

    [Required]
    public string Instructions { get; set; }

    public List<string> Ingredients { get; set; } = new List<string>();
}