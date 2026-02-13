using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Rules")]
    [Tooltip("Number of enemies the player needs to defeat to win.")]
    [SerializeField] private int enemiesToWin = 3;
    [Header("Flow")]
    [SerializeField] private float restartDelaySeconds = 1.5f;
    [Header("UI")]
    [SerializeField] private TMP_Text enemiesLeftText;
    [SerializeField] private string enemiesLeftPrefix = "Enemies Left: ";
    [SerializeField] private GameObject gameWonPanel;

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

        Time.timeScale = 1f;
        if (gameWonPanel != null) gameWonPanel.SetActive(false);
        RefreshEnemiesLeftUI();
    }

    public void OnEnemyDefeated()
    {
        if (_isGameOver) return;

        _enemiesDefeated++;
        Debug.Log($"Enemies Defeated: {_enemiesDefeated}/{enemiesToWin}");
        RefreshEnemiesLeftUI();

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
        SetWinCursorState();
        Time.timeScale = 0f;
        if (gameWonPanel != null) gameWonPanel.SetActive(true);
    }

    private void LoseGame()
    {
        _isGameOver = true;
        Debug.Log("YOU LOSE!");
        Invoke(nameof(RestartCurrentScene), Mathf.Max(0f, restartDelaySeconds));
    }

    private void RestartCurrentScene()
    {
        RestoreGameplayCursorState();
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    public void PlayAgain()
    {
        if (!_isGameOver) return;
        RestartCurrentScene();
    }

    private void RefreshEnemiesLeftUI()
    {
        if (enemiesLeftText == null) return;

        int left = Mathf.Max(0, enemiesToWin - _enemiesDefeated);
        enemiesLeftText.text = enemiesLeftPrefix + left;
    }

    private void SetWinCursorState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StarterAssetsInputs inputs = FindFirstObjectByType<StarterAssetsInputs>();
        if (inputs == null) return;

        inputs.cursorLocked = false;
        inputs.cursorInputForLook = false;
    }

    private void RestoreGameplayCursorState()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StarterAssetsInputs inputs = FindFirstObjectByType<StarterAssetsInputs>();
        if (inputs == null) return;

        inputs.cursorLocked = true;
        inputs.cursorInputForLook = true;
    }
}
