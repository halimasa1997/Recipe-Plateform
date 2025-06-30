using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipePlatform.API.DTOs;
using RecipePlatform.DAL.UnitOfWork;
using RecipePlatform.Models.Entities;

namespace RecipePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
    }
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            try
            {
                var categories = await _unitOfWork.Categories.GetCategoriesWithRecipeCountAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/recipes")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByCategory(int id)
        {
            try
            {
                var recipes = await _unitOfWork.Recipes.GetRecipesByCategoryAsync(id);

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

