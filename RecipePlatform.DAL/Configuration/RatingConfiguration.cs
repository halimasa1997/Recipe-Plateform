using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipePlatform.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipePlatform.DAL.Configuration
{public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.RatingId);

        builder.Property(r => r.Score)
               .IsRequired()
               .HasAnnotation("Range", new[] { 1, 5 });

        builder.Property(r => r.Comment)
               .HasMaxLength(1000);

        builder.Property(r => r.CreatedDate)
               .IsRequired();

        builder.Property(r => r.UserId)
               .IsRequired();

        builder.Property(r => r.Recipe)
               .IsRequired();

        // Configure relationships with explicit cascade behavior
        builder.HasOne(r => r.User)
               .WithMany() // Define inverse navigation if needed
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(r => r.Recipe)
               .WithMany(recipe => recipe.Ratings)
               .HasForeignKey(r => r.Recipe)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

}
