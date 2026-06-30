using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern 
    public static GameManager Instance; 
    [Header("UI References")]
    public GameObject winPanel;
    public GameObject losePanel;
    private bool isGameOver = false; // Flag to check if the game is over

    void Awake()
    {
        // Implementing the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    void Update()
    {
        // Check for restart input when the game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    public void PlayerWon()
    {
        
        if (isGameOver) return;
        isGameOver = true;
        // Activate win panel
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void PlayerLost()
    {
        if (isGameOver) return;
        isGameOver = true;
        // Activate lose panel
        if (losePanel != null) losePanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void CheckWinCondition(int playerHearts, float aiStars)
    {
        if (isGameOver) return;
        // Win condition: Player collects 10 hearts
        if (playerHearts >= 10)
        {
            PlayerWon();
        }
        //Win condition: AI collects 10 stars
        else if ((int)aiStars >= 10)
        {
            PlayerLost();
        }
    }
}
