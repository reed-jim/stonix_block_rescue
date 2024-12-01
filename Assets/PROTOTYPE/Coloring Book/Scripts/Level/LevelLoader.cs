using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Saferio.Prototype.ColoringBook
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private CurrentLevelData currentLevelData;

        private void Awake()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Level {currentLevelData.Level}");

            handle.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject loadedObject = Instantiate(op.Result, transform.position, Quaternion.identity);
                }
                else
                {

                }
            };
        }
    }
}
