using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipeBox
{
  public class Recipe
  {
    private int _id;
    private string _name;
    private string _instructions;
    private List<Ingredient> _ingredients;

    public Recipe(string name, string instructions, List<Ingredient> ingredients, int id = 0)
    {
      _id = id;
      _name = name;
      _instructions = instructions;
      _ingredients = ingredients;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public string GetInstructions()
    {
      return _instructions;
    }
    public List<Ingredient> GetIngredients()
    {
      return _ingredients;
    }

    public override bool Equals(System.Object otherRecipe)
    {
      if (!(otherRecipe is Recipe))
      {
        return false;
      }
      else
      {
        Recipe newRecipe = (Recipe) otherRecipe;
        bool idEquality = this.GetId() == newRecipe.GetId();
        bool nameEquality = this.GetName() == newRecipe.GetName();
        bool instructionEquality = this.GetInstructions() == newRecipe.GetInstructions();
        bool ingredientEquality = Ingredient.ListEquality(this.GetIngredients(), newRecipe.GetIngredients());
        return (idEquality && nameEquality && instructionEquality && ingredientEquality);
      }
    }

    public string WriteToString()
    {
      string result = _name +"\n";
      result += _instructions + "\n";
      result += _id + "\n";
      foreach (Ingredient ingredient in _ingredients)
      {
        result += ingredient.GetName() +" - " +ingredient.GetId() +"\n";
      }
      return result;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM recipes; DELETE FROM recipes_categories; DELETE FROM recipes_ingredients;", conn);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void Delete()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM recipes WHERE id = @RecipeId;", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this.GetId();

      cmd.Parameters.Add(recipeIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }


    public static List<Recipe> GetAll()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM recipes;", conn);
      rdr = cmd.ExecuteReader();

      List<Recipe> allRecipes = new List<Recipe>{};
      List<Ingredient> testList = new List<Ingredient>{};
      while(rdr.Read())
      {
        int recipeId = rdr.GetInt32(0);
        string recipeName = rdr.GetString(1);
        string recipeInstructions = rdr.GetString(2);
        Recipe newRecipe = new Recipe(recipeName, recipeInstructions, testList, recipeId);
        allRecipes.Add(newRecipe);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allRecipes;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO recipes (name, instructions) OUTPUT INSERTED.id VALUES (@RecipeName, @RecipeInstructions);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@RecipeName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);

      SqlParameter instructionParameter = new SqlParameter();
      instructionParameter.ParameterName = "@RecipeInstructions";
      instructionParameter.Value = this.GetInstructions();
      cmd.Parameters.Add(instructionParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public static Recipe Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM recipes WHERE id = @RecipeId;", conn);
      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = id.ToString();
      cmd.Parameters.Add(recipeIdParameter);
      rdr = cmd.ExecuteReader();

      int foundRecipeId = 0;
      string foundRecipeName = null;
      string foundInstructions = null;
      while(rdr.Read())
      {
        foundRecipeId = rdr.GetInt32(0);
        foundRecipeName = rdr.GetString(1);
        foundInstructions = rdr.GetString(2);
      }
      List<Ingredient> testList = new List<Ingredient>{};
      Recipe foundRecipe = new Recipe(foundRecipeName, foundInstructions, testList, foundRecipeId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundRecipe;
    }

    public void AddCategory(int categoryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO recipes_categories (recipe_id, category_id) VALUES (@RecipeId, @CategoryId);", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = categoryId;
      cmd.Parameters.Add(categoryIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public void RemoveCategory(int categoryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM recipes_categories WHERE category_id=@CategoryId AND recipe_id=@RecipeId;", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = categoryId;
      cmd.Parameters.Add(categoryIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Category> GetCategories()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      SqlCommand cmd = new SqlCommand("SELECT categories.* FROM recipes JOIN recipes_categories ON (recipes.id = recipes_categories.recipe_id) JOIN categories ON (categories.id = recipes_categories.category_id) WHERE recipes.id = @RecipeId", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      List<Category> categories = new List<Category>{};
      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        categories.Add(newCategory);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return categories;
    }

    public List<Category> GetAvailableCategories()
    {
      List<Category> usedCategories = this.GetCategories();

      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE categories.id NOT IN(SELECT categories.id FROM categories INNER JOIN recipes_categories ON (recipes_categories.category_id = categories.id) WHERE recipes_categories.recipe_id = @RecipeId);", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      List<Category> categories = new List<Category>{};
      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        if (!usedCategories.Contains(newCategory)) categories.Add(newCategory);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return categories;
    }

    public void EditRecipe(string newName, string newInstructions)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE recipes SET name = @NewName, instructions = @NewInstructions OUTPUT INSERTED.name, INSERTED.instructions WHERE id = @RecipeId;", conn);
      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      SqlParameter recipeNameParameter = new SqlParameter();
      recipeNameParameter.ParameterName = "@NewName";
      recipeNameParameter.Value = newName;
      cmd.Parameters.Add(recipeNameParameter);

      SqlParameter recipeInstructionsParameter = new SqlParameter();
      recipeInstructionsParameter.ParameterName = "@NewInstructions";
      recipeInstructionsParameter.Value = newInstructions;
      cmd.Parameters.Add(recipeInstructionsParameter);

      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        _name = rdr.GetString(0);
        _instructions = rdr.GetString(1);
      }
      if(rdr != null) rdr.Close();
      if(conn != null) conn.Close();
    }

    public void AddIngredient(int ingredientId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO recipes_ingredients (recipe_id, ingredient_id) VALUES (@RecipeId, @IngredientId);", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = ingredientId;
      cmd.Parameters.Add(ingredientIdParameter);

      cmd.ExecuteNonQuery();

      Ingredient ingredient = Ingredient.Find(ingredientId);
      this._ingredients.Add(ingredient);

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Ingredient> GetIngredientsFromTable()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      SqlCommand cmd = new SqlCommand("SELECT ingredients.* FROM recipes JOIN recipes_ingredients ON (recipes.id = recipes_ingredients.recipe_id) JOIN ingredients ON (ingredients.id = recipes_ingredients.ingredient_id) WHERE recipes.id = @RecipeId", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      List<Ingredient> ingredients = new List<Ingredient>{};
      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int ingredientId = rdr.GetInt32(0);
        string ingredientName = rdr.GetString(1);
        Ingredient newIngredient = new Ingredient(ingredientName, ingredientId);
        ingredients.Add(newIngredient);
        this._ingredients.Add(newIngredient);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return ingredients;
    }

    public void RemoveIngredient(int ingredientId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("DELETE FROM recipes_ingredients WHERE ingredient_id=@IngredientId AND recipe_id=@RecipeId;", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = ingredientId;
      cmd.Parameters.Add(ingredientIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Ingredient> GetAvailableIngredients()
    {
      List<Ingredient> usedIngredients = this.GetIngredientsFromTable();

      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients WHERE ingredients.id NOT IN(SELECT ingredients.id FROM ingredients INNER JOIN recipes_ingredients ON (recipes_ingredients.ingredient_id = ingredients.id) WHERE recipes_ingredients.recipe_id = @RecipeId);", conn);

      SqlParameter recipeIdParameter = new SqlParameter();
      recipeIdParameter.ParameterName = "@RecipeId";
      recipeIdParameter.Value = this._id;
      cmd.Parameters.Add(recipeIdParameter);

      List<Ingredient> ingredients = new List<Ingredient>{};
      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int ingredientId = rdr.GetInt32(0);
        string ingredientName = rdr.GetString(1);
        Ingredient newIngredient = new Ingredient(ingredientName, ingredientId);
        if (!usedIngredients.Contains(newIngredient)) ingredients.Add(newIngredient);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return ingredients;
    }

  }
}
