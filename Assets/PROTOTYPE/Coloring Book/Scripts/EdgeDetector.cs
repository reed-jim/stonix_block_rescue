using UnityEngine;

public class EdgeDetector : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private int blurRadius = 2;
    [SerializeField] private float threshold = 0.1f;

    void Start()
    {
        Texture2D blurredTexture = ApplyGaussianBlur(spriteRenderer.sprite.texture, blurRadius);
        Texture2D edgeTexture = ApplyEdgeDetection(blurredTexture);

        Sprite newSprite = Sprite.Create(edgeTexture, new Rect(0, 0, edgeTexture.width, edgeTexture.height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = newSprite;

        spriteRenderer.sprite = newSprite;
    }

    Texture2D ApplyGaussianBlur(Texture2D texture, int radius)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D blurredTexture = new Texture2D(width, height);
        Color[] pixels = texture.GetPixels();
        Color[] blurredPixels = new Color[width * height];

        // Calculate Gaussian kernel based on radius
        float[,] kernel = GenerateGaussianKernel(radius);
        float kernelSum = 0f;

        // Calculate the sum of all weights in the kernel for normalization
        for (int i = 0; i < 2 * radius + 1; i++)
        {
            for (int j = 0; j < 2 * radius + 1; j++)
            {
                kernelSum += kernel[i, j];
            }
        }

        // Apply the kernel to each pixel
        for (int y = radius; y < height - radius; y++)  // Skip borders to avoid out-of-bounds access
        {
            for (int x = radius; x < width - radius; x++)
            {
                float r = 0f, g = 0f, b = 0f;

                // Apply Gaussian kernel to the neighborhood of each pixel
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        Color pixelColor = pixels[(y + i) * width + (x + j)];
                        float weight = kernel[i + radius, j + radius] / kernelSum;  // Normalize weight
                        r += pixelColor.r * weight;
                        g += pixelColor.g * weight;
                        b += pixelColor.b * weight;
                    }
                }

                blurredPixels[y * width + x] = new Color(r, g, b);
            }
        }

        blurredTexture.SetPixels(blurredPixels);
        blurredTexture.Apply();
        return blurredTexture;
    }

    // Generate a Gaussian kernel based on the given radius
    float[,] GenerateGaussianKernel(int radius)
    {
        int size = 2 * radius + 1;
        float[,] kernel = new float[size, size];
        float sigma = radius / 3f; // This defines the "spread" of the Gaussian distribution
        float sigma2 = 2 * sigma * sigma;
        float piSigma2 = Mathf.PI * sigma2;
        float sum = 0f;

        // Create the kernel based on the Gaussian function
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                float weight = Mathf.Exp(-(x * x + y * y) / sigma2) / piSigma2;
                kernel[y + radius, x + radius] = weight;
                sum += weight;
            }
        }

        // Normalize the kernel so that the sum of all weights is 1
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                kernel[i, j] /= sum;
            }
        }

        return kernel;
    }

    Texture2D ApplyEdgeDetection(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        Texture2D edgeTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        Color[] pixels = texture.GetPixels();

        int[] sobelX = new int[] { -1, 0, 1, -2, 0, 2, -1, 0, 1 };
        int[] sobelY = new int[] { -1, -2, -1, 0, 0, 0, 1, 2, 1 };

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int gx = 0;
                int gy = 0;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        Color pixelColor = pixels[(y + i) * width + (x + j)];
                        float gray = pixelColor.grayscale;

                        gx += (int)(sobelX[(i + 1) * 3 + (j + 1)] * gray);
                        gy += (int)(sobelY[(i + 1) * 3 + (j + 1)] * gray);
                    }
                }

                float magnitude = Mathf.Sqrt(gx * gx + gy * gy);
                if (magnitude > threshold)
                {
                    edgeTexture.SetPixel(x, y, new Color(magnitude, magnitude, magnitude));
                }
                else
                {
                    edgeTexture.SetPixel(x, y, Color.black);
                }
            }
        }

        edgeTexture.Apply();
        return edgeTexture;
    }
}
