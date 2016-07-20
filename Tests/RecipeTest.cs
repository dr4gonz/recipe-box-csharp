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

  }
}
