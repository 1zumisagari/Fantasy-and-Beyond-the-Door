using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource resultAudioSource;
    [SerializeField] private AudioClip gameOverSound;

    private bool isGameOver;
    private bool isRestarting;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        // Put this script on the always-active Canvas, not the panel it hides.
        if (gameOverPanel == null || gameOverPanel == gameObject ||
            !gameOverPanel.transform.IsChildOf(transform))
        {
            Debug.LogError("GameOverUI: Assign a child GameOverPanel; put this script on its active Canvas parent.", this);
            enabled = false;
            return;
        }

        // Hide the Game Over screen at the start.
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        // Update and UI input still run while timeScale is zero.
        Keyboard keyboard = Keyboard.current;
        if (isGameOver && keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }

    public void ShowGameOver(float delay = 0f)
    {
        if (!isActiveAndEnabled || isGameOver)
        {
            return;
        }

        isGameOver = true;
        previousTimeScale = Time.timeScale;
        gameOverPanel.transform.SetAsLastSibling();
        // Wait for the death animation before showing the panel.
        StartCoroutine(ShowPanelAfterDeath(delay));
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
        Time.timeScale = 0f;
    }

    private IEnumerator ShowPanelAfterDeath(float delay)
    {
        // Wait using real time, since the game is already paused.
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        gameOverPanel.SetActive(true);
        if (resultAudioSource != null && gameOverSound != null)
        {
            // Play the Game Over sound once.
            resultAudioSource.PlayOneShot(gameOverSound);
        }
    }

    public void Restart()
    {
        if (!isGameOver || isRestarting || !gameOverPanel.activeSelf)
        {
            return;
        }

        isRestarting = true;
        Time.timeScale = previousTimeScale;
        // Reload this level with a fresh player and enemies.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDisable()
    {
        // Do not leave the next scene or Editor session paused.
        if (isGameOver)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}
