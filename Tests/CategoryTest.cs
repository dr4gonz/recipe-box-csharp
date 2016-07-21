using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace RecipeBox
{
  public class CategoryTest : IDisposable
  {
    private List<Ingredient> ingredients = new List<Ingredient> {};
    private string recipeName = "chocolate chip cookies";
    private string instructions = "1. Mix Ingredients. 2. Bake for 15 minutes";
    public void Dispose()

    {
      Category.DeleteAll();
      Recipe.DeleteAll();
      Ingredient.DeleteAll();
    }

    public CategoryTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipe_box_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Category.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void Test_EqualOverrideTrueForSameDescription()
    {
      //Arrange, Act
      Category firstCategory = new Category("Desserts");
      Category secondCategory = new Category("Desserts");

      //Assert
      Assert.Equal(firstCategory, secondCategory);
    }

    [Fact]
    public void Test_Save()
    {
      //Arrange
      Category testCategory = new Category("Desserts");
      testCategory.Save();
      //Act
      List<Category> result = Category.GetAll();
      List<Category> testList = new List<Category>{testCategory};

      //Assert
      Assert.Equal(testList, result);
    }

    [Fact]
    public void Test_FindFindsCategoryInDatabase()
    {
      //Arrange
      Category testCategory = new Category("Desserts");
      testCategory.Save();

      //Act
      Category result = Category.Find(testCategory.GetId());

      //Assert
      Assert.Equal(testCategory, result);
    }

    [Fact]
    public void Test_DeleteDeletesCategoryFromDatabase()
    {
      Category testCategory = new Category("Desserts");
      testCategory.Save();

      //Act
      testCategory.Delete();
      List<Category> allCategorys = Category.GetAll();

      //Assert
      Assert.Equal(0, allCategorys.Count);
    }

    [Fact]
    public void Test_GetRecipes_GetsRecipesForACategory()
    {
      //Arrange
      Recipe testRecipe = new Recipe(recipeName, instructions, ingredients, 5);
      testRecipe.Save();
      Category testCategory = new Category("Desserts");
      testCategory.Save();
      List<Recipe> expectedResult = new List<Recipe>{testRecipe};
      //Act
      testRecipe.AddCategory(testCategory.GetId());
      List<Recipe> result = testCategory.GetRecipes();
      //Assert
      Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Test_GetAllWithArgument_ReturnsSortedList()
    {
      //Arrange
      Recipe firstRecipe = new Recipe(recipeName, instructions, ingredients, 1);
      Recipe secondRecipe = new Recipe(recipeName, instructions, ingredients, 5);
      firstRecipe.Save();
      secondRecipe.Save();
      Category newCategory = new Category("baking");
      newCategory.Save();
      firstRecipe.AddCategory(newCategory.GetId());
      secondRecipe.AddCategory(newCategory.GetId());
      //Act
      List<Recipe> result = newCategory.GetRecipes("rating DESC;");
      List<Recipe> testList = new List<Recipe>{secondRecipe, firstRecipe};

      //Assert
      Assert.Equal(testList, result);
    }

  }
}
