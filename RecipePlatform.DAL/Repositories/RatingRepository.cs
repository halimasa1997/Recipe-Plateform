
using RecipePlatform.DAL.Context;
using RecipePlatform.DAL.Interfaces;
using RecipePlatform.DAL.Repositories;
using RecipePlatform.Models.Entities;
using System.Data.Entity;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    public RatingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rating>> GetRatingsByRecipeAsync(int recipeId)
    {
        return await _context.Ratings
            .Include(r => r.User)
            .Where(r => r.RecipeId == recipeId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rating>> GetRatingsByUserAsync(string userId)
    {
        return await _context.Ratings
            .Include(r => r.Recipe)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<Rating> GetUserRatingForRecipeAsync(string userId, int recipeId)
    {
        return await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId);
    }

    public async Task<double> GetAverageRatingForRecipeAsync(int recipeId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.RecipeId == recipeId)
            .Select(r => r.Score)
            .ToListAsync();

        return ratings.Any() ? ratings.Average() : 0;
    }
}

