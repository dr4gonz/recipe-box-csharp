using System.Collections.Generic;
using Nancy;
using Nancy.ViewEngines.Razor;

namespace RecipeBox
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] = _ =>
      {
        return View["index.cshtml"];
      };
      Get["/recipes"] = _ =>
      {
        List<Recipe> allRecipes = Recipe.GetAll();
        return View["recipes.cshtml", allRecipes];
      };
      Get["/recipes/{id}"] = parameters =>
      {
        Recipe newRecipe = Recipe.Find(parameters.id);
        return View["recipe.cshtml", newRecipe];
      };
      Get["/categories"] = _ =>
      {
        List<Category> allCategories = Category.GetAll();
        return View["categories.cshtml", allCategories];
      };
      Get["/categories/{id}"] = parameters =>
      {
        Category newCategory = Category.Find(parameters.id);
        return View["category.cshtml", newCategory];
      };
      Get["/recipes/add"] = _ => View["add_recipe.cshtml"];
      Post["/recipes/add"] = _ =>
      {
        List<Ingredient> ingredients = new List<Ingredient>{};
        Recipe newRecipe = new Recipe(Request.Form["recipe-title"], Request.Form["instructions"], ingredients);
        newRecipe.Save();
        return View["recipe.cshtml", newRecipe];
      };
      Delete["/recipes/{id}/delete"] = parameters =>
      {
        Recipe recipe = Recipe.Find(parameters.id);
        recipe.Delete();
        List<Recipe> allRecipes = Recipe.GetAll();
        return View["recipes.cshtml", allRecipes];
      };
      Get["/categories/add"] = _ => View["add_category.cshtml"];
      Post["/categories/add"] = _ =>
      {
        Category newCategory = new Category(Request.Form["category-title"]);
        newCategory.Save();
        return View["category.cshtml", newCategory];
      };
      Delete["/categories/{id}/delete"] = parameters =>
      {
        Category category = Category.Find(parameters.id);
        category.Delete();
        List<Category> allCategories = Category.GetAll();
        return View["categories.cshtml", allCategories];
      };
    }
  }
}
