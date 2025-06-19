using System.Collections.Generic;

public static class SceneTimerConfig
{
  // Centralized configuration for scene time limits
  // Add new scenes here to configure their time limits
  public static readonly Dictionary<string, float> SceneTimeLimits = new Dictionary<string, float>
  {
    // Collector Series
    { "Ball Collector 1", 15f },
    { "Ball Collector 2", 45f },
    
    // Balancer Series  
    { "Ball Balancer 1", 10f },
    
    // Dodger Series
    { "Ball Dodger 1", 10f },
    { "Ball Dodger 2", 10f },
    
    // Jumper Series
    { "Ball Jumper 1", 10f },
    
    // Pusher Series
    { "Ball Pusher 1", 30f },
    
    // Test Scenes
    { "Ball Tester", 50f }
  };

  // Helper methods for easy configuration
  public static void AddSceneTimeLimit(string sceneName, float timeLimit)
  {
    SceneTimeLimits[sceneName] = timeLimit;
  }

  public static void RemoveSceneTimeLimit(string sceneName)
  {
    if (SceneTimeLimits.ContainsKey(sceneName))
    {
      SceneTimeLimits.Remove(sceneName);
    }
  }

  public static float GetTimeLimit(string sceneName)
  {
    return SceneTimeLimits.TryGetValue(sceneName, out float timeLimit) ? timeLimit : 0f;
  }

  public static bool HasTimeLimit(string sceneName)
  {
    return SceneTimeLimits.ContainsKey(sceneName);
  }

  // Batch configuration methods for series
  public static void ConfigureCollectorSeries()
  {
    AddSceneTimeLimit("Ball Collector 1", 15f);
    AddSceneTimeLimit("Ball Collector 2", 45f);
    // Add more collector levels here as they're created
  }

  public static void ConfigureBalancerSeries()
  {
    AddSceneTimeLimit("Ball Balancer 1", 10f);
    // Add more balancer levels here as they're created
  }

  public static void ConfigureDodgerSeries()
  {
    AddSceneTimeLimit("Ball Dodger 1", 10f);
    AddSceneTimeLimit("Ball Dodger 2", 10f);
    // Add more dodger levels here as they're created
  }

  public static void ConfigureJumperSeries()
  {
    AddSceneTimeLimit("Ball Jumper 1", 10f);
    // Add more jumper levels here as they're created
  }

  public static void ConfigurePusherSeries()
  {
    AddSceneTimeLimit("Ball Pusher 1", 30f);
    // Add more pusher levels here as they're created
  }
}