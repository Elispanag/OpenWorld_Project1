using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern 
    public static GameManager Instance; 
    [Header("UI References")]
    public TextMeshProUGUI gameOverText; // Reference to the Game Over text UI
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
            Time.timeScale = 1f; // Reset time scale in case it was paused
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
        }
    }

    public void CheckWinCondition(int playerHearts, float aiStars)
    {
        if (isGameOver) return;
        // Win condition: Player collects 10 hearts
        if (playerHearts >= 10)
        {
            EndGame("You Win! \nPress 'R' to Restart");
        }
        //Win condition: AI collects 10 stars
        else if ((int)aiStars >= 10)
        {
            EndGame("You Lose! Sparrow was faster! \nPress 'R' to Restart");
        }
    }

    void EndGame(string message)
    {
        isGameOver = true;
        if (gameOverText != null)
        {   
            gameOverText.gameObject.SetActive(true); // Show the Game Over text
            gameOverText.text = message;
            
        }

        Time.timeScale = 0f; // Pause the game
    }
}
