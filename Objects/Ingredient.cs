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
          if (list1[i] != list2[i]) return false;
        }
      }
      return true;
    }
  }
}
