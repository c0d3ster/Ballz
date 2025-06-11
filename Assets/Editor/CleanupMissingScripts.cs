using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class CleanupMissingScripts : EditorWindow
{
  [MenuItem("Tools/Cleanup/Remove All Missing Scripts")]
  static void CleanupAllMissingScripts()
  {
    // Get all scene paths in the project
    string[] scenePaths = AssetDatabase.FindAssets("t:Scene")
        .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
        .ToArray();

    int totalScenes = scenePaths.Length;
    int processedScenes = 0;
    int totalGameObjectsCleaned = 0;
    int totalScriptsCleaned = 0;

    // Process each scene
    foreach (string scenePath in scenePaths)
    {
      EditorUtility.DisplayProgressBar("Cleaning Missing Scripts",
          $"Processing scene {processedScenes + 1} of {totalScenes}: {System.IO.Path.GetFileName(scenePath)}",
          (float)processedScenes / totalScenes);

      // Load the scene
      EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
      bool sceneModified = false;

      // Get all GameObjects in the scene, including inactive ones
      GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>()
          .Where(go => go.scene.path == scenePath)
          .ToArray();

      foreach (GameObject obj in allObjects)
      {
        // Get all components
        Component[] components = obj.GetComponents<Component>();
        int nullComponents = 0;

        // Count missing scripts
        foreach (Component component in components)
        {
          if (component == null)
            nullComponents++;
        }

        // Remove missing scripts if any found
        if (nullComponents > 0)
        {
          Debug.Log($"Removing {nullComponents} missing scripts from '{obj.name}' in {scenePath}");
          GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
          totalGameObjectsCleaned++;
          totalScriptsCleaned += nullComponents;
          sceneModified = true;
        }
      }

      // Save scene if modified
      if (sceneModified)
      {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
      }

      processedScenes++;
    }

    EditorUtility.ClearProgressBar();
    Debug.Log($"Cleanup complete!\n" +
             $"Processed {processedScenes} scenes\n" +
             $"Cleaned {totalGameObjectsCleaned} GameObjects\n" +
             $"Removed {totalScriptsCleaned} missing scripts");

    // Refresh the editor
    AssetDatabase.Refresh();
  }

  [MenuItem("Tools/Cleanup/Find Missing Scripts in Selection")]
  static void FindMissingScriptsInSelection()
  {
    GameObject[] selection = Selection.gameObjects;
    int totalGameObjects = 0;
    int totalMissingScripts = 0;

    foreach (GameObject obj in selection)
    {
      Component[] components = obj.GetComponents<Component>();
      int missingScripts = components.Count(component => component == null);

      if (missingScripts > 0)
      {
        Debug.Log($"Found {missingScripts} missing scripts on '{obj.name}'", obj);
        totalMissingScripts += missingScripts;
        totalGameObjects++;
      }
    }

    if (totalMissingScripts > 0)
      Debug.Log($"Found {totalMissingScripts} missing scripts on {totalGameObjects} GameObjects in selection");
    else
      Debug.Log("No missing scripts found in selection");
  }
}