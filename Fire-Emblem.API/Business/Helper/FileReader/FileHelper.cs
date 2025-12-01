using Fire_Emblem.Common.Models;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Helper.FileReader
{
  public class FileHelper
  {
    public static async Task<string> ReadFromFileAsync<T>(string filePath)
    {
      if (!System.IO.File.Exists(filePath))
      {
        return null;
      }
      try
      {
        var jsonString = await System.IO.File.ReadAllTextAsync(filePath);
        var jsonList = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
        return jsonString;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static async Task WriteToFileAsync<T>(T item, string filePath)
    {
      var existingJson = await ReadFromFileAsync<T>(filePath);
      var jsonList = new List<T>();
      if (existingJson != null)
      {
        jsonList = JsonSerializer.Deserialize<List<T>>(existingJson);
      }
      jsonList?.Add(item);
      var jsonString = JsonSerializer.Serialize(jsonList, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
      await System.IO.File.WriteAllTextAsync(filePath, jsonString);
    }

    public static async Task<bool> DeleteFromFileAsync<T>(string id, string filePath)
    {
      var items = await ReadFromFileAsync<T>(filePath);
      Type type = typeof(T);
      JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
      List<T> itemsList = JsonSerializer.Deserialize<List<T>>(items);
      PropertyInfo idProperty = type.GetProperty("Id");
      if (idProperty != null && itemsList != null)
      {
        var item = itemsList.FirstOrDefault(item =>
        {
          var idValue = idProperty.GetValue(item);
          if (idValue is string)
          {
            return (string)idValue == id;
          }
          else if (idValue is int)
          {
            return (int)idValue == int.Parse(id);
          }
          return false;
        });
        if (item != null)
        {
          var delete = itemsList.Remove(item);
          var jsonString = JsonSerializer.Serialize(itemsList, options);
          await System.IO.File.WriteAllTextAsync(filePath, jsonString);
          return true;
        }
      }
      return false;
    }

    public static async Task<bool> UpdateFileAsync<T>(T item, string id, string filePath)
    {
      var items = await ReadFromFileAsync<T>(filePath);
      Type itemType = typeof(T);
      JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
      List<T> itemsList = string.IsNullOrWhiteSpace(items) ? new List<T>() : JsonSerializer.Deserialize<List<T>>(items);
      PropertyInfo idProperty = typeof(T).GetProperty("Id");
      if (itemsList != null && idProperty != null)
      {
        var updateItem = itemsList.FirstOrDefault(item =>
        {
          var idValue = idProperty.GetValue(item);
          if (idValue is string)
          {
            return (string)idValue == id;
          }
          else if (idValue is int)
          {
            return (int)idValue == int.Parse(id);
          }
          return false;
        });

        if (updateItem != null)
        {
          int i = itemsList.IndexOf(updateItem);
          updateItem = item;
          itemsList[i] = updateItem;
          var jsonString = JsonSerializer.Serialize(itemsList, options);
          await System.IO.File.WriteAllTextAsync(filePath, jsonString);
          return true;
        }
      }
      return false;
    }

    public static async Task WriteNewCharacter(Character character, string filePath)
    {
      var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
      var jsonString = JsonSerializer.Serialize(character, options);

      await System.IO.File.WriteAllTextAsync(filePath, jsonString);
    }
  }
}
