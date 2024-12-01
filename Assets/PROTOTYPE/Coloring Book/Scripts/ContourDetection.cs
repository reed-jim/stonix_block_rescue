using UnityEngine;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using Emgu.CV.Util;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Linq;
using System;
using Saferio.Prototype.ColoringBook;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;

public class ImageSegmenter : MonoBehaviour
{
    [SerializeField] private Transform segmentContainer;
    public SpriteRenderer originalSpriteRenderer;
    public GameObject spritePrefab;

    [Header("CUSTOMIZE")]
    [SerializeField] private double thresh;
    [SerializeField] private double threshLinking;
    [SerializeField] private int blurRadius;
    [SerializeField] float minContourArea = 100.0f;

    [SerializeField] private List<UnityEngine.Color> distinctDetectedColors;
    [SerializeField] private List<int> numberSegmentOfDistinctColors;

    #region PRIVATE FIELD
    private Texture2D texture;
    #endregion

    #region ACTION
    public static event Action<UnityEngine.Color> spawnColorButtonEvent;
    public static event Action<UnityEngine.Color> addRegionColorButtonEvent;
    public static event Action<List<SpriteRegion>, ColorGroupData[]> saveSpriteRegionsEvent;
    #endregion      

    private void Awake()
    {
        LevelController.spawnSegmentationSpriteEvent += SpawnSegmentationSprite;

        distinctDetectedColors = new List<UnityEngine.Color>();
        numberSegmentOfDistinctColors = new List<int>();
    }

    private void OnDestroy()
    {
        LevelController.spawnSegmentationSpriteEvent -= SpawnSegmentationSprite;
    }

    void SpawnSegmentationSprite(CurrentLevelData currentLevelData)
    {
        originalSpriteRenderer.sprite = currentLevelData.Sprite;

        texture = originalSpriteRenderer.sprite.texture;

        texture.GetPixels32();

        ProcessImageAndSegment();
    }

    public void ProcessImageAndSegment(Sprite originalSprite)
    {
        originalSpriteRenderer.sprite = originalSprite;

        texture = originalSpriteRenderer.sprite.texture;

        texture.GetPixels32();

        ProcessImageAndSegment();
    }

    void ProcessImageAndSegment()
    {
        Image<Bgr, byte> image;
        VectorOfVectorOfPoint contours;

        FindContours(out image, out contours);

        DrawOutlinedImage(image, contours);

        DrawImageSegments(image, contours);

        originalSpriteRenderer.gameObject.SetActive(false);
    }

