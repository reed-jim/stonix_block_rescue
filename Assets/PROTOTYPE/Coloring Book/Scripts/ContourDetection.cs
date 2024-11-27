using UnityEngine;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using Emgu.CV.Util;
using System.Drawing;

public class EmguCVContourDetection : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite spritePrefab;
    public Texture2D inputTexture;

    void Start()
    {
        // Convert Texture2D to EmguCV Image
        Image<Bgr, byte> image = TextureToImage(inputTexture);

        // Convert to grayscale
        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

        // Apply Canny edge detection
        Image<Gray, byte> edges = grayImage.Canny(100, 200);

        // Find contours
        var contours = new VectorOfVectorOfPoint();
        var hierarchy = new Mat();
        CvInvoke.FindContours(edges, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        // Create a copy of the original image to draw contours on it
        Image<Bgr, byte> imageWithContours = image.Copy();

        // Loop through the contours and draw them with random colors
        System.Random random = new System.Random();
        for (int i = 0; i < contours.Size; i++)
        {
            // Generate a random color for each contour
            MCvScalar randomColor = new MCvScalar(
                random.Next(0, 256), // Blue (0 to 255)
                random.Next(0, 256), // Green (0 to 255)
                random.Next(0, 256)  // Red (0 to 255)
            );

            // Draw the contour with the random color
            CvInvoke.DrawContours(imageWithContours, contours, i, randomColor, 2, LineType.EightConnected, hierarchy, 0);

            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);

            // Spawn the sprite at the center of the bounding box
            SpawnSpriteWithRegion(boundingBox);
        }

        // Convert the image back to Texture2D to display it in Unity
        Texture2D outputTexture = ImageToTexture(imageWithContours);

        Sprite newSprite = Sprite.Create(outputTexture, new Rect(0, 0, outputTexture.width, outputTexture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = newSprite;
    }

    // Convert Texture2D to EmguCV Image<Bgr, byte>
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

    // Convert EmguCV Image<Bgr, byte> to Texture2D
    private Texture2D ImageToTexture(Image<Bgr, byte> image)
    {
        // Create a new Texture2D with the same size as the image
        Texture2D texture = new Texture2D(image.Width, image.Height);

        // Convert Image<Bgr, byte> to Texture2D
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Bgr color = image[y, x];
                texture.SetPixel(x, y, new UnityEngine.Color32((byte)color.Blue, (byte)color.Green, (byte)color.Red, 255));
            }
        }

        texture.Apply();
        return texture;
    }

    private void SpawnSpriteWithRegion(Rectangle boundingBox)
    {
        // Crop the region from the original texture based on the bounding box
        Texture2D regionTexture = CropTexture(inputTexture, boundingBox);

        // Convert the cropped texture to a sprite
        Sprite regionSprite = Sprite.Create(regionTexture, new Rect(0, 0, regionTexture.width, regionTexture.height), new Vector2(0.5f, 0.5f));

        // Create a new GameObject for the sprite
        GameObject spriteObject = new GameObject("RegionSprite");
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = regionSprite;

        // Set the sprite's position to the center of the bounding box
        Vector3 spawnPosition = new Vector3(boundingBox.X + boundingBox.Width / 2, boundingBox.Y + boundingBox.Height / 2, 0);
        spriteObject.transform.position = spawnPosition;

        // Scale the sprite based on the size of the bounding box
        spriteObject.transform.localScale = new Vector3(boundingBox.Width / 100f, boundingBox.Height / 100f, 1);
    }

    // Function to crop the original texture based on the bounding box
    private Texture2D CropTexture(Texture2D originalTexture, Rectangle boundingBox)
    {
        // Create a new Texture2D with the size of the bounding box
        Texture2D croppedTexture = new Texture2D(boundingBox.Width, boundingBox.Height);

        // Copy the pixels from the original texture to the cropped texture
        for (int y = 0; y < boundingBox.Height; y++)
        {
            for (int x = 0; x < boundingBox.Width; x++)
            {
                // Get the pixel from the original texture and set it in the cropped texture
                UnityEngine.Color pixelColor = originalTexture.GetPixel(boundingBox.X + x, boundingBox.Y + y);
                croppedTexture.SetPixel(x, y, pixelColor);
            }
        }

        croppedTexture.Apply();
        return croppedTexture;
    }
}
