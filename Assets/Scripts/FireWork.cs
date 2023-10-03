using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWork : MonoBehaviour
{
    public AudioClip fireWorkSound; // The firework sound.
    public AudioSource audioSource; // Reference to our audiosource.
    public int numberOfFireworks = 3; // The number of fireworks that will be spawned.
    public float initialDelay = 2; // An initial delay before the first firework is spawned.
    public float timeBetweenFireWorks = 0.5f; // Half a second between each firework.


    // Start is called before the first frame update.
    void Start()
    {
        StartCoroutine(PlayFireworks()); // Start our coroutine up here.
    }

    /// <summary>
    /// a coroutine that allows us to dictate when certain of code should be played, this allows us to delay certain parts of code.
    /// but it also allows us do more complex actions.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayFireworks()
    {
        yield return new WaitForSeconds(initialDelay); // Wait a couple of seconds before continuing with our code.
        for(int i =0; i<numberOfFireworks; i++)
        {
            audioSource.PlayOneShot(fireWorkSound); // Play the fireworks sound once.
            yield return new WaitForSeconds(timeBetweenFireWorks); // Now wait before before we iterate to the next part of the for loop.
        }

        yield return null;
    }
}
