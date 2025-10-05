using UnityEditor;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class ItchPublisher
{
    private const string BUTLER_PATH = @"C:\Butler\butler.exe";
    private const string ITCH_TARGET = "d-demetori/ld-58-collector:webgl";

    [MenuItem("Demetori/Build/Publish to Itch.io Async with Logs")]
    public static void BuildAndPublishAsync()
    {
        string buildFolder = Path.Combine(Application.dataPath, "../Build/WebGL");

        if (!System.IO.Directory.Exists(buildFolder))
            System.IO.Directory.CreateDirectory(buildFolder);

        string[] scenes = GetEnabledScenes();
        BuildPipeline.BuildPlayer(scenes, buildFolder, BuildTarget.WebGL, BuildOptions.None);

        // Fire-and-forget async upload, logs visible in Console
        UploadWithButlerAsync(buildFolder);

        //EditorUtility.RevealInFinder(buildFolder);
        Debug.Log("✅ Build completed; started async upload with live logs.");
    }

    private static async void UploadWithButlerAsync(string folderPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = BUTLER_PATH,
            Arguments = $"push \"{folderPath}\" {ITCH_TARGET}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8
        };

        using (var process = new Process() { StartInfo = startInfo, EnableRaisingEvents = true })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.Log($"[Butler] {e.Data}");
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    Debug.LogError($"[Butler ERROR] {e.Data}");
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());
            Debug.Log("[Butler] Upload finished.");
        }
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
            if (scene.enabled) scenes.Add(scene.path);
        return scenes.ToArray();
    }
}
