using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Rules")]
    [Tooltip("Number of enemies the player needs to defeat to win.")]
    [SerializeField] private int enemiesToWin = 3;
    [Header("Flow")]
    [SerializeField] private float restartDelaySeconds = 1.5f;

    private int _enemiesDefeated;
    private bool _isGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEnemyDefeated()
    {
        if (_isGameOver) return;

        _enemiesDefeated++;
        Debug.Log($"Enemies Defeated: {_enemiesDefeated}/{enemiesToWin}");

        if (_enemiesDefeated >= enemiesToWin)
        {
            WinGame();
        }
    }

    public void OnPlayerDied()
    {
        if (_isGameOver) return;
        LoseGame();
    }

    private void WinGame()
    {
        _isGameOver = true;
        Debug.Log("YOU WIN!");
        AudioManager.Instance?.PlayWinning();
    }

    private void LoseGame()
    {
        _isGameOver = true;
        Debug.Log("YOU LOSE!");
        Invoke(nameof(RestartCurrentScene), Mathf.Max(0f, restartDelaySeconds));
    }

    private void RestartCurrentScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}
