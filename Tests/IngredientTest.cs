using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipeBox
{
  public class IngredientTest : IDisposable
  {
    private List<Ingredient> ingredients = new List<Ingredient> {};
    private string recipeName = "chocolate chip cookies";
    private string instructions = "1. Mix Ingredients. 2. Bake for 15 minutes";

    public void Dispose()
    {
      Ingredient.DeleteAll();
      Recipe.DeleteAll();
      Category.DeleteAll();
    }

    public IngredientTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipe_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Ingredient.GetAll().Count;
      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Ingredient firstIngredient = new Ingredient("Flour");
      Ingredient secondIngredient = new Ingredient("Flour");

      //Assert
      Assert.Equal(firstIngredient, secondIngredient);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();
      //Act
      List<Ingredient> result = Ingredient.GetAll();
      List<Ingredient> testList = new List<Ingredient>{testIngredient};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_FindFindsIngredientInDatabase()
    {
      //Arrange
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();

      //Act
      Ingredient result = Ingredient.Find(testIngredient.GetId());

      //Assert
      Assert.Equal(testIngredient, result);
    }

    [Fact]
    public void Test_DeleteDeletesIngredientFromDatabase()
    {
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();

      //Act
      testIngredient.Delete();
      List<Ingredient> allIngredients = Ingredient.GetAll();

      //Assert
      Assert.Equal(0, allIngredients.Count);
    }

    [Fact]
    public void Test_GetRecipes_GetsRecipesForAnIngredient()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients, 5);
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Flour");
      testIngredient.Save();
      testRecipe.AddIngredient(testIngredient.GetId());
      //Act
      Recipe result = testIngredient.GetRecipes()[0];
      //Assert
      Assert.Equal(testRecipe, result);
    }

    [Fact]
    public void Test_GetAllWithArgument_ReturnsSortedList()
    {
      //Arrange
      Recipe firstRecipe = new Recipe(recipeName, instructions, ingredients, 1);
      Recipe secondRecipe = new Recipe(recipeName, instructions, ingredients, 5);
      firstRecipe.Save();
      secondRecipe.Save();
      Ingredient newIngredient = new Ingredient("Flour");
      newIngredient.Save();
      firstRecipe.AddIngredient(newIngredient.GetId());
      secondRecipe.AddIngredient(newIngredient.GetId());
      //Act
      List<Recipe> result = newIngredient.GetRecipes("rating DESC;");
      List<Recipe> testList = new List<Recipe>{secondRecipe, firstRecipe};

      //Assert
      Assert.Equal(testList, result);
    }


  }
}