    #region CORE
    private void FindContours(out Image<Bgr, byte> image, out VectorOfVectorOfPoint contours)
    {
        image = TextureToImage(originalSpriteRenderer.sprite.texture);

        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

        Image<Gray, byte> edges = grayImage.Canny(thresh, threshLinking);

        CvInvoke.GaussianBlur(edges, edges, new System.Drawing.Size(blurRadius, blurRadius), 0);

        contours = new VectorOfVectorOfPoint();

        Mat hierarchy = new Mat();

        CvInvoke.FindContours(edges, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
    }
    private void DrawOutlinedImage(Image<Bgr, byte> image, VectorOfVectorOfPoint contours)
    {
        Image<Bgr, byte> contourHighlightedImage = new Image<Bgr, byte>(image.Width, image.Height, new Bgr(0, 0, 0));

        for (int i = 0; i < contours.Size; i++)
        {
            CvInvoke.DrawContours(contourHighlightedImage, contours, i, new MCvScalar(225, 225, 225), -1);
        }

        Texture2D contourTexture = ImageToTexture(contourHighlightedImage);

        Sprite contourSprite = Sprite.Create(contourTexture, new Rect(0, 0, contourTexture.width, contourTexture.height), new Vector2(0.5f, 0.5f));

        originalSpriteRenderer.sprite = contourSprite;
    }

    private void DrawImageSegments(Image<Bgr, byte> image, VectorOfVectorOfPoint contours)
    {
        List<SpriteRegion> spriteRegions = new List<SpriteRegion>();

        for (int i = 0; i < contours.Size; i++)
        {
            double contourArea = CvInvoke.ContourArea(contours[i]);

            if (contourArea < minContourArea)
            {
                continue;
            }

            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);

            Image<Bgr, byte> segment = new Image<Bgr, byte>(boundingBox.Width, boundingBox.Height);
            image.ROI = boundingBox;
            image.CopyTo(segment);
            image.ROI = System.Drawing.Rectangle.Empty;

            Texture2D segmentOutlinedTexture = ImageToTexture(segment);
            Texture2D segmentTexture = ImageToTexture(segment);
            Texture2D segmentHighlightTexture = ImageToTexture(segment);

            for (int y = 0; y < segment.Height; y++)
            {
                for (int x = 0; x < segment.Width; x++)
                {
                    System.Drawing.Point segmentPoint = new System.Drawing.Point(x + boundingBox.X, y + boundingBox.Y);

                    double polygonTestValue = CvInvoke.PointPolygonTest(contours[i], segmentPoint, true);

                    bool isInside = polygonTestValue >= 0f;
                    bool isInEdge = polygonTestValue == 0;

                    if (isInside)
                    {
                        Bgr pixel = segment[y, x];
                        segmentTexture.SetPixel(x, y, new UnityEngine.Color((float)pixel.Red / 255f, (float)pixel.Green / 255f, (float)pixel.Blue / 255f, 1f)); // Opaque


                        if (isInEdge)
                        {
                            segmentOutlinedTexture.SetPixel(x, y, new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1f));
                            segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1f));
                        }
                        else
                        {
                            segmentOutlinedTexture.SetPixel(x, y, new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1f));
                            segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0.5f, 0f, 1f));
                        }
                    }
                    else
                    {
                        segmentOutlinedTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                        segmentTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                        segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                    }
                }
            }

            segmentOutlinedTexture.Apply();
            segmentTexture.Apply();
            segmentHighlightTexture.Apply();

            Sprite segmentOutlineSprite = Sprite.Create(segmentOutlinedTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);
            Sprite segmentSprite = Sprite.Create(segmentTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);
            Sprite segmentHighlightSprite = Sprite.Create(segmentHighlightTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);

            segmentOutlinedTexture.name = $"Outline - {texture.name} {i}";
            segmentSprite.name = $"Filled - {texture.name} {i}";
            segmentHighlightSprite.name = $"Highlight - {texture.name} {i}";

            SaveSpriteToFile(segmentOutlineSprite, "Assets/PROTOTYPE/Coloring Book/Sprites/Gallery/Level 1/Outlined", segmentOutlinedTexture.name);
            // SaveSprite(segmentSprite, "Assets/PROTOTYPE/Coloring Book/Sprites/Gallery/Level 1/Outlined", segmentSprite.name);
            // SaveSprite(segmentHighlightSprite, "Assets/PROTOTYPE/Coloring Book/Sprites/Gallery/Level 1/Outlined", segmentHighlightSprite.name);

            GameObject newSegmentObject = Instantiate(spritePrefab, segmentContainer);

            SpriteRegion spriteRegion = newSegmentObject.GetComponent<SpriteRegion>();

            spriteRegion.SegmentIndex = spriteRegions.Count;

            spriteRegions.Add(spriteRegion);

            spriteRegion.Setup(segmentOutlineSprite, segmentSprite, segmentHighlightSprite);

            Vector2 spriteSize = originalSpriteRenderer.sprite.bounds.size;

            newSegmentObject.transform.position = new Vector3(spriteSize.x * ((float)boundingBox.Location.X) / image.Width,
                spriteSize.y * ((float)boundingBox.Location.Y - image.Height) / image.Height, 0);

            newSegmentObject.transform.position -= new Vector3(0.5f * spriteSize.x, -0.5f * spriteSize.y);

            UnityEngine.Color contourDominantColor = GetMostCommonColorInContour(TextureToMat(segmentTexture), contours[i]);

            bool isCloseColor = false;

            if (distinctDetectedColors.Count == 0)
            {
                distinctDetectedColors.Add(contourDominantColor);
                numberSegmentOfDistinctColors.Add(1);
            }
            else
            {
                for (int j = 0; j < distinctDetectedColors.Count; j++)
                {
                    if (AreColorsClose(distinctDetectedColors[j], contourDominantColor))
                    {
                        spriteRegion.ColorGroup = distinctDetectedColors[j];

                        numberSegmentOfDistinctColors[j]++;

                        addRegionColorButtonEvent?.Invoke(distinctDetectedColors[j]);

                        isCloseColor = true;

                        break;
                    }
                }
            }

            if (isCloseColor)
            {
                continue;
            }
            else
            {
                distinctDetectedColors.Add(contourDominantColor);

                numberSegmentOfDistinctColors.Add(1);
            }

            spriteRegion.ColorGroup = contourDominantColor;

            spawnColorButtonEvent?.Invoke(contourDominantColor);
        }

        ColorGroupData[] colorGroupsData = new ColorGroupData[distinctDetectedColors.Count];

        for (int i = 0; i < distinctDetectedColors.Count; i++)
        {
            colorGroupsData[i] = new ColorGroupData();

            colorGroupsData[i].ColorString = distinctDetectedColors[i].ToString();
            colorGroupsData[i].NumberOfRegions = numberSegmentOfDistinctColors[i];
        }

        saveSpriteRegionsEvent?.Invoke(spriteRegions, colorGroupsData);
    }
    #endregion

    #region UTIL
    private Image<Bgr, byte> TextureToImage(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        byte[] imageBytes = new byte[width * height * 3];

        int index = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                UnityEngine.Color color = texture.GetPixel(x, y);

                imageBytes[index++] = (byte)(color.b * 255);
                imageBytes[index++] = (byte)(color.g * 255);
                imageBytes[index++] = (byte)(color.r * 255);
            }
        }

        Image<Bgr, byte> image = new Image<Bgr, byte>(width, height);
        image.Bytes = imageBytes;

        return image;
    }

    private Texture2D ImageToTexture(Image<Gray, byte> grayImage)
    {
        Texture2D texture = new Texture2D(grayImage.Width, grayImage.Height);

        for (int y = 0; y < grayImage.Height; y++)
        {
            for (int x = 0; x < grayImage.Width; x++)
            {
                byte pixelValue = grayImage.Data[y, x, 0];

                UnityEngine.Color color = new UnityEngine.Color(pixelValue / 255f, pixelValue / 255f, pixelValue / 255f);

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    private Texture2D ImageToTexture(Image<Bgr, byte> image)
    {
        Texture2D texture = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false);

        // Copy image data to Texture2D
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Bgr pixel = image[y, x];
                texture.SetPixel(x, y, new UnityEngine.Color((float)pixel.Red / 255f, (float)pixel.Green / 255f, (float)pixel.Blue / 255f, 1f)); // Alpha = 1
            }
        }
        texture.Apply();  // Apply the changes to the texture

        return texture;
    }

    Mat TextureToMat(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        int width = texture.width;
        int height = texture.height;

        byte[] bytes = new byte[width * height * 3];
        int index = 0;

        for (int i = 0; i < pixels.Length; i++)
        {
            bytes[index++] = pixels[i].b;
            bytes[index++] = pixels[i].g;
            bytes[index++] = pixels[i].r;
        }

        Mat mat = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, 3);

        mat.SetTo(bytes);

        return mat;
    }

    Texture2D MatToTexture(Mat mat)
    {
        Texture2D texture = new Texture2D(mat.Width, mat.Height, TextureFormat.RGB24, false);

        byte[] data = new byte[mat.Width * mat.Height * 3];
        Marshal.Copy(mat.DataPointer, data, 0, data.Length);

        texture.LoadRawTextureData(data);
        texture.Apply();

        return texture;
    }
    #endregion

    #region GET DOMINANT COLOR FROM CONTOUR
    public static UnityEngine.Color GetMostCommonColorInContour(Mat image, VectorOfPoint contour)
    {
        Mat mask = new Mat(image.Size, DepthType.Cv8U, 1);
        mask.SetTo(new MCvScalar(0));
        CvInvoke.FillPoly(mask, new VectorOfVectorOfPoint(new VectorOfPoint[] { contour }), new MCvScalar(0));

        Mat roi = new Mat(image.Size, image.Depth, image.NumberOfChannels);

        image.CopyTo(roi, mask);

        byte[,,] roiData = image.ToImage<Bgr, byte>().Data;

        double sumBlue = 0, sumGreen = 0, sumRed = 0;
        int pixelCount = 0;

        Dictionary<UnityEngine.Color, int> colorGroups = new Dictionary<UnityEngine.Color, int>();

        for (int y = 0; y < roi.Height; y++)
        {
            for (int x = 0; x < roi.Width; x++)
            {
                byte blue = roiData[y, x, 0];
                byte green = roiData[y, x, 1];
                byte red = roiData[y, x, 2];

                if (blue == 0 && green == 0 && red == 0)
                {
                    continue;
                }

                UnityEngine.Color colorGroup = new UnityEngine.Color(red / 255f, green / 255f, blue / 255f);

                if (colorGroups.Count == 0)
                {
                    colorGroups.Add(colorGroup, 1);
                }
                else
                {
                    bool isFound = false;

                    foreach (var colorGroupsKey in colorGroups.Keys)
                    {
                        float cachedBlue = colorGroupsKey.b;
                        float cachedGreen = colorGroupsKey.g;
                        float cachedRed = colorGroupsKey.r;

                        float difference = 0;

                        difference += Mathf.Abs((blue / 255f) - cachedBlue);
                        difference += Mathf.Abs((green / 255f) - cachedGreen);
                        difference += Mathf.Abs((red / 255f) - cachedRed);

                        if (difference / 3f < 0.2f)
                        {
                            colorGroups[colorGroupsKey]++;

                            isFound = true;

                            break;
                        }
                        else
                        {

                        }
                    }

                    if (!isFound)
                    {
                        if (!colorGroups.ContainsKey(colorGroup))
                        {
                            colorGroups.Add(colorGroup, 1);
                        }
                    }
                }
            }
        }

        if (colorGroups.Count == 0)
        {
            return new UnityEngine.Color(0, 0, 0, 1);
        }
        else
        {
            return colorGroups.OrderByDescending(colorGroup => colorGroup.Value).First().Key;
        }
    }
    #endregion

    #region COLOR UTIL
    private bool AreColorsClose(UnityEngine.Color firstColor, UnityEngine.Color secondColor)
    {
        float difference = 0;

        difference += Mathf.Abs(secondColor.r - firstColor.r);
        difference += Mathf.Abs(secondColor.g - firstColor.g);
        difference += Mathf.Abs(secondColor.b - firstColor.b);

        difference /= 3f;

        if (difference < 0.13f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion














    #region SAVE/LOAD
    private void SaveSprite(Sprite sprite, string folderPath, string spriteName)
    {
        if (sprite == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a sprite to save.", "OK");
            return;
        }

        string spritePath = $"{folderPath}/{spriteName}.png";

        AssetDatabase.CreateAsset(sprite, spritePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    public async void SaveSpriteToFile(Sprite sprite, string savePath, string spriteName)
    {
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        Texture2D texture = SpriteToTexture2D(sprite);

        byte[] pngBytes = texture.EncodeToPNG();
        string filePath = Path.Combine(savePath, $"{spriteName}.png");

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
    #endregion
}
