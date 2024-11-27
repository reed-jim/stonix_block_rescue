using UnityEngine;
using System.Collections.Generic;

public class RegionExtraction : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;  // The SpriteRenderer to work with
    public SpriteRenderer exampleSpriteRenderer;

    public Texture2D inputTexture;         // The input texture to perform edge detection on
    private Texture2D edgeMap;             // The edge map after detection

    private List<List<Vector2Int>> regions; // List of extracted regions

    void Start()
    {
        // Perform edge detection and extract regions
        edgeMap = GetEdgeMap(inputTexture);
        regions = ExtractRegions(edgeMap);

        // Replace the sprite with a new texture that visualizes the regions
        Sprite newSprite = CreateRegionSprite(edgeMap, regions);
        spriteRenderer.sprite = newSprite;





        Texture2D exampleTexture = new Texture2D(inputTexture.width, inputTexture.height, TextureFormat.RGBA32, false);

        foreach (var pixel in regions[1])
        {
            exampleTexture.SetPixel(pixel.x, pixel.y, inputTexture.GetPixel(pixel.x, pixel.y));
        }

        exampleTexture.Apply();

        Sprite exampleSprite = Sprite.Create(exampleTexture, new Rect(0, 0, exampleTexture.width, exampleTexture.height), new Vector2(0.5f, 0.5f));
        exampleSpriteRenderer.sprite = exampleSprite;
    }

    // Step 1: Edge detection using a simple threshold (no shaders)
    Texture2D GetEdgeMap(Texture2D inputTexture)
    {
        int width = inputTexture.width;
        int height = inputTexture.height;

        Texture2D edgeMap = new Texture2D(width, height);

        // Sobel kernels for detecting horizontal and vertical gradients
        int[,] sobelX = new int[3, 3] {
        { -1,  0,  1 },
        { -2,  0,  2 },
        { -1,  0,  1 }
    };
        int[,] sobelY = new int[3, 3] {
        { -1, -2, -1 },
        {  0,  0,  0 },
        {  1,  2,  1 }
    };

        // Apply Sobel filter to calculate gradients
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                int gradientX = 0;
                int gradientY = 0;

                // Apply Sobel kernels on the 3x3 neighborhood of the current pixel
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        Color pixelColor = inputTexture.GetPixel(x + i, y + j);
                        float brightness = pixelColor.grayscale;

                        gradientX += (int)(brightness * sobelX[i + 1, j + 1]);
                        gradientY += (int)(brightness * sobelY[i + 1, j + 1]);
                    }
                }

                // Calculate the magnitude of the gradient
                float magnitude = Mathf.Sqrt(gradientX * gradientX + gradientY * gradientY);
                edgeMap.SetPixel(x, y, magnitude > 0.0f ? Color.white : Color.black); // Adjust threshold
            }
        }

        edgeMap.Apply();
        return edgeMap;
    }

    // Step 2: Extract regions using flood fill (DFS)
    List<List<Vector2Int>> ExtractRegions(Texture2D edgeMap)
    {
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        bool[,] visited = new bool[edgeMap.width, edgeMap.height];

        for (int y = 0; y < edgeMap.height; y++)
        {
            for (int x = 0; x < edgeMap.width; x++)
            {
                if (visited[x, y] || edgeMap.GetPixel(x, y) == Color.black)
                    continue;

                // New region found, perform flood fill
                List<Vector2Int> region = new List<Vector2Int>();
                FloodFill(edgeMap, x, y, visited, region);
                regions.Add(region);
            }
        }

        return regions;
    }

    // Flood fill algorithm to find connected regions
    void FloodFill(Texture2D edgeMap, int startX, int startY, bool[,] visited, List<Vector2Int> region)
    {
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            int cx = current.x;
            int cy = current.y;

            if (cx < 0 || cx >= edgeMap.width || cy < 0 || cy >= edgeMap.height || visited[cx, cy] || edgeMap.GetPixel(cx, cy) == Color.black)
                continue;

            visited[cx, cy] = true;
            region.Add(current);

            // Check 4 neighboring pixels (up, down, left, right)
            stack.Push(new Vector2Int(cx + 1, cy)); // Right
            stack.Push(new Vector2Int(cx - 1, cy)); // Left
            stack.Push(new Vector2Int(cx, cy + 1)); // Up
            stack.Push(new Vector2Int(cx, cy - 1)); // Down
        }
    }

    // Step 3: Create a new sprite with regions visualized
    Sprite CreateRegionSprite(Texture2D edgeMap, List<List<Vector2Int>> regions)
    {
        Texture2D regionTexture = new Texture2D(edgeMap.width, edgeMap.height);

        // Randomly assign a color to each region
        foreach (var region in regions)
        {
            Color randomColor = new Color(Random.value, Random.value, Random.value);

            foreach (var pixel in region)
            {
                regionTexture.SetPixel(pixel.x, pixel.y, randomColor);
            }
        }

        // Apply the changes to the texture
        regionTexture.Apply();

        // Create a new sprite from the modified texture
        return Sprite.Create(regionTexture, new Rect(0, 0, regionTexture.width, regionTexture.height), new Vector2(0.5f, 0.5f));
    }
}
