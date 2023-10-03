using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public int playerNumber = 1; // The player that scored's number.
    public GameManager gameManager; // Reference to game manager.

    public GameObject fireWorksPrefab; // Reference to our firework prefab.
    public Transform leftFireWorksPosition; // An empty transform to the left of our goal.
    public Transform rightFireWorksPosition; // An empty transform to the right of our goal.


    private void OnTriggerEnter(Collider other)
    {
        // When the soccerball collides with the goal.
        if(other.CompareTag("SoccerBall"))
        {
            // Score a goal.
            Debug.Log("goal Score!");
            gameManager.IncreasePlayerScore(playerNumber);

            // Spawn in our fireworks at the left and right positions respectively, and parent them to our AR parent.
            GameObject leftFireWorksCLone = Instantiate(fireWorksPrefab, leftFireWorksPosition.position, fireWorksPrefab.transform.rotation, gameManager.arContentParent); // Gave the firework clone a clear varible name.
            Destroy(leftFireWorksCLone, 5); // Destroy the fireworks after they play.
            GameObject rightFireWorksClone = Instantiate(fireWorksPrefab, rightFireWorksPosition.position, fireWorksPrefab.transform.rotation, gameManager.arContentParent); // Gave the firework clone a clear varible name.
            Destroy(rightFireWorksClone, 5); // Destroy the fireworks after they play.
        }
    }

    // Display the a cube, with it's color based on the player's number.
    private void OnDrawGizmos()
    {
        Gizmos.color = playerNumber == 1 ? Color.red : Color.blue; // A short hand if statement.
        Gizmos.DrawCube(transform.position, transform.localScale); // Show our cube.
    }
}
