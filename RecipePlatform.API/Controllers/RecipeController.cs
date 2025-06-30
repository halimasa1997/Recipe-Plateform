using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipePlatform.API.DTOs;
using RecipePlatform.DAL.Interfaces;
using RecipePlatform.DAL.UnitOfWork;
using RecipePlatform.Models;
using RecipePlatform.Models.Entities;
using System.Security.Claims;

namespace RecipePlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecipesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes()
        {
            try
            {
                var recipes = await _unitOfWork.Recipes.GetWithIncludeAsync(
                    r => r.User,
                    r => r.Category,
                    r => r.Ratings
                );

                var recipeDtos = recipes.Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                    Ingredients = r.Ingredients,
                    Instructions = r.Instructions,
                    PrepTimeMinutes = r.PrepTimeMinutes,
                    CookTimeMinutes = r.CookTimeMinutes,
                    Servings = r.Servings,
                    Difficulty = r.Difficulty,
                    CreatedDate = r.CreatedDate,
                    UserId = r.UserId,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    CategoryId = r.CategoryId,
                    CategoryName = r.Category.Name,
                    AverageRating = r.Ratings.Any() ? (double)r.Ratings.Average(rt => rt.Score) : 0,
                    RatingCount = r.Ratings.Count
                });

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeWithDetailsAsync(id);
                if (recipe == null)
                {
                    return NotFound();
                }

                var recipeDto = new RecipeDto
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    PrepTimeMinutes = recipe.PrepTimeMinutes,
                    CookTimeMinutes = recipe.CookTimeMinutes,
                    Servings = recipe.Servings,
                    Difficulty = recipe.Difficulty,
                    CreatedDate = recipe.CreatedDate,
                    UserId = recipe.UserId,
                    UserName = $"{recipe.User.FirstName} {recipe.User.LastName}",
                    CategoryId = recipe.CategoryId,
                    CategoryName = recipe.Category.Name,
                    AverageRating = recipe.Ratings.Any() ? (double)recipe.Ratings.Average(r => r.Score) : 0,
                    RatingCount = recipe.Ratings.Count
                };

                return Ok(recipeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> SearchRecipes([FromQuery] string term)
        {
            try
            {
                var recipes = await _unitOfWork.Recipes.SearchRecipesAsync(term);

                var recipeDtos = recipes.Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                    PrepTimeMinutes = r.PrepTimeMinutes,
                    CookTimeMinutes = r.CookTimeMinutes,
                    Servings = r.Servings,
                    Difficulty = r.Difficulty,
                    CreatedDate = r.CreatedDate,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    CategoryName = r.Category.Name,
                    AverageRating = r.Ratings.Any() ? (double)r.Ratings.Average(rt => rt.Score) : 0,
                    RatingCount = r.Ratings.Count
                });

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeDto>> CreateRecipe(RecipeCreateDto recipeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var recipe = new Recipe
                {
                    Title = recipeDto.Title,
                    Description = recipeDto.Description,
                    Ingredients = recipeDto.Ingredients,
                    Instructions = recipeDto.Instructions,
                    PrepTimeMinutes = recipeDto.PrepTimeMinutes,
                    CookTimeMinutes = recipeDto.CookTimeMinutes,
                    Servings = recipeDto.Servings,
                    Difficulty = recipeDto.Difficulty,
                    CategoryId = recipeDto.CategoryId,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Recipes.AddAsync(recipe);
                await _unitOfWork.SaveChangesAsync();

                // Get the created the recipe 
                var createdRecipe = await _unitOfWork.Recipes.GetRecipeWithDetailsAsync(recipe.RecipeId);

                var responseDto = new RecipeDto
                {
                    RecipeId = createdRecipe.RecipeId,
                    Title = createdRecipe.Title,
                    Description = createdRecipe.Description,
                    Ingredients = createdRecipe.Ingredients,
                    Instructions = createdRecipe.Instructions,
                    PrepTimeMinutes = createdRecipe.PrepTimeMinutes,
                    CookTimeMinutes = createdRecipe.CookTimeMinutes,
                    Servings = createdRecipe.Servings,
                    Difficulty = createdRecipe.Difficulty,
                    CreatedDate = createdRecipe.CreatedDate,
                    UserId = createdRecipe.UserId,
                    UserName = $"{createdRecipe.User.FirstName} {createdRecipe.User.LastName}",
                    CategoryId = createdRecipe.CategoryId,
                    CategoryName = createdRecipe.Category.Name,
                    AverageRating = 0,
                    RatingCount = 0
                };

                return CreatedAtAction(nameof(GetRecipe), new { id = recipe.RecipeId }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRecipe(int id, RecipeCreateDto recipeDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var existingRecipe = await _unitOfWork.Recipes.GetByIdAsync(id);

                if (existingRecipe == null)
                {
                    return NotFound();
                }

                // Check if user have the the recipe
                if (existingRecipe.UserId != userId)
                {
                    return Forbid();
                }

                existingRecipe.Title = recipeDto.Title;
                existingRecipe.Description = recipeDto.Description;
                existingRecipe.Ingredients = recipeDto.Ingredients;
                existingRecipe.Instructions = recipeDto.Instructions;
                existingRecipe.PrepTimeMinutes = recipeDto.PrepTimeMinutes;
                existingRecipe.CookTimeMinutes = recipeDto.CookTimeMinutes;
                existingRecipe.Servings = recipeDto.Servings;
                existingRecipe.Difficulty = recipeDto.Difficulty;
                existingRecipe.CategoryId = recipeDto.CategoryId;

                _unitOfWork.Recipes.Update(existingRecipe);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var recipe = await _unitOfWork.Recipes.GetByIdAsync(id);

                if (recipe == null)
                {
                    return NotFound();
                }

                // Check if user owns the recipe
                if (recipe.UserId != userId)
                {
                    return Forbid();
                }

                _unitOfWork.Recipes.Remove(recipe);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/rate")]
        [Authorize]
        public async Task<ActionResult<RatingDto>> RateRecipe(int id, CreateRatingDto ratingDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Check if recipe exists
                var recipe = await _unitOfWork.Recipes.GetByIdAsync(id);
                if (recipe == null)
                {
                    return NotFound("Recipe not found");
                }

                // Check if user already rated this recipe
                var existingRating = await _unitOfWork.Ratings.GetUserRatingForRecipeAsync(userId, id);

                if (existingRating != null)
                {
                    // Update existing rating
                    existingRating.Score = (int)ratingDto.Score;
                    existingRating.Comment = ratingDto.Comment;
                    existingRating.CreatedDate = DateTime.UtcNow;

                    _unitOfWork.Ratings.Update(existingRating);
                    await _unitOfWork.SaveChangesAsync();

                    return Ok(new RatingDto
                    {
                        RatingId = existingRating.RatingId,
                        Score = existingRating.Score,
                        Comment = existingRating.Comment,
                        CreatedDate = existingRating.CreatedDate,
                        UserName = $"{existingRating.User.FirstName} {existingRating.User.LastName}"
                    });
                }
                else
                {
                    // Create new rating
                    var newRating = new Rating
                    {
                        RecipeId = id,
                        UserId = userId,
                        Score = (int)ratingDto.Score,
                        Comment = ratingDto.Comment,
                        CreatedDate = DateTime.UtcNow
                    };

                    await _unitOfWork.Ratings.AddAsync(newRating);
                    await _unitOfWork.SaveChangesAsync();

                    // Get the rating  -user detaillls
                    var createdRating = await _unitOfWork.Ratings.FindWithIncludeAsync(
                        r => r.RatingId == newRating.RatingId,
                        r => r.User
                    );
                    var rating = createdRating.FirstOrDefault();

                    return CreatedAtAction("GetRating", new { id = newRating.RatingId }, new RatingDto
                    {
                        RatingId = rating.RatingId,
                        Score = rating.Score,
                        Comment = rating.Comment,
                        CreatedDate = rating.CreatedDate,
                        UserName = $"{rating.User.FirstName} {rating.User.LastName}"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetTopRatedRecipes([FromQuery] int count = 10)
        {
            try
            {
                var recipes = await _unitOfWork.Recipes.GetTopRatedRecipesAsync(count);

                var recipeDtos = recipes.Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                    PrepTimeMinutes = r.PrepTimeMinutes,
                    CookTimeMinutes = r.CookTimeMinutes,
                    Servings = r.Servings,
                    Difficulty = r.Difficulty,
                    CreatedDate = r.CreatedDate,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    CategoryName = r.Category.Name,
                    AverageRating = r.Ratings.Any() ? (double)r.Ratings.Average(rt => rt.Score) : 0,
                    RatingCount = r.Ratings.Count
                });

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetLatestRecipes([FromQuery] int count = 10)
        {
            try
            {
                var recipes = await _unitOfWork.Recipes.GetLatestRecipesAsync(count);

                var recipeDtos = recipes.Select(r => new RecipeDto
                {
                    RecipeId = r.RecipeId,
                    Title = r.Title,
                    Description = r.Description,
                    PrepTimeMinutes = r.PrepTimeMinutes,
                    CookTimeMinutes = r.CookTimeMinutes,
                    Servings = r.Servings,
                    Difficulty = r.Difficulty,
                    CreatedDate = r.CreatedDate,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    CategoryName = r.Category.Name,
                    AverageRating = r.Ratings.Any() ? (double)r.Ratings.Average(rt => rt.Score) : 0,
                    RatingCount = r.Ratings.Count
                });

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
