using RecipePlatform.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRecipeRepository Recipes { get; }
        ICategoryRepository Categories { get; }
        IRatingRepository Ratings { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
