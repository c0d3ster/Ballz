using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

public class IconGenerator : EditorWindow
{
  private GameObject targetObject;
  private string savePath = "Assets/Sprites";
  private string fileName = "PlayerIcon";
  private int resolution = 256;
  private Color backgroundColor = Color.clear;

  [MenuItem("Tools/Generate Icon from 3D Object")]
  public static void ShowWindow()
  {
    GetWindow<IconGenerator>("Icon Generator");
  }

  void OnGUI()
  {
    GUILayout.Label("Generate Icon from 3D Object", EditorStyles.boldLabel);

    targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
    fileName = EditorGUILayout.TextField("File Name", fileName);
    resolution = EditorGUILayout.IntField("Resolution", resolution);
    backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);

    EditorGUILayout.Space();
    EditorGUILayout.HelpBox($"Icon will be saved to: {savePath}/{fileName}.png", MessageType.Info);

    if (GUILayout.Button("Generate Icon"))
    {
      if (targetObject != null)
      {
        GenerateIcon();
      }
      else
      {
        EditorUtility.DisplayDialog("Error", "Please select a target object", "OK");
      }
    }
  }

  void GenerateIcon()
  {
    // Store the current scene
    var currentScene = EditorSceneManager.GetActiveScene();

    try
    {
      // Create a temporary scene for rendering
      var tempScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

      // Create a temporary camera
      GameObject cameraObject = new GameObject("IconCamera");
      Camera camera = cameraObject.AddComponent<Camera>();
      camera.clearFlags = CameraClearFlags.SolidColor;
      camera.backgroundColor = backgroundColor;
      camera.orthographic = true;
      camera.orthographicSize = 0.7f;
      camera.nearClipPlane = 0.1f;
      camera.farClipPlane = 100f;

      // Ensure proper transparency settings
      camera.allowHDR = false;
      camera.allowMSAA = false;
      camera.transparencySortMode = TransparencySortMode.Default;
      camera.transparencySortAxis = new Vector3(0, 0, 1);

      // Position camera to look at the object
      cameraObject.transform.position = new Vector3(0, 0, -5);
      cameraObject.transform.LookAt(Vector3.zero);

      // Add a directional light
      GameObject lightObject = new GameObject("Directional Light");
      Light light = lightObject.AddComponent<Light>();
      light.type = LightType.Directional;
      light.intensity = 1f;
      lightObject.transform.rotation = Quaternion.Euler(50, -30, 0);

      // Create a temporary object to render
      GameObject tempObject = null;
      if (targetObject != null)
      {
        Debug.Log($"Attempting to instantiate target object: {targetObject.name}");

        // Try to instantiate as prefab first
        if (PrefabUtility.IsPartOfPrefabAsset(targetObject))
        {
          Debug.Log("Target is a prefab asset, using PrefabUtility.InstantiatePrefab");
          tempObject = PrefabUtility.InstantiatePrefab(targetObject) as GameObject;
        }
        else
        {
          Debug.Log("Target is not a prefab asset, using regular Instantiate");
          tempObject = Instantiate(targetObject);
        }

        if (tempObject == null)
        {
          Debug.LogError($"Failed to instantiate target object. Object type: {targetObject.GetType()}, IsPrefab: {PrefabUtility.IsPartOfPrefabAsset(targetObject)}");
          throw new System.Exception("Failed to instantiate target object - instantiation returned null");
        }

        // Remove the material settings section that was forcing opaque mode
        tempObject.transform.position = Vector3.zero;
        tempObject.transform.rotation = Quaternion.identity;
      }
      else
      {
        Debug.LogError("Target object is null");
        throw new System.Exception("Target object is null");
      }

      // Create a temporary render texture with alpha channel
      RenderTexture renderTexture = new RenderTexture(resolution, resolution, 32, RenderTextureFormat.ARGB32);
      renderTexture.antiAliasing = 1;
      camera.targetTexture = renderTexture;

      // Render the object
      camera.Render();

      // Create a new texture and read the render texture into it
      Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
      RenderTexture.active = renderTexture;
      texture.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
      texture.Apply();

      // Debug: Check if the texture has any transparent pixels
      Color[] pixels = texture.GetPixels();
      bool hasTransparency = false;
      foreach (Color pixel in pixels)
      {
        if (pixel.a < 1.0f)
        {
          hasTransparency = true;
          break;
        }
      }
      Debug.Log($"Texture has transparency: {hasTransparency}");

      // Clean up render texture and camera
      RenderTexture.active = null;
      camera.targetTexture = null;
      DestroyImmediate(renderTexture);
      DestroyImmediate(cameraObject);
      DestroyImmediate(tempObject);
      DestroyImmediate(lightObject);

      // Save the texture as a PNG
      if (!Directory.Exists(savePath))
      {
        Directory.CreateDirectory(savePath);
      }

      // Ensure we're using the correct PNG encoding
      byte[] bytes = texture.EncodeToPNG();
      string fullPath = Path.Combine(savePath, fileName + ".png");
      File.WriteAllBytes(fullPath, bytes);
      AssetDatabase.Refresh();

      // Set the texture import settings
      string assetPath = fullPath.Replace(Application.dataPath, "Assets");
      TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
      if (importer != null)
      {
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;

        // Set platform-specific settings for Android
        TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
        androidSettings.name = "Android";
        androidSettings.maxTextureSize = 2048;
        androidSettings.format = TextureImporterFormat.RGBA32;
        androidSettings.overridden = true;
        androidSettings.allowsAlphaSplitting = false;
        importer.SetPlatformTextureSettings(androidSettings);

        // Also set default settings to ensure consistency
        TextureImporterPlatformSettings defaultSettings = new TextureImporterPlatformSettings();
        defaultSettings.name = "DefaultTexturePlatform";
        defaultSettings.maxTextureSize = 2048;
        defaultSettings.format = TextureImporterFormat.RGBA32;
        defaultSettings.overridden = true;
        defaultSettings.allowsAlphaSplitting = false;
        importer.SetPlatformTextureSettings(defaultSettings);

        importer.SaveAndReimport();

        // Debug: Verify the settings were applied
        Debug.Log($"Texture import settings applied - Alpha is transparency: {importer.alphaIsTransparency}");
        Debug.Log($"Android format: {importer.GetPlatformTextureSettings("Android").format}");
      }

      EditorUtility.DisplayDialog("Success", $"Icon generated successfully!\nSaved to: {assetPath}", "OK");
    }
    catch (System.Exception e)
    {
      Debug.LogError($"Error generating icon: {e.Message}");
      EditorUtility.DisplayDialog("Error", $"Failed to generate icon: {e.Message}", "OK");
    }
    finally
    {
      // Always restore the original scene
      if (currentScene.IsValid())
      {
        EditorSceneManager.OpenScene(currentScene.path);
      }
    }
  }
}