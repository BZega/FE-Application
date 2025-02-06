using Fire_Emblem.Common.Models;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Fire_Emblem.API.Business.Helper.FileReader
{
    public class FileHelper
    {
        public static string ReadFromFile<T>(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }
            try
            {
                var jsonString = System.IO.File.ReadAllText(filePath);
                var jsonList = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
                return jsonString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void WriteToFile<T>(T item, string filePath)
        {
            var existingJson = ReadFromFile<T>(filePath);
            var jsonList = new List<T>();
            if (existingJson != null)
            {
                jsonList = JsonSerializer.Deserialize<List<T>>(existingJson);
            }
            jsonList?.Add(item);
            var jsonString = JsonSerializer.Serialize(jsonList, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            System.IO.File.WriteAllText(filePath, jsonString);
        }

        public static bool DeleteFromFile<T>(string id, string filePath)
        {
            var items = ReadFromFile<T>(filePath);
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
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            return false;
        }

        public static bool UpdateFile<T>(T item, string id, string filePath)
        {
            var items = ReadFromFile<T>(filePath);
            Type itemType = typeof(T);
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            List<T> itemsList = JsonSerializer.Deserialize<List<T>>(items);
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
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            return false;
        }
    }
}
