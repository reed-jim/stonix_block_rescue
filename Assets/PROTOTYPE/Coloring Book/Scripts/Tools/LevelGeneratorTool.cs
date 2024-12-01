#if UNITY_EDITOR
using System.IO;
using Saferio.Prototype.ColoringBook;
using UnityEditor;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class LevelGeneratorTool : EditorWindow
    {
        string _savePath = "Assets/SavedImages";
        private Sprite _targetSprite;

        ImageSegmenter _imageSegmenter;


        [MenuItem("Tools/Saferio/Prototype/Coloring Book/Level Generator Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<LevelGeneratorTool>();
            window.titleContent = new GUIContent("Level Generator Tool");
            window.Show();
        }

        private void OnGUI()
        {
            _savePath = EditorGUILayout.TextField("Save Path", _savePath);

            _targetSprite = (Sprite)EditorGUILayout.ObjectField(_targetSprite, typeof(Sprite));

            _imageSegmenter = (ImageSegmenter)EditorGUILayout.ObjectField(_imageSegmenter, typeof(ImageSegmenter));

            if (GUILayout.Button("Generate Level"))
            {
                GenerateLevel();
            }
        }

        private void GenerateLevel()
        {
            GameObject level = AssetDatabase.LoadAssetAtPath<GameObject>(_savePath);

            _imageSegmenter.ProcessImageAndSegment(_targetSprite);

            PrefabUtility.SavePrefabAsset(level);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Prefab Created", $"Prefab created at {_savePath}", "OK");
        }
    }
}
#endif
