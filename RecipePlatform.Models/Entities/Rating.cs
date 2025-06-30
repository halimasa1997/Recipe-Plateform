using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipePlatform.Models.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int RecipeId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}