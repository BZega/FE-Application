using Fire_Emblem.API.Business.Helper.FileReader;
using Fire_Emblem.API.Business.Repository.Equips;
using Fire_Emblem.API.Models.Character;
using Fire_Emblem.Common.Models;
using Fire_Emblem.Common.TypeCodes;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Fire_Emblem.API.Business.Repository.Characters
{
  public class CharactersRepository : ICharactersRepository
  {
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/character.txt");
    private readonly string _characterFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/Characters");
    private readonly string _convoyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/convoy.txt");
    private readonly string _supportFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/support.txt");
    private readonly string _enemyFilePath = Path.Combine(Directory.GetCurrentDirectory(), "DataStore/enemy.txt");
    private readonly IEquipmentRepository _equipmentRepository;
    public CharactersRepository(IEquipmentRepository equipmentRepository)
    {
      _equipmentRepository = equipmentRepository;
    }

    public async Task<bool> AddNewCharacter(Character character)
    {
      try
      {
        if (character == null)
          return false;

        // ensure directory exists
        if (!Directory.Exists(_characterFilePath))
          Directory.CreateDirectory(_characterFilePath);

        // build per-character file path: DataStore/Characters/{id}.txt
        var filePath = Path.Combine(_characterFilePath, $"{character.Id}.txt");

        // write the single-document file atomically
        await FileHelper.WriteNewCharacter(character, filePath);

        var characterIndex = new CharacterIndex
        {
          Id = character.Id,
          Name = character.Biography.Name
        };

        await FileHelper.WriteToFileAsync(characterIndex, _filePath);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<List<Character>> GetAllCharacters()
    {
      try
      {
        // Read character indexes from character.txt
        var indexJson = await FileHelper.ReadFromFileAsync<CharacterIndex>(_filePath);
        var characterIndexes = JsonSerializer.Deserialize<List<CharacterIndex>>(indexJson);
        
        if (characterIndexes == null || characterIndexes.Count == 0)
          return new List<Character>();

        var characters = new List<Character>();

        // Load each character from their individual file
        foreach (var index in characterIndexes)
        {
          try
          {
            var individualFilePath = Path.Combine(_characterFilePath, $"{index.Id}.txt");
            
            if (File.Exists(individualFilePath))
            {
              var characterJson = await File.ReadAllTextAsync(individualFilePath);
              var character = JsonSerializer.Deserialize<Character>(characterJson);
              
              if (character != null)
              {
                characters.Add(character);
              }
            }
          }
          catch (Exception)
          {
            // Continue loading other characters even if one fails
            continue;
          }
        }

        return characters;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Character> GetCharacter(string id)
    {
      try
      {
        // Read directly from individual character file
        var individualFilePath = Path.Combine(_characterFilePath, $"{id}.txt");
        
        if (!File.Exists(individualFilePath))
          return null;

        var characterJson = await File.ReadAllTextAsync(individualFilePath);
        var character = JsonSerializer.Deserialize<Character>(characterJson);
        
        return character;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateCharacter(Character character)
    {
      try
      {
        if (character == null)
          return false;

        // Ensure characters directory exists
        if (!Directory.Exists(_characterFilePath))
          Directory.CreateDirectory(_characterFilePath);

        // Write/overwrite the per-character file
        var individualFilePath = Path.Combine(_characterFilePath, $"{character.Id}.txt");
        var options = new JsonSerializerOptions
        {
          WriteIndented = true,
          Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var characterJson = JsonSerializer.Serialize(character, options);
        await File.WriteAllTextAsync(individualFilePath, characterJson);

        // Ensure character index (character.txt) contains an entry for this character
        var indexJson = await FileHelper.ReadFromFileAsync<CharacterIndex>(_filePath);
        var characterIndexes = string.IsNullOrWhiteSpace(indexJson)
          ? new List<CharacterIndex>()
          : JsonSerializer.Deserialize<List<CharacterIndex>>(indexJson) ?? new List<CharacterIndex>();

        var existingIndex = characterIndexes.FirstOrDefault(ci => ci.Id == character.Id);
        if (existingIndex != null)
        {
          existingIndex.Name = character.Biography?.Name ?? existingIndex.Name;
        }
        else
        {
          characterIndexes.Add(new CharacterIndex { Id = character.Id, Name = character.Biography?.Name ?? string.Empty });
        }

        var indexesJson = JsonSerializer.Serialize(characterIndexes, options);
        await File.WriteAllTextAsync(_filePath, indexesJson);

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> RemoveCharacterById(string id, bool shouldDeleteConvoy)
    {
      try
      {
        var character = await GetCharacter(id);
        if (character == null)
          return false;

        // Delete individual character file
        var individualFilePath = Path.Combine(_characterFilePath, $"{id}.txt");
        if (File.Exists(individualFilePath))
        {
          File.Delete(individualFilePath);
        }

        // Remove from character index
        await FileHelper.DeleteFromFileAsync<CharacterIndex>(id, _filePath);

        if (shouldDeleteConvoy)
        {
          await FileHelper.DeleteFromFileAsync<Convoy>(character.ConvoyId, _convoyFilePath);
        }
        
        if (character.Supports != null)
        {
          foreach (var support in character.Supports)
          {
            await FileHelper.DeleteFromFileAsync<Support>(support.Id, _supportFilePath);
          }
        }

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> AddNewConvoy(Convoy convoy)
    {
      try
      {
        if (convoy == null)
        {
          return false;
        }
        else
        {
          FileHelper.WriteToFileAsync(convoy, _convoyFilePath);
          return true;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<List<Convoy>> GetAllConvoys()
    {
      try
      {
        var convoyFile = await FileHelper.ReadFromFileAsync<Convoy>(_convoyFilePath);
        var convoys = JsonSerializer.Deserialize<List<Convoy>>(convoyFile);
        return convoys;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Inventory> GetConvoyInventory(string id)
    {
      try
      {
        var convoys = await GetAllConvoys();
        var convoy = convoys.Find(convoy => convoy.Id == id);
        if (convoy != null)
        {
          return convoy.ConvoyItems;
        }
        else
        {
          return null;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Convoy> GetConvoyById(string id)
    {
      try
      {
        var convoys = await GetAllConvoys();
        var convoy = convoys.Find(convoy => convoy.Id == id);
        if (convoy != null)
        {
          return convoy;
        }
        else
        {
          return null;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateConvoy(Convoy convoy)
    {
      try
      {
        var result = await FileHelper.UpdateFileAsync(convoy, convoy.Id.ToString(), _convoyFilePath);
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> AddNewSupport(Support support)
    {
      try
      {
        if (support == null)
        {
          return false;
        }
        else
        {
          await FileHelper.WriteToFileAsync(support, _supportFilePath);
          return true;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<List<Support>> GetAllSupports()
    {
      try
      {
        var supportFile = await FileHelper.ReadFromFileAsync<Support>(_supportFilePath);
        var supports = JsonSerializer.Deserialize<List<Support>>(supportFile);
        return supports;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Support> GetSupportById(string id)
    {
      try
      {
        var supports = await GetAllSupports();
        var support = supports.Find(support => support.Id == id);
        if (support != null)
        {
          return support;
        }
        else
        {
          return null;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateSupport(Support support)
    {
      try
      {
        var result = await FileHelper.UpdateFileAsync(support, support.Id, _supportFilePath);
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<Enemy> AddNewEnemy(Enemy enemy)
    {
      try
      {
        if (enemy == null)
        {
          return null;
        }
        else
        {
          await FileHelper.WriteToFileAsync(enemy, _enemyFilePath);
          return enemy;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<List<Enemy>> GetAllEnemies()
    {
      try
      {
        var enemyFile = await FileHelper.ReadFromFileAsync<Enemy>(_enemyFilePath);
        var enemies = JsonSerializer.Deserialize<List<Enemy>>(enemyFile);
        return enemies;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<Enemy> GetEnemyById(int id)
    {
      try
      {
        var enemies = await GetAllEnemies();
        var enemy = enemies.Find(enemy => enemy.Id == id);
        if (enemy != null)
        {
          return enemy;
        }
        else
        {
          return null;
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public async Task<bool> UpdateEnemy(Enemy enemy)
    {
      try
      {
        var result = await FileHelper.UpdateFileAsync(enemy, enemy.Id.ToString(), _enemyFilePath);
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<bool> RemoveEnemy(int enemyId)
    {
      try
      {
        var result = await FileHelper.DeleteFromFileAsync<Enemy>(enemyId.ToString(), _enemyFilePath);
        return result;
      }
      catch (Exception)
      {
        return false;
      }
    }

    public async Task<Tuple<bool, int>> ConvertCharacterFileToIndividualFiles()
    {
      try
      {
        var characterIndexes = new List<CharacterIndex>();
        // Check if character.txt exists
        if (!File.Exists(_filePath))
          return new Tuple<bool, int>(false, 0);

        // Read all characters from character.txt
        var jsonString = await File.ReadAllTextAsync(_filePath);
        var characters = JsonSerializer.Deserialize<List<Character>>(jsonString);

        if (characters == null || characters.Count == 0)
          return new Tuple<bool, int>(false, 0);

        // Ensure Characters directory exists
        if (!Directory.Exists(_characterFilePath))
          Directory.CreateDirectory(_characterFilePath);

        int successCount = 0;


        // Write each character to individual file
        foreach (var character in characters)
        {
          try
          {
            character.Id = GenerateCharacterOid(characters);
            var individualFilePath = Path.Combine(_characterFilePath, $"{character.Id}.txt");
            
            var options = new JsonSerializerOptions 
            { 
              WriteIndented = true, 
              Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
            };
            
            var characterJson = JsonSerializer.Serialize(character, options);
            await File.WriteAllTextAsync(individualFilePath, characterJson);
            var characterIndex = new CharacterIndex
            {
              Id = character.Id,
              Name = character.Biography.Name
            };
            
            characterIndexes.Add(characterIndex);
            successCount++;
          }
          catch (Exception)
          {
            // Continue with next character even if one fails
            continue;
          }
        }

        // Create backup of original file
        var backupPath = _filePath + ".backup";
        File.Copy(_filePath, backupPath, true);
        // Overwrite character.txt with new indexes
        var indexesJson = JsonSerializer.Serialize(characterIndexes, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, indexesJson);

        return new Tuple<bool, int>(successCount > 0, successCount);
      }
      catch (Exception)
      {
        return new Tuple<bool, int>(false, 0);
      }
    }

    private static string GenerateCharacterOid(List<Character> existing)
    {
      const string prefix = "CHA";
      const int width = 8;

      var maxNum = 0;
      if (existing != null)
      {
        foreach (var c in existing)
        {
          if (string.IsNullOrWhiteSpace(c?.Id))
            continue;

          var m = Regex.Match(c.Id, @"^CHA0*(\d+)$", RegexOptions.IgnoreCase);
          if (m.Success && int.TryParse(m.Groups[1].Value, out var n))
          {
            if (n > maxNum) maxNum = n;
          }
        }
      }

      var next = maxNum + 1;
      return $"{prefix}{next.ToString().PadLeft(width, '0')}";
    }
  }
}
