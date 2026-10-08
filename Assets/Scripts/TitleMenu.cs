using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "Reality";
    private bool starting;

    // Game Start opens the first scene.
    public void StartGame()
    {
        // Ignore extra clicks while the first scene is loading.
        if (starting) return;
        if (!Application.CanStreamedLevelBeLoaded(firstSceneName))
        {
            Debug.LogError("TitleMenu: To go to the first scene.", this);
            return;
        }
        starting = true;
        // Make sure the new game is not paused.
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstSceneName);
    }
}
