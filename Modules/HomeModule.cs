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
      Get["/recipes"] = _ =>
      {
        List<Recipe> allRecipes = Recipe.GetAll();
        return View["recipes.cshtml", allRecipes];
      };
      Post["/recipes/sort"] = _ => {
        List<Recipe> allRecipes = Recipe.GetAll("rating DESC;");
        return View["recipes.cshtml", allRecipes];
      };
      Get["/recipes/{id}"] = parameters =>
      {
        Recipe newRecipe = Recipe.Find(parameters.id);
        return View["recipe.cshtml", newRecipe];
      };
      Get["/recipes/add"] = _ => View["add_recipe.cshtml"];
      Post["/recipes/add"] = _ =>
      {
        List<Ingredient> ingredients = new List<Ingredient>{};
        Recipe newRecipe = new Recipe(Request.Form["recipe-title"], Request.Form["instructions"], ingredients, 0);
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
      Post["/recipes/{id}/add-category"] = parameters =>
      {
        Recipe recipe = Recipe.Find(parameters.id);
        recipe.AddCategory(Request.Form["category"]);
        return View["recipe.cshtml", recipe];
      };
      Post["/recipes/{id}/add-ingredient"] = parameters =>
      {
        Recipe recipe = Recipe.Find(parameters.id);
        recipe.AddIngredient(Request.Form["ingredient"]);
        return View["recipe.cshtml", recipe];
      };
      Get["/recipes/update/{id}"] = parameters => {
        Recipe recipe = Recipe.Find(parameters.id);
        return View["recipe_edit.cshtml", recipe];
      };
      Patch["/recipes/{id}"] = parameters => {
        Recipe recipe = Recipe.Find(parameters.id);
        recipe.EditRecipe(Request.Form["recipe-title"], Request.Form["instructions"], Request.Form["rating"]);
        return View["recipe.cshtml", recipe];
      };
      Delete["/recipes/update/{id}"] = parameters =>
      {
        Recipe recipe = Recipe.Find(parameters.id);
        recipe.RemoveCategory(Request.Form["category-id"]);
        return View["recipe_edit.cshtml", recipe];
      };
      Get["/ingredients"] = _ => {
        List<Ingredient> allIngredients = Ingredient.GetAll();
        return View["ingredients.cshtml", allIngredients];
      };
      Get["/ingredients/{id}"] = parameters => {
        Ingredient ingredient = Ingredient.Find(parameters.id);
        return View["ingredient.cshtml", ingredient];
      };
      Get["/ingredients/add"] = _ => {
        List<Ingredient> allIngredients = Ingredient.GetAll();
        return View["add_ingredient.cshtml", allIngredients];
      };
      Post["/ingredients/add"] = _ => {
        Ingredient newIngredient = new Ingredient(Request.Form["ingredient-name"]);
        newIngredient.Save();
        List<Ingredient> allIngredients = Ingredient.GetAll();
        return View["add_ingredient.cshtml", allIngredients];
      };
      Delete["/ingredients/{id}/delete"] = parameters => {
        Ingredient ingredient = Ingredient.Find(parameters.id);
        ingredient.Delete();
        List<Ingredient> allIngredients = Ingredient.GetAll();
        return View["ingredients.cshtml", allIngredients];
      };
      Delete["/clear"] = _ =>
      {
        Ingredient.DeleteAll();
        Recipe.DeleteAll();
        Category.DeleteAll();
        return View["index.cshtml"];
      };

    }
  }
}
