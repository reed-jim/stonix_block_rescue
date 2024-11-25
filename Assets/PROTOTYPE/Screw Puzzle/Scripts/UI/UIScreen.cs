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
        SceneManager.LoadSceneAsync("Screw Puzzle");
    }
}
