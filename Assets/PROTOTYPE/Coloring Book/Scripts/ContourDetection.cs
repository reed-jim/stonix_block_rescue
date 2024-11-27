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

        Image<Gray, byte> edges = grayImage.Canny(100, 200);

        var contours = new VectorOfVectorOfPoint();
        var hierarchy = new Mat();
        CvInvoke.FindContours(edges, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        // Step 5: Process each contour (segment)
        for (int i = 0; i < contours.Size; i++)
        {
            // Extract the bounding box for each contour (a segment)
            Rectangle boundingBox = CvInvoke.BoundingRectangle(contours[i]);

            // Step 6: Extract the segment from the original image
            Image<Bgr, byte> segment = new Image<Bgr, byte>(boundingBox.Width, boundingBox.Height);
            image.ROI = boundingBox;  // Set region of interest (ROI)
            image.CopyTo(segment);    // Copy the ROI into the segment image
            image.ROI = System.Drawing.Rectangle.Empty; // Reset the ROI

            // Step 7: Create a mask for the contour (inside the contour is white, outside is black)
            Image<Gray, byte> mask = new Image<Gray, byte>(boundingBox.Width, boundingBox.Height);
            mask.SetZero();  // Start with a black (transparent) mask
            Point[] contourArray = contours[i].ToArray();  // Convert contour to array of points

            // Convert the Point[] array to VectorOfPoint
            VectorOfPoint contourVector = new VectorOfPoint(contourArray);

            // Step 8: Draw the contour on the mask (white)
            // Create a VectorOfVectorOfPoint to hold all contours for FillPoly
            VectorOfVectorOfPoint contoursToFill = new VectorOfVectorOfPoint();
            contoursToFill.Push(contourVector);  // Push the single contour into the container

            // Use FillPoly with VectorOfVectorOfPoint
            CvInvoke.FillPoly(mask, contoursToFill, new MCvScalar(255));  // Use VectorOfVectorOfPoint

            // Step 9: Apply the mask to the segment (set outside alpha to 0)
            ApplyMaskToSegment(segment, mask);

            // Step 10: Convert the masked segment into a Texture2D
            Texture2D segmentTexture = ImageToTexture(segment);

            // Step 11: Create a sprite from the masked segment texture
            Sprite newSprite = Sprite.Create(segmentTexture, new Rect(0, 0, segmentTexture.width, segmentTexture.height), Vector2.zero);

            // Step 12: Spawn a new GameObject to visualize the sprite
            GameObject newSegmentObject = Instantiate(spritePrefab, transform.position, Quaternion.identity);
            newSegmentObject.GetComponent<SpriteRenderer>().sprite = newSprite;

            // Position the new sprite object based on the contour's bounding box
            newSegmentObject.transform.position = new Vector3((float)boundingBox.X / texture.width, (float)boundingBox.Y / texture.height, 0);
        }

        originalSpriteRenderer.gameObject.SetActive(false);
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

    // Convert EmguCV Image<Bgr, byte> to Texture2D
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
