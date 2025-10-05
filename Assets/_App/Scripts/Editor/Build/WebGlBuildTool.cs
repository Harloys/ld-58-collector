using UnityEditor;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System;

public class WebGLBuildTool
{
    [MenuItem("Demetori/Build/WebGL Build & Zip")]
    public static void BuildWebGLAndZip()
    {
        // Get project name
        string projectName = Application.productName;

        // Format date & time
        string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        // Set paths
        string buildFolder = Path.Combine(Application.dataPath, "../Build/WebGL");
        string zipFolder = Path.Combine(Application.dataPath, "../Build");
        string zipFilePath = Path.Combine(zipFolder, $"{projectName}_{dateTime}.zip");

        // Ensure build directory exists
        if (!Directory.Exists(buildFolder))
            Directory.CreateDirectory(buildFolder);

        // Build player
        string[] scenes = GetEnabledScenes();
        BuildPipeline.BuildPlayer(scenes, buildFolder, BuildTarget.WebGL, BuildOptions.None);

        // Create zip
        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);

        ZipFile.CreateFromDirectory(buildFolder, zipFilePath);

        // Open folder in Explorer/Finder
        EditorUtility.RevealInFinder(zipFilePath);

        Debug.Log($"✅ WebGL Build completed and archived at: {zipFilePath}");
    }

    [MenuItem("Demetori/Build/Open Itch.io Edit Page")]
    public static void OpenItchIoEditPage()
    {
        Application.OpenURL("https://itch.io/game/edit/3799058");
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}