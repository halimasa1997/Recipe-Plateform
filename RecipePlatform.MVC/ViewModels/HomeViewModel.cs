using RecipePlatform.API.DTOs;
using System.Collections.Generic;




namespace RecipePlatform.MVC.Views.Home.ViewModels
{
        public class HomeViewModel
        {
            public IEnumerable<RecipeDto> LatestRecipes { get; set; }
            public IEnumerable<RecipeDto> TopRatedRecipes { get; set; }
            //public IEnumerable<CategoryDto> Categories { get; set; }
        }
    }

