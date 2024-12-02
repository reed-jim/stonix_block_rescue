using Saferio.Prototype.ColoringBook;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class SoundManager : MonoBehaviour
    {
        private void Awake()
        {
            SpriteRegion.playSoundFillColorEvent += PlaySoundFillColor;
        }

        private void OnDestroy()
        {
            SpriteRegion.playSoundFillColorEvent += PlaySoundFillColor;
        }

        private void PlaySoundFillColor()
        {
            AudioSource soundFillColor = ObjectPoolingEverything.GetFromPool(GameConstant.FILL_COLOR_SOUND).GetComponent<AudioSource>();

            soundFillColor.gameObject.SetActive(true);

            soundFillColor.Play();
        }
    }
}
