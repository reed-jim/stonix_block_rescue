#if UNITY_EDITOR
using System.IO;
using System.Linq;
using Saferio.Prototype.ColoringBook;
using UnityEditor;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class LevelGeneratorTool : EditorWindow
    {
        string _savePath;
        private Sprite _targetSprite;
        ImageSegmenter _imageSegmenter;
        [SerializeField] private CurrentLevelData currentLevelData;


        [MenuItem("Tools/Saferio/Prototype/Coloring Book/Level Generator Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<LevelGeneratorTool>();
            window.titleContent = new GUIContent("Level Generator Tool");
            window.Show();
        }

        private void OnGUI()
        {
            _savePath = EditorGUILayout.TextField("Level Prefab Save Path", _savePath);

            _targetSprite = (Sprite)EditorGUILayout.ObjectField(_targetSprite, typeof(Sprite));

            _imageSegmenter = (ImageSegmenter)EditorGUILayout.ObjectField(_imageSegmenter, typeof(ImageSegmenter));

            if (GUILayout.Button("Generate Level"))
            {
                GenerateLevel();
            }

            if (GUILayout.Button("Save"))
            {
                SaveLevel();
            }

            if (GUILayout.Button("Test"))
            {
                Test();
            }
        }

        private void GenerateLevel()
        {
            GameObject level = AssetDatabase.LoadAssetAtPath<GameObject>(_savePath);

            currentLevelData.Level = _savePath.Last();

            _imageSegmenter.ProcessImageAndSegment(_targetSprite);

            PrefabUtility.SaveAsPrefabAsset(level, _savePath);

            EditorUtility.SetDirty(level);

            // AssetDatabase.SaveAssets();
            // EditorUtility.FocusProjectWindow();
            // AssetDatabase.Refresh();
        }

        private void SaveLevel()
        {
            GameObject level = AssetDatabase.LoadAssetAtPath<GameObject>(_savePath);

            PrefabUtility.SaveAsPrefabAsset(level, _savePath);
        }

        private void Test()
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(_savePath) as TextureImporter;

            Debug.Log(textureImporter);
        }
    }
}
#endif
