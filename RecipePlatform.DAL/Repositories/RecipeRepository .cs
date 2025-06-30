using RecipePlatform.DAL.Context;
using RecipePlatform.DAL.Interfaces;
using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.DAL.Repositories
{

    public class RecipeRepository : GenericRepository<Recipe>, IGenericRepository<Recipe>
    {
        public RecipeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .Where(r => r.CategoryId == categoryId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByUserAsync(string userId)
        {
            return await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var lowerSearchTerm = searchTerm.ToLower();
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .Where(r => r.Title.ToLower().Contains(lowerSearchTerm) ||
                           r.Description.ToLower().Contains(lowerSearchTerm) ||
                           r.Ingredients.ToLower().Contains(lowerSearchTerm) ||
                           r.Category.Name.ToLower().Contains(lowerSearchTerm))
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetTopRatedRecipesAsync(int count = 10)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .Where(r => r.Ratings.Any())
                .OrderByDescending(r => r.Ratings.Average(rt => rt.Score))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetLatestRecipesAsync(int count = 10)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                .OrderByDescending(r => r.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Recipe> GetRecipeWithDetailsAsync(int recipeId)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.Ratings)
                    .Include(rt => rt.User)
                .FirstOrDefaultAsync(r => r.RecipeId == recipeId);
        }

       /* public async Task<double> GetAverageRatingAsync(int recipeId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.RecipeId == recipeId)
                .Select(r => r.Score)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0;
        }*/
    }
}