using Microsoft.AspNetCore.Mvc;
using RecipePlatform.DAL.Interfaces;
using RecipePlatform.MVC.Models;
using RecipePlatform.MVC.Views.Home.ViewModels;
using System.Diagnostics;

namespace RecipePlatform.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipeRepository _recipeService;

        public HomeController(
            IRecipeRepository recipeService)
        {
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                LatestRecipes = (IEnumerable<API.DTOs.RecipeDto>)await _recipeService.GetLatestRecipesAsync(6),
                TopRatedRecipes = (IEnumerable<API.DTOs.RecipeDto>)await _recipeService.GetTopRatedRecipesAsync(6),
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}