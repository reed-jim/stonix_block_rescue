using UnityEngine;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using Emgu.CV.Util;
using System.Drawing;
using System.Runtime.InteropServices;

public class ImageSegmenter : MonoBehaviour
{
    public SpriteRenderer originalSpriteRenderer; // The original SpriteRenderer
    public GameObject spritePrefab;  // Prefab used to display segments (with SpriteRenderer)

    private Texture2D texture;
    private Mat image;

    [SerializeField] private double thresh;
    [SerializeField] private double threshLinking;
    [SerializeField] private int blurRadius;
    [SerializeField] float minContourArea = 100.0f;

    void Start()
    {
        // Step 1: Get the texture from the SpriteRenderer
        texture = originalSpriteRenderer.sprite.texture;

        // Convert the texture to a Mat (Emgu.CV Mat) for processing
        image = new Mat(texture.height, texture.width, DepthType.Cv8U, 3); // Assuming 3 channels (RGB)
        texture.GetPixels32();  // Get the texture pixels for processing

        // Step 2: Process the texture to detect boundaries/objects
        ProcessImageAndSegment();
    }

    void ProcessImageAndSegment()
    {
        Image<Bgr, byte> image = TextureToImage(originalSpriteRenderer.sprite.texture);

        Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

        Image<Gray, byte> edges = grayImage.Canny(thresh, threshLinking);

        Mat binaryImage = new Mat();
        CvInvoke.Threshold(edges, binaryImage, 128, 255, ThresholdType.Binary);

        Mat floodFillImage = new Mat();
        binaryImage.ConvertTo(floodFillImage, DepthType.Cv32S, 1.0);

        CvInvoke.GaussianBlur(edges, edges, new System.Drawing.Size(blurRadius, blurRadius), 0);

        var contours = new VectorOfVectorOfPoint();
        var hierarchy = new Mat();
        CvInvoke.FindContours(edges, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);

        // Step 5: Create a new image to highlight the contours
        Image<Bgr, byte> contourHighlightedImage = new Image<Bgr, byte>(image.Width, image.Height, new Bgr(0, 0, 0)); // Black background

        // Step 6: Fill the contours and draw on the new image (highlighted in red)
        for (int i = 0; i < contours.Size; i++)
        {
            // Fill the contour (using thickness = -1 to fill it) and highlight in red
            CvInvoke.DrawContours(contourHighlightedImage, contours, i, new MCvScalar(225, 225, 225), -1); // Red color, filled
        }

        // Texture2D generatedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        // for (int y = 0; y < generatedTexture.height; y++)
        // {
        //     for (int x = 0; x < generatedTexture.width; x++)
        //     {
        //         // Adjust the pixel position by the bounding box offset
        //         System.Drawing.Point segmentPoint = new System.Drawing.Point(x, y);

        //         for (int j = 0; j < contours.Size; j++)
        //         {
        //             bool isInside = CvInvoke.PointPolygonTest(contours[j], segmentPoint, true) >= 0f;

        //             if (!isInside)
        //             {
        //                 Bgr pixel = image[y, x];
        //                 generatedTexture.SetPixel(x, y, new UnityEngine.Color((float)pixel.Red / 255f, (float)pixel.Green / 255f, (float)pixel.Blue / 255f, 1f)); // Opaque
        //             }
        //             else
        //             {
        //                 generatedTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 1f));
        //             }
        //         }
        //     }
        // }

        // generatedTexture.Apply();

        // Step 7: Convert the highlighted contour image to a Texture2D
        Texture2D contourTexture = ImageToTexture(contourHighlightedImage);

        // Step 8: Create a new sprite from the texture
        Sprite contourSprite = Sprite.Create(contourTexture, new Rect(0, 0, contourTexture.width, contourTexture.height), new Vector2(0.5f, 0.5f));

        // Step 9: Replace the sprite in the SpriteRenderer
        originalSpriteRenderer.sprite = contourSprite;

        // Step 5: Process each contour (segment)
        for (int i = 0; i < contours.Size; i++)
        {
            double contourArea = CvInvoke.ContourArea(contours[i]);

            if (contourArea < minContourArea)
            {
                continue;
            }

            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);

            // Step 6: Extract the segment from the original image
            Image<Bgr, byte> segment = new Image<Bgr, byte>(boundingBox.Width, boundingBox.Height);
            image.ROI = boundingBox;  // Set region of interest (ROI)
            image.CopyTo(segment);    // Copy the ROI into the segment image
            image.ROI = System.Drawing.Rectangle.Empty; // Reset the ROI

            // Step 7: Convert the segment to a Texture2D
            Texture2D segmentTexture = ImageToTexture(segment);

            // Step 8: Check if each pixel is inside the contour using PointPolygonTest
            for (int y = 0; y < segment.Height; y++)
            {
                for (int x = 0; x < segment.Width; x++)
                {
                    // Adjust the pixel position by the bounding box offset
                    System.Drawing.Point segmentPoint = new System.Drawing.Point(x + boundingBox.X, y + boundingBox.Y);

                    // Check if the pixel (segmentPoint) is inside the contour
                    bool isInside = CvInvoke.PointPolygonTest(contours[i], segmentPoint, true) >= 0f;

                    // If the pixel is inside the contour, make it visible (alpha = 1)
                    if (isInside)
                    {
                        Bgr pixel = segment[y, x];
                        segmentTexture.SetPixel(x, y, new UnityEngine.Color((float)pixel.Red / 255f, (float)pixel.Green / 255f, (float)pixel.Blue / 255f, 1f)); // Opaque
                    }
                    else
                    {
                        segmentTexture.SetPixel(x, y, new UnityEngine.Color(0f, 0f, 0f, 0f));
                    }
                }
            }

            segmentTexture.Apply();  // Apply changes to the texture

            // Step 9: Create a sprite from the modified segment texture
            Sprite newSprite = Sprite.Create(segmentTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);

            // Step 10: Spawn a new GameObject to visualize the sprite
            GameObject newSegmentObject = Instantiate(spritePrefab, transform.position, Quaternion.identity);
            newSegmentObject.GetComponent<SpriteRenderer>().sprite = newSprite;

            SpriteRegion spriteRegion = newSegmentObject.AddComponent<SpriteRegion>();

            spriteRegion.Sprite = newSprite;

            newSegmentObject.AddComponent<BoxCollider2D>();

            // Position the new sprite object based on the contour's bounding box

            Vector2 spriteSize = originalSpriteRenderer.sprite.bounds.size;

            newSegmentObject.transform.position = new Vector3(spriteSize.x * ((float)boundingBox.Location.X) / image.Width,
                spriteSize.y * ((float)boundingBox.Location.Y - image.Height) / image.Height, 0);
        }

