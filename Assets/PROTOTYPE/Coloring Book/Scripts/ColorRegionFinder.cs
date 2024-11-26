using UnityEngine;
using System.Collections.Generic;

public class ColorRegionFinder : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private Texture2D texture;

    private Texture2D _generatedTexture;

    Color[] pixels;

    public Color edgeColor = Color.black;
    [SerializeField] private Color targetColor;
    [SerializeField] private float threshold;

    void Start()
    {
        texture = spriteRenderer.sprite.texture;

        _generatedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        _generatedTexture.SetPixels(texture.GetPixels());

        pixels = _generatedTexture.GetPixels();

        List<List<Vector2Int>> redRegions = FindRedRegions(texture);

        HighlightRegionEdges(texture, redRegions);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _generatedTexture.SetPixels(pixels);
            _generatedTexture.Apply();

            Sprite newSprite = Sprite.Create(_generatedTexture, new Rect(0, 0, _generatedTexture.width, _generatedTexture.height), new Vector2(0.5f, 0.5f));

            spriteRenderer.sprite = newSprite;
        }
    }

    List<List<Vector2Int>> FindRedRegions(Texture2D texture)
    {
        List<List<Vector2Int>> redRegions = new List<List<Vector2Int>>();

        bool[,] visited = new bool[texture.width, texture.height];

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                if (visited[x, y]) continue;

                Color pixelColor = texture.GetPixel(x, y);

                if (IsRedColor(pixelColor))
                {
                    pixels[x + y * texture.width] = Color.black;

                    List<Vector2Int> region = new List<Vector2Int>();

                    FloodFill(texture, x, y, visited, region);

                    redRegions.Add(region);
                }
            }
        }

        return redRegions;
    }

    bool IsRedColor(Color color)
    {
        if (Vector4.Distance(color, targetColor) < threshold)
        {
            return true;
        }
        else
        {
            return false;
        }

        // Define a simple threshold for the red color
        // A simple red color is when red is higher than both green and blue.
        return color.r > 0.5f && color.g < 0.3f && color.b < 0.3f;
    }

    void FloodFill(Texture2D texture, int startX, int startY, bool[,] visited, List<Vector2Int> region)
    {
        // Use a stack for flood fill (depth-first search)
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();

            int x = current.x;
            int y = current.y;

            // Skip if out of bounds or already visited
            if (x < 0 || x >= texture.width || y < 0 || y >= texture.height || visited[x, y])
                continue;

            visited[x, y] = true;

            // Check if this pixel is red
            if (IsRedColor(texture.GetPixel(x, y)))
            {
                pixels[x + y * texture.width] = Color.black;

                region.Add(current);

                // Push neighboring pixels (4-connectivity)
                stack.Push(new Vector2Int(x + 1, y));
                stack.Push(new Vector2Int(x - 1, y));
                stack.Push(new Vector2Int(x, y + 1));
                stack.Push(new Vector2Int(x, y - 1));
            }
        }
    }

    void HighlightRegionEdges(Texture2D texture, List<List<Vector2Int>> redRegions)
    {
        // Create a new texture to store the highlighted result
        Texture2D highlightedTexture = new Texture2D(texture.width, texture.height);

        // Copy the original texture to the new one
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                highlightedTexture.SetPixel(x, y, texture.GetPixel(x, y));
            }
        }

        // Loop through all regions and highlight edges
        foreach (var region in redRegions)
        {
            foreach (var pixel in region)
            {
                int x = pixel.x;
                int y = pixel.y;

                // Check if this pixel is on the edge of the region
                if (IsEdgePixel(texture, x, y))
                {
                    highlightedTexture.SetPixel(x, y, edgeColor);
                }
            }
        }

        highlightedTexture.Apply();

        Sprite highlightedSprite = Sprite.Create(highlightedTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = highlightedSprite;
    }

    bool IsEdgePixel(Texture2D texture, int x, int y)
    {
        // Check neighboring pixels to see if they are non-red (indicating edge)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = x + dx;
                int ny = y + dy;

                // Skip if out of bounds
                if (nx < 0 || nx >= texture.width || ny < 0 || ny >= texture.height)
                    continue;

                // If any neighboring pixel is not red, it's an edge pixel
                if (!IsRedColor(texture.GetPixel(nx, ny)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
