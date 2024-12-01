#if UNITY_EDITOR
using System.IO;
using Saferio.Prototype.ColoringBook;
using UnityEditor;
using UnityEngine;

public class ImageSegmentationTool : EditorWindow
{
    string _savePath = "Assets/SavedImages";
    private Sprite _targetSprite;

    [MenuItem("Tools/Saferio/Image Segmentation Tool")]
    private static void ShowWindow()
    {
        var window = GetWindow<ImageSegmentationTool>();
        window.titleContent = new GUIContent("Image Segmentation Tool");
        window.Show();
    }

    private void OnGUI()
    {
        _savePath = EditorGUILayout.TextField("Save Path", _savePath);

        _targetSprite = (Sprite)EditorGUILayout.ObjectField(_targetSprite, typeof(Sprite));

        if (GUILayout.Button("Generate Outline Sprite"))
        {
            GenerateOutlineSprite();
        }
    }

    private void GenerateOutlineSprite()
    {
        Sprite outlinedSprite = SaferioImageProcessingUtil.GenerateOutlineSprite(_targetSprite);

        SaveSpriteToFile(outlinedSprite, _savePath, $"Outlined - {_targetSprite.name}.png");
    }

    public async void SaveSpriteToFile(Sprite sprite, string savePath, string spriteName)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Texture2D texture = SpriteToTexture2D(sprite);

        byte[] pngBytes = texture.EncodeToPNG();
        string filePath = Path.Combine(savePath, spriteName);

        await File.WriteAllBytesAsync(filePath, pngBytes);






        TextureImporter textureImporter = AssetImporter.GetAtPath(savePath) as TextureImporter;

        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;

            AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);
        }
    }

    private Texture2D SpriteToTexture2D(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        texture.SetPixels(sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height));
        texture.Apply();
        return texture;
    }
}
#endif
