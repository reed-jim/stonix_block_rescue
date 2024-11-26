using UnityEngine;

public class SpriteAlphaRemover : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Texture2D maskTexture;

    public float radius = 50f;

    // void Start()
    // {
    //     ApplyAlphaMask();
    // }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ClearAtMouse();
        }
    }

    void ApplyAlphaMask()
    {
        Texture2D texture = spriteRenderer.sprite.texture;

        Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        readableTexture.SetPixels(texture.GetPixels());

        Color[] pixels = readableTexture.GetPixels();

        for (int y = 0; y < readableTexture.height; y++)
        {
            for (int x = 0; x < readableTexture.width / 2f; x++)
            {
                Color pixelColor = pixels[y * readableTexture.width + x];

                pixelColor.a = 0;

                pixels[y * readableTexture.width + x] = pixelColor;
            }
        }

        readableTexture.SetPixels(pixels);
        readableTexture.Apply();

        Sprite newSprite = Sprite.Create(readableTexture, new Rect(0, 0, readableTexture.width, readableTexture.height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = newSprite;
    }

    private void ClearAtMouse()
    {
        Texture2D texture = spriteRenderer.sprite.texture;

        Texture2D readableTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

        readableTexture.SetPixels(texture.GetPixels());

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Convert the world position to local texture coordinates
        Vector2 texturePos = spriteRenderer.transform.InverseTransformPoint(mouseWorldPos);

        int width = readableTexture.width;
        int height = readableTexture.height;

        Vector2 spriteSizeWorld = spriteRenderer.sprite.bounds.size;

        Color[] pixels = readableTexture.GetPixels();

        // IMPORTANT: (0, 0) of a texture is LEFT-BOTTOM CORNER
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Vector2 pixelPos = new Vector2(x, y);
                // float distance = Vector2.Distance(pixelPos, texturePos);

                Vector2 pixelWorldPosition = new Vector2(
                    (x - 0.5f * width) / width * spriteSizeWorld.x,
                    (y - 0.5f * height) / height * spriteSizeWorld.y
                );

                float distance = Vector2.Distance(pixelWorldPosition, mouseWorldPos);

                if (distance <= radius)
                {
                    int index = y * width + x;
                    Color color = pixels[index];
                    color.a = 0f;
                    pixels[index] = color;
                }
            }
        }

        readableTexture.SetPixels(pixels);
        readableTexture.Apply();

        Sprite newSprite = Sprite.Create(readableTexture, new Rect(0, 0, readableTexture.width, readableTexture.height), new Vector2(0.5f, 0.5f));

        spriteRenderer.sprite = newSprite;
    }
}
