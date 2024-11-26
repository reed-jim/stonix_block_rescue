#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SaferioSceneOpener : EditorWindow
{
    [MenuItem("Tools/Open Scene/Menu")]
    public static void OpenMenuScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
    }

    [MenuItem("Tools/Open Scene/Gameplay")]
    public static void OpenGameplayScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
    }
}
#endif
