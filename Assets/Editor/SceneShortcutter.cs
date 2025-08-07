using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

public class SceneShortcutter : EditorWindow
{
    [MenuItem("Tools/Scene Shortcutter")]
    public static void ShowWindow()
    {
        GetWindow<SceneShortcutter>("Scene Shortcutter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Scenes in Build Settings", EditorStyles.boldLabel);

        if (EditorBuildSettings.scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in Build Settings!", MessageType.Warning);
            return;
        }

        foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
        {
            if (GUILayout.Button(System.IO.Path.GetFileNameWithoutExtension(scene.path)))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scene.path);
                }
            }
        }
    }
}