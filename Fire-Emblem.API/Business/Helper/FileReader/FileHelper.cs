using Fire_Emblem.Common.Models;
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

        public static bool DeleteFromFile<T>(int id, string filePath)
        {
            var items = ReadFromFile<T>(filePath);
            Type item = typeof(T);
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            if (items != null && item == typeof(Ability))
            {
                List<Ability> abilities = JsonSerializer.Deserialize<List<Ability>>(items);
                var ability = abilities?.Find(item => item.Id == id);
                if (ability != null)
                {
                    var delete = abilities?.Remove(ability);
                    var jsonString = JsonSerializer.Serialize(abilities, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && item == typeof(UnitClass))
            {
                List<UnitClass> classes = JsonSerializer.Deserialize<List<UnitClass>>(items);
                var unitClass = classes?.Find(item => item.Id == id);
                if (unitClass != null)
                {
                    var delete = classes?.Remove(unitClass);
                    var jsonString = JsonSerializer.Serialize(classes, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && item == typeof(Equipment))
            {
                List<Equipment> equipmentList = JsonSerializer.Deserialize<List<Equipment>>(items);
                var equipment = equipmentList?.Find(item => item.Id == id);
                if (equipment != null)
                {
                    var delete = equipmentList?.Remove(equipment);
                    var jsonString = JsonSerializer.Serialize(equipmentList, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && item == typeof(PersonalAbility))
            {
                List<PersonalAbility> personalAbilities = JsonSerializer.Deserialize<List<PersonalAbility>>(items);
                var personalAbility = personalAbilities?.Find(item => item.Id == id);
                if (personalAbility != null)
                {
                    var delete = personalAbilities?.Remove(personalAbility);
                    var jsonString = JsonSerializer.Serialize(personalAbilities, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && item == typeof(Character))
            {
                List<Character> characters = JsonSerializer.Deserialize<List<Character>>(items);
                var character = characters?.Find(item => item.Id == id);
                if (character != null)
                {
                    var delete = characters?.Remove(character);
                    var jsonString = JsonSerializer.Serialize(characters, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            return false;
        }

        public static bool UpdateFile<T>(T item, int id, string filePath)
        {
            var items = ReadFromFile<T>(filePath);
            Type itemType = typeof(T);
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            if (items != null && itemType == typeof(Ability))
            {
                List<Ability> abilities = JsonSerializer.Deserialize<List<Ability>>(items);
                var ability = abilities?.Find(item => item.Id == id);
                if (ability != null)
                {
                    var delete = abilities?.Remove(ability);
                    var jsonString = JsonSerializer.Serialize(abilities, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && itemType == typeof(UnitClass))
            {
                List<UnitClass> classes = JsonSerializer.Deserialize<List<UnitClass>>(items);
                var unitClass = classes?.Find(item => item.Id == id);
                if (unitClass != null)
                {
                    var delete = classes?.Remove(unitClass);
                    var jsonString = JsonSerializer.Serialize(classes, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && itemType == typeof(Equipment))
            {
                List<Equipment> equipmentList = JsonSerializer.Deserialize<List<Equipment>>(items);
                var equipment = equipmentList?.Find(item => item.Id == id);
                if (equipment != null)
                {
                    var delete = equipmentList?.Remove(equipment);
                    var jsonString = JsonSerializer.Serialize(equipmentList, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && itemType == typeof(PersonalAbility))
            {
                List<PersonalAbility> personalAbilities = JsonSerializer.Deserialize<List<PersonalAbility>>(items);
                var personalAbility = personalAbilities?.Find(item => item.Id == id);
                if (personalAbility != null)
                {
                    var delete = personalAbilities?.Remove(personalAbility);
                    var jsonString = JsonSerializer.Serialize(personalAbilities, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            else if (items != null && itemType == typeof(Character))
            {
                List<Character> characters = JsonSerializer.Deserialize<List<Character>>(items);
                var character = characters?.Find(c => c.Id == id);          
                if (character != null)
                {
                    int i = characters.IndexOf(character);
                    character = item as Character;
                    characters[i] = character;
                    var jsonString = JsonSerializer.Serialize(characters, options);
                    System.IO.File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            return false;
        }
    }
}
