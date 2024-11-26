using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
    }

    private void Play()
    {
        PlaySoundClick();

        SceneManager.LoadSceneAsync("Screw Puzzle");
    }

    private void PlaySoundClick()
    {
        AudioSource selectSound = ObjectPoolingEverything.GetFromPool("ClickSound").GetComponent<AudioSource>();

        selectSound.gameObject.SetActive(true);

        selectSound.Play();
    }
}
