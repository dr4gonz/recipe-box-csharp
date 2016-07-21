using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipeBox
{
  public class Ingredient
  {
    private int _id;
    private string _name;

    public Ingredient(string name, int id=0)
    {
      _id = id;
      _name = name;
    }

    public string GetName()
    {
      return _name;
    }
    public int GetId()
    {
      return _id;
    }

    public override bool Equals(System.Object otherIngredient)
    {
      if (!(otherIngredient is Ingredient))
      {
        return false;
      }
      else
      {
        Ingredient newIngredient = (Ingredient) otherIngredient;
        bool idEquality = this.GetId() == newIngredient.GetId();
        bool nameEquality = this.GetName() == newIngredient.GetName();
        return (idEquality && nameEquality);
      }
    }

    public static bool ListEquality(List<Ingredient> list1, List<Ingredient> list2)
    {
      if (list1.Count != list2.Count) return false;
      else
      {
        for (int i = 0; i < list1.Count; i++)
        {
          if (!list1[i].Equals(list2[i])) return false;
        }
      }
      return true;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM ingredients; DELETE FROM recipes_ingredients;", conn);
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

      SqlCommand cmd = new SqlCommand("DELETE FROM ingredients WHERE id = @IngredientId;", conn);

      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = this.GetId();

      cmd.Parameters.Add(ingredientIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }


    public static List<Ingredient> GetAll()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients;", conn);
      rdr = cmd.ExecuteReader();

      List<Ingredient> allIngredients = new List<Ingredient>{};
      while(rdr.Read())
      {
        int ingredientId = rdr.GetInt32(0);
        string ingredientName = rdr.GetString(1);
        Ingredient newIngredient = new Ingredient(ingredientName, ingredientId);
        allIngredients.Add(newIngredient);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allIngredients;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO ingredients (name) OUTPUT INSERTED.id VALUES (@IngredientName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@IngredientName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);

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

    public static Ingredient Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM ingredients WHERE id = @IngredientId;", conn);
      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = id.ToString();
      cmd.Parameters.Add(ingredientIdParameter);
      rdr = cmd.ExecuteReader();

      int foundIngredientId = 0;
      string foundIngredientName = null;
      while(rdr.Read())
      {
        foundIngredientId = rdr.GetInt32(0);
        foundIngredientName = rdr.GetString(1);
      }
      Ingredient foundIngredient = new Ingredient(foundIngredientName, foundIngredientId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundIngredient;
    }

    public List<Recipe> GetRecipes(string sortBy = "name;")
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      string queryString = "SELECT recipes.* FROM ingredients JOIN recipes_ingredients ON (ingredients.id = recipes_ingredients.ingredient_id) JOIN recipes ON (recipes.id = recipes_ingredients.recipe_id) WHERE ingredients.id = @IngredientId ORDER BY " + sortBy;
      SqlCommand cmd = new SqlCommand(queryString, conn);

      SqlParameter ingredientIdParameter = new SqlParameter();
      ingredientIdParameter.ParameterName = "@IngredientId";
      ingredientIdParameter.Value = this._id;
      cmd.Parameters.Add(ingredientIdParameter);

      List<Ingredient> ingredients = new List<Ingredient> {};
      List<Recipe> recipes = new List<Recipe>{};
      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int recipeId = rdr.GetInt32(0);
        string recipeName = rdr.GetString(1);
        string recipeInstructions = rdr.GetString(2);
        int recipeRating = rdr.GetInt32(3);
        Recipe newRecipe = new Recipe(recipeName, recipeInstructions, ingredients, recipeRating, recipeId);
        newRecipe.GetIngredientsFromTable();
        recipes.Add(newRecipe);
      }


      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return recipes;
    }
  }
}
