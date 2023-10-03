using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnZone : MonoBehaviour
{
    public GameManager gameManager; // Reference to our game manager.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called when object collides with a trigger.
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = gameManager.ReturnRandomPositionOnField(); // Grab a random position in the world and move the object back there.
        if(other.GetComponent<Rigidbody>()) // Check to see if the object falling through has a rigidbody, if it does continue.
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero; // Reset our veloicty to 0.
            other.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;// Reset our velocity to zero.
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }

}
