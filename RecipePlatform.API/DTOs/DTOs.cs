using RecipePlatform.Models.Entities.Enum;

namespace RecipePlatform.API.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class RecipeCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int CategoryId { get; set; }
    }

    public class RecipeDto
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public int PrepTimeMinutes { get; set; }
        public int CookTimeMinutes { get; set; }
        public int Servings { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double AverageRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class RatingDto
    {
        public int RatingId { get; set; }
        public decimal Score { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
    }

    public class CreateRatingDto
    {
        public decimal Score{ get; set; }
        public string Comment { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime Expiration { get; set; }
    }
}
