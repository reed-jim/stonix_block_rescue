using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject levelPrefab;

    private GameObject _currentLevel;

    private void Awake()
    {
        GameplayUIScreen.replayEvent += Replay;

        SpawnLevel();
    }

    private void OnDestroy()
    {
        GameplayUIScreen.replayEvent -= Replay;
    }

    private void SpawnLevel()
    {
        _currentLevel = Instantiate(levelPrefab);
    }

    private void Replay()
    {
        DestroyImmediate(_currentLevel);

        SpawnLevel();
    }
}
