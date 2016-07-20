using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
namespace RecipeBox
{


  public class SerializableList
  {

    public static string GetXMLFromObject(List<string> ingredients)
    {
      StringWriter sw = new StringWriter();
      XmlTextWriter tw = null;
      try
      {
        XmlSerializer serializer = new XmlSerializer(ingredients.GetType());
        tw = new XmlTextWriter(sw);
        serializer.Serialize(tw, ingredients);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine("Error: Problem Serializing Ingredients");
      }
      finally
      {
        sw.Close();
        if (tw != null)
        {
          tw.Close();
        }
      }
      return sw.ToString();
    }

    public static List<String> ObjectToXML(string xml, Type objectType)
    {
      StringReader strReader = null;
      XmlSerializer serializer = null;
      XmlTextReader xmlReader = null;
      List<string> ingredients = new List<string>{};
      try
      {
        strReader = new StringReader(xml);
        serializer = new XmlSerializer(objectType);
        xmlReader = new XmlTextReader(strReader);
        ingredients = (List<string>) serializer.Deserialize(xmlReader);

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine("Error: Problem Deserializing Ingredients");
      }
      finally
      {
        if (xmlReader != null)
        {
          xmlReader.Close();
        }
        if (strReader != null)
        {
          strReader.Close();
        }
      }
      return ingredients;
    }

  }
}
