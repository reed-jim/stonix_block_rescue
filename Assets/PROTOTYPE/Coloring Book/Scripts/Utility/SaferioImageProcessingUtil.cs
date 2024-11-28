using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using UnityEditor;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public static class SaferioImageProcessingUtil
    {
        private static double thresh = 100;
        private static double threshLinking = 200;
        private static int blurRadius = 3;

        public static Sprite GenerateOutlineSprite(Sprite targetSprite)
        {
            Image<Bgr, byte> image = TextureToImage(targetSprite.texture);

            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();

            Image<Gray, byte> edges = grayImage.Canny(thresh, threshLinking);

            CvInvoke.GaussianBlur(edges, edges, new System.Drawing.Size(blurRadius, blurRadius), 0);

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(edges, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);

            Image<Bgr, byte> contourHighlightedImage = new Image<Bgr, byte>(image.Width, image.Height, new Bgr(0, 0, 0));

            for (int i = 0; i < contours.Size; i++)
            {
                CvInvoke.DrawContours(contourHighlightedImage, contours, i, new MCvScalar(225, 225, 225), 1);
            }



            Texture2D outlinedTexture = ImageToTexture(contourHighlightedImage);

            Sprite outlinedSprite = Sprite.Create(outlinedTexture, new Rect(0, 0, outlinedTexture.width, outlinedTexture.height), new Vector2(0.5f, 0.5f));

            return outlinedSprite;
        }

        private static Image<Bgr, byte> TextureToImage(Texture2D texture)
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

        private static Texture2D ImageToTexture(Image<Bgr, byte> image)
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

        private static Texture2D ImageToTexture(Image<Gray, byte> grayImage)
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
    }
}
