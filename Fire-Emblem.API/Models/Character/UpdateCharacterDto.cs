using Fire_Emblem.Common.Models;

namespace Fire_Emblem.API.Models.Character
{
  public class UpdateCharacterDto
  {
    public int Gold { get; set; } = 0;
    public int ExperiencePoints { get; set; } = 0;
    public List<LevelUp>? ManualLevelUps { get; set; } = null;
  }
}