        originalSpriteRenderer.gameObject.SetActive(false);
    }

    Vector3 BoundingRectToWorldSpacePosition(Rectangle boundingRect, int imageWidth, int imageHeight)
    {
        // Convert the top-left corner of the bounding box to normalized screen coordinates
        float normalizedX = (float)boundingRect.X / imageWidth;
        float normalizedY = 1 - (float)(boundingRect.Y + boundingRect.Height) / imageHeight;  // Y axis is flipped in Unity

        // Convert the normalized coordinates to screen coordinates (pixel space)
        Vector3 screenPosition = new Vector3(normalizedX * Screen.width, normalizedY * Screen.height, 0);

        // Convert screen coordinates to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        // Return the world position of the top-left corner of the bounding box
        return worldPosition;
    }

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
        // Get the pixel data from the texture
        Color32[] pixels = texture.GetPixels32();
        int width = texture.width;
        int height = texture.height;

        // Create a byte array to hold the pixel data in BGR format
        byte[] bytes = new byte[width * height * 3];  // 3 channels for BGR format
        int index = 0;

        // Convert each pixel (Color32) into a byte array (BGR format)
        for (int i = 0; i < pixels.Length; i++)
        {
            bytes[index++] = pixels[i].b;  // Blue
            bytes[index++] = pixels[i].g;  // Green
            bytes[index++] = pixels[i].r;  // Red
        }

        // Create an empty Mat with the required size (width, height) and depth (8-bit unsigned)
        // Mat with 3 channels (RGB), DepthType.Cv8U means 8-bit unsigned integer for each channel
        Mat mat = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, 3);

        // Copy the pixel data into the Mat
        mat.SetTo(bytes);

        return mat;
    }

    Texture2D ConvertMatToTexture(Mat mat)
    {
        // Create a new Texture2D to hold the image
        Texture2D texture = new Texture2D(mat.Width, mat.Height, TextureFormat.RGB24, false);

        // Get the raw image data from the Mat
        byte[] data = new byte[mat.Width * mat.Height * 3]; // 3 for RGB channels
        Marshal.Copy(mat.DataPointer, data, 0, data.Length);

        // Load the raw texture data into the Texture2D
        texture.LoadRawTextureData(data);
        texture.Apply();  // Apply the changes to the texture

        return texture;
    }

    void ApplyMaskToSegment(Image<Bgr, byte> segment, Image<Gray, byte> mask)
    {
        for (int y = 0; y < segment.Height; y++)
        {
            for (int x = 0; x < segment.Width; x++)
            {
                byte maskValue = mask.Data[y, x, 0];  // Get mask value at this pixel (0 for black, 255 for white)
                if (maskValue == 0) // If the pixel is black (outside the contour)
                {
                    segment[y, x] = new Bgr(50, 0, 0);  // Set pixel to black (transparent for Unity)
                }
            }
        }
    }
    #endregion
}
