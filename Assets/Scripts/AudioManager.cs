using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public AudioClip roamingMusic; // The music we will use when roaming.
    public AudioClip playingMusic; // The music we will use whilst they play soccer.
    public AudioClip fleeingMusic; // Play a people screaming sound.

    private AudioClip currentTrack; // The current track being played.
    private AudioClip previousTrack; // The previous track that was played.
    public AudioSource audioSource; // A reference to our audiosource, where the music will be played from.

    /// <summary>
    /// So this gets called everytime the script gets turn off/on.
    /// </summary>
    private void OnEnable()
    {
        // If there is no current track.
        if(currentTrack == null)
        {
            currentTrack = roamingMusic; // Play the roaming music by defult.
        }
        ChangeTrack(currentTrack); // Start playing music.
    }

    /// <summary>
    /// Plays the roaming music that is played at the start of the game whilst roaming.
    /// </summary>
    public void PlayRoamingMusic()
    {
        // Set the current music to the roaming music.
        currentTrack = roamingMusic;
        ChangeTrack(currentTrack);
    }

    /// <summary>
    /// Plays music that is used during the game.
    /// </summary>
    public void PlayPlayingMusic()
    {
        // Set the current music to the playing music.
        currentTrack = playingMusic;
        ChangeTrack(currentTrack);
    }

    /// <summary>
    /// Plays the fleeing music.
    /// </summary>
    public void PlayFleeingMusic()
    {
        // Set the current music to the fleeing music.
        currentTrack = fleeingMusic;
        ChangeTrack(currentTrack);
    }

    /// <summary>
    /// Play the previous track that was being played.
    /// </summary>
    public void PlayPreviousTrack()
    {
        // If there is no previous track.
        if(previousTrack == null)
        {
            return; 
        }
        currentTrack = previousTrack; // Set the current track to the previous track.
        ChangeTrack(currentTrack); // play our previous track.
    }

    /// <summary>
    /// This function changes the clip being played at the momenet.
    /// </summary>
    /// <param name="clip"></param>
    private void ChangeTrack(AudioClip clip)
    {
        audioSource.Stop(); // Stop playing the current clip.
        if(audioSource.clip != clip) // If the current clip in the audio source is not equal to the clip we are trying to play.
        {
            previousTrack = audioSource.clip; // Store the previous track.
            audioSource.clip = clip; // Set the new track.
        }
        audioSource.loop = true; // Set the track to be looping.
        audioSource.Play(); // Start playing our music.
    }
}
