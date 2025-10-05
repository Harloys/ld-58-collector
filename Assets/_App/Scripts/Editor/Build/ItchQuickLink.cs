using UnityEditor;
using UnityEngine;

public static class ItchQuickLink
{
    [MenuItem("Demetori/Build/Open Game Page on itch.io")]
    private static void OpenItchGamePage()
    {
        const string url = "https://d-demetori.itch.io/ld-58-collector";
        Application.OpenURL(url);
    }
}