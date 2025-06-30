using RecipePlatform.Models.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.Models.Entities
{

    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Ingredients { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Instructions { get; set; }

        [Range(1, 1440)]
        public int PrepTimeMinutes { get; set; }

        [Range(1, 1440)]
        public int CookTimeMinutes { get; set; }

        [Range(1, 50)]
        public int Servings { get; set; }

        public DifficultyLevel Difficulty { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? ImageUrl { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        [NotMapped]
        public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Score) : 0;

        [NotMapped]
        public int TotalTime => PrepTimeMinutes + CookTimeMinutes;
    }
}