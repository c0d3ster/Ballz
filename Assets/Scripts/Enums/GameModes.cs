using UnityEngine;

namespace Enums
{
  public enum GameMode
  {
    [SceneSuffix("or")]
    Collect,
    [SceneSuffix("r")]
    Balance,
    [SceneSuffix("r")]
    Dodge,
    [SceneSuffix("er")]
    Jump,
    [SceneSuffix("er")]
    Push
  }

  [System.AttributeUsage(System.AttributeTargets.Field)]
  public class SceneSuffixAttribute : System.Attribute
  {
    public string Suffix { get; private set; }

    public SceneSuffixAttribute(string suffix)
    {
      Suffix = suffix;
    }
  }

  public static class GameModeExtensions
  {
    public static string ToString(this GameMode gameMode)
    {
      return gameMode.ToString();
    }
  }
}