using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipePlatform.DAL.Configuration;

using RecipePlatform.Models.Entities;

namespace RecipePlatform.DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
           /* modelBuilder.ApplyConfiguration(new RecipeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new RatingConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());*/

            // Or use this to apply all configurations from assembly
             modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Breakfast", Description = "Morning meals" },
                new Category { CategoryId = 2, Name = "Lunch", Description = "Midday meals" },
                new Category { CategoryId = 3, Name = "Dinner", Description = "Evening meals" },
                new Category { CategoryId = 4, Name = "Dessert", Description = "Sweet treats" },
                new Category { CategoryId = 5, Name = "Snacks", Description = "Light bites" },
                new Category { CategoryId = 6, Name = "Beverages", Description = "Drinks and smoothies" }
            );
        }
    }
}