using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Rules")]
    [Tooltip("Number of enemies the player needs to defeat to win.")]
    [SerializeField] private int enemiesToWin = 3;

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
    }
}