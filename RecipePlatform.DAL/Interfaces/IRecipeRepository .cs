using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.DAL.Interfaces
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId);
        Task<IEnumerable<Recipe>> GetRecipesByUserAsync(string userId);
        Task<IEnumerable<Recipe>> SearchRecipesAsync(string searchTerm);
        Task<IEnumerable<Recipe>> GetTopRatedRecipesAsync(int count = 10);
        Task<IEnumerable<Recipe>> GetLatestRecipesAsync(int count = 10);
        Task<Recipe> GetRecipeWithDetailsAsync(int recipeId);
        Task<double> GetAverageRatingAsync(int recipeId);
    }

    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<Category> GetCategoryByNameAsync(string name);
        Task<IEnumerable<Category>> GetCategoriesWithRecipeCountAsync();
    }

    public interface IRatingRepository : IGenericRepository<Rating>
    {
        Task<IEnumerable<Rating>> GetRatingsByRecipeAsync(int recipeId);
        Task<IEnumerable<Rating>> GetRatingsByUserAsync(string userId);
        Task<Rating> GetUserRatingForRecipeAsync(string userId, int recipeId);
        Task<double> GetAverageRatingForRecipeAsync(int recipeId);
    }
}
