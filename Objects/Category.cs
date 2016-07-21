using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace RecipeBox
{
  public class Category
  {
    private int _id;
    private string _name;

    public Category(string name, int id = 0)
    {
      _id = id;
      _name = name;
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }

    public override bool Equals(System.Object otherCategory)
    {
      if (!(otherCategory is Category))
      {
        return false;
      }
      else
      {
        Category newCategory = (Category) otherCategory;
        bool idEquality = this.GetId() == newCategory.GetId();
        bool nameEquality = this.GetName() == newCategory.GetName();
        return (idEquality && nameEquality);
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM categories; DELETE FROM recipes_categories;", conn);
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

      SqlCommand cmd = new SqlCommand("DELETE FROM categories WHERE id = @CategoryId;", conn);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this.GetId();

      cmd.Parameters.Add(categoryIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }


    public static List<Category> GetAll()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories;", conn);
      rdr = cmd.ExecuteReader();

      List<Category> allCategories = new List<Category>{};
      while(rdr.Read())
      {
        int categoryId = rdr.GetInt32(0);
        string categoryName = rdr.GetString(1);
        Category newCategory = new Category(categoryName, categoryId);
        allCategories.Add(newCategory);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allCategories;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO categories (name) OUTPUT INSERTED.id VALUES (@CategoryName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@CategoryName";
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

    public static Category Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM categories WHERE id = @CategoryId;", conn);
      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = id.ToString();
      cmd.Parameters.Add(categoryIdParameter);
      rdr = cmd.ExecuteReader();

      int foundCategoryId = 0;
      string foundCategoryName = null;
      while(rdr.Read())
      {
        foundCategoryId = rdr.GetInt32(0);
        foundCategoryName = rdr.GetString(1);
      }
      Category foundCategory = new Category(foundCategoryName, foundCategoryId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundCategory;
    }

    public List<Recipe> GetRecipes()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlDataReader rdr = null;
      SqlCommand cmd = new SqlCommand("SELECT recipes.* FROM categories JOIN recipes_categories ON (categories.id = recipes_categories.category_id) JOIN recipes ON (recipes.id = recipes_categories.recipe_id) WHERE categories.id = @CategoryId", conn);

      SqlParameter categoryIdParameter = new SqlParameter();
      categoryIdParameter.ParameterName = "@CategoryId";
      categoryIdParameter.Value = this._id;
      cmd.Parameters.Add(categoryIdParameter);

      List<Recipe> recipes = new List<Recipe>{};
      rdr = cmd.ExecuteReader();
      List<Ingredient> ingredients = new List<Ingredient> {};
      while(rdr.Read())
      {
        int recipeId = rdr.GetInt32(0);
        string recipeName = rdr.GetString(1);
        string recipeInstructions = rdr.GetString(2);
        int recipeRating = rdr.GetInt32(3);
        Recipe newRecipe = new Recipe(recipeName, recipeInstructions, ingredients, recipeRating, recipeId);
        recipes.Add(newRecipe);
      }

      if (rdr != null) rdr.Close();
      if (conn != null) conn.Close();

      return recipes;
    }
  }
}
