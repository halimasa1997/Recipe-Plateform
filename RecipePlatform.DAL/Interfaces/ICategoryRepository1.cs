using RecipePlatform.Models.Entities;

namespace RecipePlatform.DAL.Interfaces
{
    public interface ICategoryRepository1
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetCategoriesWithRecipeCountAsync();
        Task<Category> GetCategoryByNameAsync(string name);
    }
}