using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameClearUI : MonoBehaviour
{
    [SerializeField] private GameObject gameClearPanel;
    [SerializeField] private string restartSceneName = "Reality";
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource resultAudioSource;
    [SerializeField] private AudioClip clearSound;

    private bool isCleared;
    private bool isRestarting;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        // Keep this script on the active Canvas, just like GameOverUI.
        if (gameClearPanel == null || gameClearPanel == gameObject ||
            !gameClearPanel.transform.IsChildOf(transform))
        {
            Debug.LogError("GameClearUI: Assign a child GameClearPanel on the active Canvas.", this);
            enabled = false;
            return;
        }

        // Hide the clear screen when the level starts.
        gameClearPanel.SetActive(false);
        if (string.IsNullOrWhiteSpace(restartSceneName) ||
            !Application.CanStreamedLevelBeLoaded(restartSceneName))
        {
            Debug.LogError("GameClearUI: Add Restart Scene Name to the active Build Profiles Scene List: " + restartSceneName, this);
            enabled = false;
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        // R starts a new run after clearing the game.
        if (isCleared && keyboard != null && keyboard.rKey.wasPressedThisFrame)
        {
            Restart();
        }
    }

    public void ShowGameClear(PlayerHealth player)
    {
        if (!isActiveAndEnabled || isCleared || player == null ||
            !player.isActiveAndEnabled || player.IsDead || Time.timeScale <= 0f)
        {
            return;
        }

        isCleared = true;
        // Stop damage and controls so the clear screen cannot turn into Game Over.
        player.enabled = false;
        player.GetComponent<PlayerController>().enabled = false;
        PlayerAttack attack = player.GetComponent<PlayerAttack>();
        if (attack != null)
        {
            attack.enabled = false;
        }
        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        body.linearVelocity = Vector2.zero;
        body.simulated = false;

        previousTimeScale = Time.timeScale;
        // Put the clear screen above the other UI.
        gameClearPanel.transform.SetAsLastSibling();
        gameClearPanel.SetActive(true);
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }
        if (resultAudioSource != null && clearSound != null)
        {
            resultAudioSource.PlayOneShot(clearSound);
        }
        // Pause the level while the clear screen is open.
        Time.timeScale = 0f;
        Debug.Log("Game Clear", this);
    }

    public void Restart()
    {
        if (!isCleared || isRestarting)
        {
            return;
        }

        isRestarting = true;
        Time.timeScale = previousTimeScale;
        // Start again from Reality.
        SceneManager.LoadScene(restartSceneName, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        if (isCleared)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}
