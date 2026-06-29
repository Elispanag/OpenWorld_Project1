using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Controller : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel; // Reference to the Main Menu panel   
    public GameObject instructionsPanel; // Reference to the Instructions panel

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // Load the main game scene
    }

    public void OpenInstructions()
    {
        mainMenuPanel.SetActive(false); // Hide the Main Menu panel
        instructionsPanel.SetActive(true); // Show the Instructions panel
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        if(instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("The game is closed!");
        Application.Quit();
    }
}
