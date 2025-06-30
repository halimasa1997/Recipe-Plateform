using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.DAL.Configuration
{

    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.HasKey(r => r.RecipeId);

            builder.Property(r => r.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(r => r.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(r => r.Ingredients)
                   .IsRequired()
                   .HasColumnType("text");

            builder.Property(r => r.Instructions)
                   .IsRequired()
                   .HasColumnType("text");

            builder.Property(r => r.PrepTimeMinutes)
                   .IsRequired()
                   .HasAnnotation("Range", new[] { 1, 1440 });

            builder.Property(r => r.CookTimeMinutes)
                   .IsRequired()
                   .HasAnnotation("Range", new[] { 1, 1440 });

            builder.Property(r => r.Servings)
                   .IsRequired()
                   .HasAnnotation("Range", new[] { 1, 50 });

            builder.Property(r => r.CreatedDate)
                   .IsRequired();

            builder.Property(r => r.UserId)
                   .IsRequired();

            builder.Property(r => r.CategoryId)
                   .IsRequired();

            // Configure relationships
            builder.HasOne(r => r.User)
                   .WithMany() // Define inverse navigation if needed
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Category)
                   .WithMany() // Define inverse navigation if needed
                   .HasForeignKey(r => r.CategoryId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configure collection navigation
            builder.HasMany(r => r.Ratings)
                   .WithOne(rating => rating.Recipe)
                   .HasForeignKey(rating => rating.Recipe)
                   .OnDelete(DeleteBehavior.Cascade);

            // Ignore calculated properties
            builder.Ignore(r => r.AverageRating);
            builder.Ignore(r => r.TotalTime);
        }
    }
}
