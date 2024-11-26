using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterLevelButton : MonoBehaviour
{
    [SerializeField] private Button enterLevelButton;

    private void Awake()
    {
        enterLevelButton.onClick.AddListener(GoToLevel);
    }

    private void GoToLevel()
    {
        SceneManager.LoadSceneAsync("Delete Master - Gameplay");
    }
}
