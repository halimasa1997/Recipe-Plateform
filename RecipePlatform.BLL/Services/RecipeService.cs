using RecipePlatform.DAL.Interfaces;
using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.BLL.Services
{

    public class RecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ICategoryRepository _categoryRepository;

        public RecipeService(IRecipeRepository recipeRepository, ICategoryRepository categoryRepository)
        {
            _recipeRepository = recipeRepository;
            _categoryRepository = categoryRepository;
        }

        // Get recipe with all details
        public async Task<Recipe> GetRecipeDetailsAsync(int recipeId)
        {
            return await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
        }

        // Search recipes with business logic
        public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string searchTerm, int? categoryId = null)
        {
            IEnumerable<Recipe> recipes;

            if (categoryId.HasValue)
            {
                recipes = await _recipeRepository.GetRecipesByCategoryAsync(categoryId.Value);
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    var lowerSearchTerm = searchTerm.ToLower();
                    recipes = recipes.Where(r =>
                        r.Title.ToLower().Contains(lowerSearchTerm) ||
                        r.Description.ToLower().Contains(lowerSearchTerm));
                }
            }
            else
            {
                recipes = await _recipeRepository.SearchRecipesAsync(searchTerm);
            }

            return recipes;
        }

        // Create recipe with validation
        public async Task<bool> CreateRecipeAsync(Recipe recipe)
        {
            try
            {
                // Business logic validation
                if (string.IsNullOrWhiteSpace(recipe.Title) ||
                    string.IsNullOrWhiteSpace(recipe.Ingredients))
                {
                    return false;
                }

                // Check if category exists
                var category = await _categoryRepository.GetByIdAsync(recipe.CategoryId);
                if (category == null)
                {
                    return false;
                }

                recipe.CreatedDate = DateTime.UtcNow;
                await _recipeRepository.AddAsync(recipe);
                // Note: SaveChanges would be called by Unit of Work
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
