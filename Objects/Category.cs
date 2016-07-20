// using System.Collections.Generic;
// using System.Data.SqlClient;
// using System;
//
// namespace CategoryBox
// {
//   public class Category
//   {
//     private int _id;
//     private string _name;
//     private string _instructions;
//     private List<Ingredient> _ingredients;
//
//     public Category(string name, string instructions, List<Ingredient> ingredients, int id = 0)
//     {
//       _id = id;
//       _name = name;
//       _instructions = instructions;
//       _ingredients = ingredients;
//     }
//
//     public int GetId()
//     {
//       return _id;
//     }
//     public string GetName()
//     {
//       return _name;
//     }
//     public string GetInstructions()
//     {
//       return _instructions;
//     }
//     public List<Ingredient> GetIngredients()
//     {
//       return _ingredients;
//     }
//
//     public override bool Equals(System.Object otherCategory)
//     {
//       if (!(otherCategory is Category))
//       {
//         return false;
//       }
//       else
//       {
//         Category newCategory = (Category) otherCategory;
//         bool idEquality = this.GetId() == newCategory.GetId();
//         bool nameEquality = this.GetName() == newCategory.GetName();
//         return (idEquality && nameEquality);
//       }
//     }
//
//     public static void DeleteAll()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//       SqlCommand cmd = new SqlCommand("DELETE FROM recipes;", conn);
//       cmd.ExecuteNonQuery();
//     }
//
//     public void Delete()
//     {
//       SqlConnection conn = DB.Connection();
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("DELETE FROM recipes WHERE id = @CategoryId;", conn);
//
//       SqlParameter recipeIdParameter = new SqlParameter();
//       recipeIdParameter.ParameterName = "@CategoryId";
//       recipeIdParameter.Value = this.GetId();
//
//       cmd.Parameters.Add(recipeIdParameter);
//       cmd.ExecuteNonQuery();
//
//       if (conn != null)
//       {
//         conn.Close();
//       }
//     }
//
//
//     public static List<Category> GetAll()
//     {
//       SqlConnection conn = DB.Connection();
//       SqlDataReader rdr = null;
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("SELECT * FROM recipes;", conn);
//       rdr = cmd.ExecuteReader();
//
//       List<Category> allCategorys = new List<Category>{};
//       List<Ingredient> testList = new List<Ingredient>{};
//       while(rdr.Read())
//       {
//         int recipeId = rdr.GetInt32(0);
//         string recipeName = rdr.GetString(1);
//         string recipeInstructions = rdr.GetString(2);
//         Category newCategory = new Category(recipeName, recipeInstructions, testList, recipeId);
//         allCategorys.Add(newCategory);
//       }
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//       return allCategorys;
//     }
//
//     public void Save()
//     {
//       SqlConnection conn = DB.Connection();
//       SqlDataReader rdr;
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("INSERT INTO recipes (name, instructions) OUTPUT INSERTED.id VALUES (@CategoryName, @CategoryInstructions);", conn);
//
//       SqlParameter nameParameter = new SqlParameter();
//       nameParameter.ParameterName = "@CategoryName";
//       nameParameter.Value = this.GetName();
//       cmd.Parameters.Add(nameParameter);
//
//       SqlParameter instructionParameter = new SqlParameter();
//       instructionParameter.ParameterName = "@CategoryInstructions";
//       instructionParameter.Value = this.GetInstructions();
//       cmd.Parameters.Add(instructionParameter);
//
//       rdr = cmd.ExecuteReader();
//
//       while(rdr.Read())
//       {
//         this._id = rdr.GetInt32(0);
//       }
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//     }
//
//     public static Category Find(int id)
//     {
//       SqlConnection conn = DB.Connection();
//       SqlDataReader rdr = null;
//       conn.Open();
//
//       SqlCommand cmd = new SqlCommand("SELECT * FROM recipes WHERE id = @CategoryId;", conn);
//       SqlParameter recipeIdParameter = new SqlParameter();
//       recipeIdParameter.ParameterName = "@CategoryId";
//       recipeIdParameter.Value = id.ToString();
//       cmd.Parameters.Add(recipeIdParameter);
//       rdr = cmd.ExecuteReader();
//
//       int foundCategoryId = 0;
//       string foundCategoryName = null;
//       string foundInstructions = null;
//       while(rdr.Read())
//       {
//         foundCategoryId = rdr.GetInt32(0);
//         foundCategoryName = rdr.GetString(1);
//         foundInstructions = rdr.GetString(2);
//       }
//       List<Ingredient> testList = new List<Ingredient>{};
//       Category foundCategory = new Category(foundCategoryName, foundInstructions, testList, foundCategoryId);
//
//       if (rdr != null)
//       {
//         rdr.Close();
//       }
//       if (conn != null)
//       {
//         conn.Close();
//       }
//       return foundCategory;
//     }
//   }
// }
