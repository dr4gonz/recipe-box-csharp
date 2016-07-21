using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipeBox
{
  public class RecipeTest : IDisposable
  {
    private List<Ingredient> ingredients = new List<Ingredient> {};
    private string recipeName = "chocolate chip cookies";
    private string instructions = "1. Mix Ingredients. 2. Bake for 15 minutes";
    public void Dispose()

    {
      Recipe.DeleteAll();
      Category.DeleteAll();
      Ingredient.DeleteAll();
    }

    public RecipeTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipe_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Recipe.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Recipe firstRecipe = new Recipe(recipeName, instructions, ingredients);
      Recipe secondRecipe = new Recipe(recipeName, instructions, ingredients);

      //Assert
      Assert.Equal(firstRecipe, secondRecipe);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      //Act
      List<Recipe> result = Recipe.GetAll();
      List<Recipe> testList = new List<Recipe>{testRecipe};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_FindFindsRecipeInDatabase()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();

      //Act
      Recipe result = Recipe.Find(testRecipe.GetId());

      //Assert
      Assert.Equal(testRecipe, result);
    }

    [Fact]
    public void Test_DeleteDeletesRecipeFromDatabase()
    {
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();

      //Act
      testRecipe.Delete();
      List<Recipe> allRecipes = Recipe.GetAll();

      //Assert
      Assert.Equal(0, allRecipes.Count);
    }

    [Fact]
    public void Test_AddCategoryToRecipe()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Category testCategory = new Category("Desserts");
      testCategory.Save();
      List<Category> expectedResult = new List<Category>{testCategory};
      //Act
      testRecipe.AddCategory(testCategory.GetId());
      List<Category> result = testRecipe.GetCategories();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RecipeHasOneCategory_GetAvailableCategories()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Category firstCategory = new Category("Desserts");
      firstCategory.Save();
      Category secondCategory = new Category("Baking");
      secondCategory.Save();
      List<Category> expectedResult = new List<Category>{firstCategory};
      //Act
      testRecipe.AddCategory(secondCategory.GetId());
      List<Category> result = testRecipe.GetAvailableCategories();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RecipeHasNoCategory_GetAvailableCategories()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Category testCategory = new Category("Desserts");
      testCategory.Save();
      List<Category> expectedResult = new List<Category>{testCategory};
      //Act
      List<Category> result = testRecipe.GetAvailableCategories();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RecipeEdit_EditsRecipeNameInstructions()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      //Act
      testRecipe.EditRecipe("Dark Chocolate Chip Cookies", instructions);
      string expectedResult = "Dark Chocolate Chip Cookies";
      string result = testRecipe.GetName();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RemoveCategoryFromRecipe()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Category testCategory = new Category("Desserts");
      testCategory.Save();
      testRecipe.AddCategory(testCategory.GetId());
      testRecipe.RemoveCategory(testCategory.GetId());
      List<Category> expectedResult = new List<Category>{};
      //Act
      List<Category> result = testRecipe.GetCategories();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_AddIngredientToRecipe()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();
      List<Ingredient> expectedResult = new List<Ingredient>{testIngredient};
      //Act
      testRecipe.AddIngredient(testIngredient.GetId());
      List<Ingredient> result = testRecipe.GetIngredientsFromTable();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RemoveIngredientFromRecipe()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();
      testRecipe.AddIngredient(testIngredient.GetId());
      testRecipe.RemoveIngredient(testIngredient.GetId());
      List<Ingredient> expectedResult = new List<Ingredient>{};
      //Act
      List<Ingredient> result = testRecipe.GetIngredientsFromTable();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RecipeHasOneIngredient_GetAvailableIngredients()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Ingredient firstIngredient = new Ingredient("Flour");
      firstIngredient.Save();
      Ingredient secondIngredient = new Ingredient("Sugar");
      secondIngredient.Save();
      List<Ingredient> expectedResult = new List<Ingredient>{firstIngredient};
      //Act
      testRecipe.AddIngredient(secondIngredient.GetId());
      List<Ingredient> result = testRecipe.GetAvailableIngredients();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_RecipeHasNoIngredient_GetAvailableIngredients()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients);
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();
      List<Ingredient> expectedResult = new List<Ingredient>{testIngredient};
      //Act
      List<Ingredient> result = testRecipe.GetAvailableIngredients();
      //Assert
      Assert.Equal(expectedResult, result);
    }


  }
}
