using RecipePlatform.DAL.UnitOfWork;
using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.BLL.Services
{
    public class RecipeServiceWithUoW
    {
        private readonly IUnitOfWork _unitOfWork;

        public RecipeServiceWithUoW(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Create recipe with rating in a transaction
        public async Task<bool> CreateRecipeWithInitialRatingAsync(Recipe recipe, Rating initialRating)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Add recipe
                recipe.CreatedDate = DateTime.UtcNow;
                await _unitOfWork.Recipes.AddAsync(recipe);
                await _unitOfWork.SaveChangesAsync(); // Save to get RecipeId

                // Add initial rating
                initialRating.RecipeId = recipe.RecipeId;
                initialRating.CreatedDate = DateTime.UtcNow;
                await _unitOfWork.Ratings.AddAsync(initialRating);

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
        }

        // Get dashboard data efficiently
        public async Task<object> GetDashboardDataAsync(string userId)
        {
            try
            {
                // Use parallel execution for independent queries
                var userRecipesTask = _unitOfWork.Recipes.GetRecipesByUserAsync(userId);
                var topRatedTask = _unitOfWork.Recipes.GetTopRatedRecipesAsync(5);
                var latestTask = _unitOfWork.Recipes.GetLatestRecipesAsync(5);
                var categoriesTask = _unitOfWork.Categories.GetCategoriesWithRecipeCountAsync();

                await Task.WhenAll(userRecipesTask, topRatedTask, latestTask, categoriesTask);

                return new
                {
                    UserRecipes = await userRecipesTask,
                    TopRated = await topRatedTask,
                    Latest = await latestTask,
                    Categories = await categoriesTask
                };
            }
            catch
            {
                return null;
            }
        }

        // Complex business operation with multiple entities
        public async Task<bool> UpdateRecipeAndAddRatingAsync(int recipeId, Recipe updatedRecipe, Rating newRating)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Check if user already rated this recipe
                var existingRating = await _unitOfWork.Ratings
                    .GetUserRatingForRecipeAsync(newRating.UserId, recipeId);

                if (existingRating != null)
                {
                    // Update existing rating
                    existingRating.Score = newRating.Score;
                    existingRating.Comment = newRating.Comment;
                    existingRating.CreatedDate = DateTime.UtcNow;
                    _unitOfWork.Ratings.Update(existingRating);
                }
                else
                {
                    // Add new rating
                    newRating.RecipeId = recipeId;
                    newRating.CreatedDate = DateTime.UtcNow;
                    await _unitOfWork.Ratings.AddAsync(newRating);
                }

                // Update recipe
                var existingRecipe = await _unitOfWork.Recipes.GetByIdAsync(recipeId);
                if (existingRecipe != null)
                {
                    existingRecipe.Title = updatedRecipe.Title;
                    existingRecipe.Description = updatedRecipe.Description;
                    existingRecipe.Ingredients = updatedRecipe.Ingredients;
                    existingRecipe.Instructions = updatedRecipe.Instructions;
                    existingRecipe.PrepTimeMinutes = updatedRecipe.PrepTimeMinutes;
                    existingRecipe.CookTimeMinutes = updatedRecipe.CookTimeMinutes;
                    existingRecipe.Servings = updatedRecipe.Servings;
                    existingRecipe.Difficulty = updatedRecipe.Difficulty;
                    existingRecipe.CategoryId = updatedRecipe.CategoryId;

                    _unitOfWork.Recipes.Update(existingRecipe);
                }

                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return false;
            }
        }
    }

}
