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

public class ImageSegmenter : MonoBehaviour
{
    public SpriteRenderer originalSpriteRenderer;
    public GameObject spritePrefab;

    [Header("CUSTOMIZE")]
    [SerializeField] private double thresh;
    [SerializeField] private double threshLinking;
    [SerializeField] private int blurRadius;
    [SerializeField] float minContourArea = 100.0f;

    [SerializeField] private List<UnityEngine.Color> distinctDetectedColors;

    #region PRIVATE FIELD
    private Texture2D texture;
    #endregion

    #region ACTION
    public static event Action<UnityEngine.Color> spawnColorButtonEvent;
    #endregion      

    private void Awake()
    {
        LevelController.spawnSegmentationSpriteEvent += SpawnSegmentationSprite;

        distinctDetectedColors = new List<UnityEngine.Color>();
    }

    private void OnDestroy()
    {
        LevelController.spawnSegmentationSpriteEvent -= SpawnSegmentationSprite;
    }

    void SpawnSegmentationSprite(CurrentLevelData currentLevelData)
    {
        Debug.Log(currentLevelData.Sprite.name);
        originalSpriteRenderer.sprite = currentLevelData.Sprite;

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
                            segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0.9f, 0.9f, 0.9f, 1f));
                        }
                        else
                        {
                            segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0.1f, 0.1f, 0.1f, 1f));
                        }
                    }
                    else
                    {
                        segmentTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                        segmentHighlightTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                    }
                }
            }

            segmentTexture.Apply();
            segmentHighlightTexture.Apply();


            Sprite newSprite = Sprite.Create(segmentTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);
            Sprite segmentHighlightSprite = Sprite.Create(segmentHighlightTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);

            GameObject newSegmentObject = Instantiate(spritePrefab, transform.position, Quaternion.identity);
            newSegmentObject.GetComponent<SpriteRenderer>().sprite = segmentHighlightSprite;

            SpriteRegion spriteRegion = newSegmentObject.GetComponent<SpriteRegion>();

            spriteRegion.HighlightSprite = segmentHighlightSprite;
            spriteRegion.Sprite = newSprite;

            newSegmentObject.AddComponent<BoxCollider2D>();

            Vector2 spriteSize = originalSpriteRenderer.sprite.bounds.size;

            newSegmentObject.transform.position = new Vector3(spriteSize.x * ((float)boundingBox.Location.X) / image.Width,
                spriteSize.y * ((float)boundingBox.Location.Y - image.Height) / image.Height, 0);



            UnityEngine.Color contourDominantColor = GetMostCommonColorInContour(TextureToMat(segmentTexture), contours[i]);

            bool isCloseColor = false;

            if (distinctDetectedColors.Count == 0)
            {
                distinctDetectedColors.Add(contourDominantColor);
            }
            else
            {
                for (int j = 0; j < distinctDetectedColors.Count; j++)
                {
                    if (AreColorsClose(distinctDetectedColors[j], contourDominantColor))
                    {
                        spriteRegion.ColorGroup = distinctDetectedColors[j];

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
            }

            spriteRegion.ColorGroup = contourDominantColor;

            spawnColorButtonEvent?.Invoke(contourDominantColor);
        }
    }
    #endregion

    #region UTIL
    private Image<Bgr, byte> TextureToImage(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        // Create a byte array for the pixel data
        byte[] imageBytes = new byte[width * height * 3]; // 3 bytes per pixel (BGR)

        int index = 0;
        // Loop through all pixels and fill the byte array with BGR data
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                UnityEngine.Color color = texture.GetPixel(x, y);

                // Convert Unity's Color (r, g, b, a) to BGR
                imageBytes[index++] = (byte)(color.b * 255); // Blue
                imageBytes[index++] = (byte)(color.g * 255); // Green
                imageBytes[index++] = (byte)(color.r * 255); // Red
            }
        }

        // Create EmguCV Image from the byte array (BGR format)
        Image<Bgr, byte> image = new Image<Bgr, byte>(width, height);
        image.Bytes = imageBytes;

        return image;
    }

    private Texture2D ImageToTexture(Image<Gray, byte> grayImage)
    {
        // Create a new Texture2D with the same width and height as the Emgu CV image
        Texture2D texture = new Texture2D(grayImage.Width, grayImage.Height);

        // Loop through each pixel and convert the grayscale value to a Unity Color
        for (int y = 0; y < grayImage.Height; y++)
        {
            for (int x = 0; x < grayImage.Width; x++)
            {
                // Get the grayscale pixel value (0 to 255)
                byte pixelValue = grayImage.Data[y, x, 0];

                // Convert the pixel value to a Unity Color (grayscale value as R, G, B)
                UnityEngine.Color color = new UnityEngine.Color(pixelValue / 255f, pixelValue / 255f, pixelValue / 255f);

                // Set the pixel on the Texture2D
                texture.SetPixel(x, y, color);
            }
        }

        // Apply the changes to the texture
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

        if (difference < 0.2f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
