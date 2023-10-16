using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Establish Singleton.

    public Transform soccerField; // A reference to our soccer field.
    public Vector3 moveArea; // The size of our area where we can move.
    public Transform arCamera; // Reference to our AR camera.

    public GameObject soccerballPrefab; // A reference to the soccer ball in our scene.
    private GameObject currentSoccerBallInstance; // The current soccerball that has been spawned in.
    public Transform arContentParent; // Reference to the overall parent of the ar content.

    public int playerOneScore;
    public int playerTwoScore;


    public UIManager uiManager; // Reference to our UI Manager.

    public AudioManager audioManager; // Reference to our audio manager.

    private bool areCharactersRunningAway = false; // Are there any characters currently running away from the player.

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




    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("New Random Position of:" + ReturnRandomPositionOnField());
        playerOneScore = 0;
        playerTwoScore = 0; // Reset our players scores.
        uiManager.DisplayScores(false); // Hide our canvases to start with.
        uiManager.UpdateScores(playerOneScore, playerTwoScore); // Update our players scores.
    }

    // Update is called once per frame
    void Update()
    {
        // Update Logic
    }

    /// <summary>
    /// Increase the passed in players score by 1.
    /// </summary>
    /// <param name="playerNumber"></param>
    public void IncreasePlayerScore(int playerNumber)
    {
        if (playerNumber == 1)
        {
            playerOneScore++;
        }
        else if(playerNumber == 2)
        {
            playerTwoScore++;
        }
        ResetSoccerBall();
        uiManager.UpdateScores(playerOneScore, playerTwoScore); // Updates the ui score to show our current values.
    }

    /// <summary>
    /// Resets the balls positions and velocities.
    /// </summary>
    private void ResetSoccerBall()
    {
        Rigidbody ballRigidbody = currentSoccerBallInstance.GetComponent<Rigidbody>(); // Created a variable that stores the ball's rigidbody instead of using "getcomponent" several times.
        ballRigidbody.velocity = Vector3.zero; // Reset the velocity of the ball.
        ballRigidbody.angularVelocity = Vector3.zero; // Reset the angular velocity of the ball.
        currentSoccerBallInstance.transform.position = ReturnRandomPositionOnField(); // Reset the position of the ball.
    }

    /// <summary>
    /// Returns a random position within our move area.
    /// </summary>
    /// <returns></returns>
    public Vector3 ReturnRandomPositionOnField()
    {
        float xPosition = Random.Range(-moveArea.x / 2, moveArea.x / 2); // Gives us a random number between negative moveArea X and positive moveAreaX.
        float yPosition = soccerField.position.y; // Our soccer fields y transform position.
        float zPosition = Random.Range(-moveArea.z / 2, moveArea.z / 2); // Gives us a random number between negative moveArea Z and positive moveAreaZ.

        return new Vector3(xPosition, yPosition, zPosition);
    }

    /// <summary>
    /// This is a debug function, that lets us draw objects in our scene view, its not viewable in the game view.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // If the user hasn't put a soccer field in, just get out of this function.
        if (soccerField == null)
        {
            return;
        }
        Gizmos.color = Color.red; // Sets my gizmo to red.
        Gizmos.DrawCube(soccerField.position + new Vector3(0,0.5f,0), moveArea); // Draws a cube the at the soccer fields position, and to the size of our move area.
    }

    /// <summary>
    /// Return true or false if we are too close, or not close enough to our AR camera.
    /// </summary>
    /// <param name="character"></param>
    /// <param name="distanceThreshold"></param>
    /// <returns></returns>
    public bool IsPlayerToCloseToCharacter(Transform character, float distanceThreshold)
    {
        if (Vector3.Distance(arCamera.position, character.position) <= distanceThreshold)
        {
            // Returns true if we are too close.
            return true;
        }
        else
        {
            // Returns false if we are too far away.
            return false;
        }
    }

    /// <summary>
    /// Spawns in a new soccer ball based on the position provided. If a soccer ball already exists in the world, we just want to move it to that new position.
    /// </summary>
    /// <param name="positionToSpawn"></param>
    public void SpawnOrMoveSoccerBall(Vector3 positionToSpawn)
    {
        if(soccerballPrefab == null)
        {
            // Return if there is no soccerball assigned in the unity inspector.
            Debug.LogError("Something is wrong there is no soccerball assigned in the inspector");
            return;
        }

        // If the soccer ball isn't spawned into the world yet.
        if(currentSoccerBallInstance == null)
        {
            // Spawn in and store a reference to our soccer ball, and parent it to our AR content parent.
            currentSoccerBallInstance = Instantiate(soccerballPrefab, positionToSpawn, soccerballPrefab.transform.rotation, arContentParent);
            Rigidbody ballRigidbody = currentSoccerBallInstance.GetComponent<Rigidbody>(); // Created a variable that stores the ball's rigidbody instead of using "getcomponent" several times.
            ballRigidbody.velocity = Vector3.zero; // Sets the velocity of the soccer ball to 0.
            ballRigidbody.angularVelocity = Vector3.zero; // Sets the angular velocity of the soccer ball to 0.
            AlertCharactersToSoccerBallSpawningIn(); // Tell everyone the ball is spawned.
        }
        else
        {
            // The soccer ball already exists, so lets just move it.
            currentSoccerBallInstance.transform.position = positionToSpawn; // Move our soccerball to the position to spawn
            Rigidbody ballRigidbody = currentSoccerBallInstance.GetComponent<Rigidbody>(); // Created a variable that stores the ball's rigidbody instead of using "getcomponent" several times.
            ballRigidbody.velocity = Vector3.zero; // Sets the velocity of the soccer ball to 0.
            ballRigidbody.angularVelocity = Vector3.zero; // Sets the angular velocity of the soccer ball to 0.
        }
    }


    /// <summary>
    /// Finds all the characters in the scene and loops through them and tells them, that there is a soccerball.
    /// </summary>
    private void AlertCharactersToSoccerBallSpawningIn()
    {
        CharacterController[] characters = FindObjectsOfType<CharacterController>(); // Find all instances of our character controller class in our scene.
        foreach (CharacterController character in characters) // Changed to a for each loop for better readability.
        {
            character.SoccerBallSpawned(currentSoccerBallInstance.transform);
        }
        uiManager.DisplayScores(true); // Display our scores on our goals.
        if (audioManager != null) // If we have a reference to the audio manager.
        {
            audioManager.PlayPlayingMusic(); // Start playing the second track/the soccer playing music.
        }
    }

    /// <summary>
    /// A function to handle the characters telling us that the player is to close, so play some music.
    /// </summary>
    /// <param name="isRunningAway"></param>
    public void RunningAwayFromPlayer(bool isRunningAway)
    {
        if (isRunningAway == areCharactersRunningAway) // Don't do anything if the value is the same that is coming in.
        {
            return;
        }
        else
        {
            areCharactersRunningAway = isRunningAway; // Set our private bool to this value.
        }

        // if characters are running away in fear.
        if(areCharactersRunningAway == true)
        {
            audioManager.PlayFleeingMusic(); // Start playing the fleeing music.
        }
        else
        {
            audioManager.PlayPreviousTrack(); // Otherwise start playing the last track.
        }
    }
}
