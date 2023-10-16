using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Establish Singleton.

    public GameObject playerOneCanvas; // Reference to the player's canvas object.
    public Text playerOneScoreText; // Reference to the actual we'll be modifying.
    public Color playerOneColour; // The colour of the text we are going to be using.

    public GameObject playerTwoCanvas; // Reference to the player's canvas object.
    public Text playerTwoScoreText; // Reference to the actual we'll be modifying.
    public Color playerTwoColour; // The colour of the text we are going to be using.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Hide the canvas when we first start the game, until the ball has been dropped.
    /// </summary>
    /// <param name="displayScores"></param>
    public void DisplayScores(bool displayScores)
    {
        // If no canvases have been assigned for the players.
        if(playerOneCanvas == null || playerTwoCanvas == null)
        {
            Debug.LogError("No Canavas has been assigned for this player");
            return; 
        }
        // Display the two canvases.
        playerOneCanvas.SetActive(displayScores);
        playerTwoCanvas.SetActive(displayScores);
    }

    /// <summary>
    /// Update the text and color of the player's score display.
    /// </summary>
    /// <param name="playerOneScore"></param>
    /// <param name="playerTwoScore"></param>
    public void UpdateScores(int playerOneScore, int playerTwoScore)
    {
        // If no text has been assigned for the players.
        if(playerOneScoreText == null || playerTwoScoreText == null)
        {
            Debug.LogError("No Text has been assigned for this player");
            return;
        }

        playerOneScoreText.color = playerOneColour; // Change the colour of our text to match the player colour.
        playerOneScoreText.text = playerOneScore.ToString(); // Set the text to display our score.

        playerTwoScoreText.color = playerTwoColour; // Change the colour of our text to match the player colour.
        playerTwoScoreText.text = playerTwoScore.ToString(); // Set the text to display our score.

    }

}
