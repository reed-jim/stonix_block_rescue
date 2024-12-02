using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetUtil
{
    public static async Task<Sprite> LoadSpriteFromAddressableAsync(string address)
    {
        AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(address);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Texture2D texture = handle.Result;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            return null;
        }
    }
}
