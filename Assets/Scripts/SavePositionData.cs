using System;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerPositionData
{
    public Vector3 player1Position; // Reference to player 1's vector3.
    public Vector3 player2Position; // Reference to player 2's vector3.
}

public class SavePositionData : MonoBehaviour
{

    public Transform PlayerOneTransform; // Reference to the first player's Transform.
    public Transform PlayerTwoTransform; // Reference to the second player's Transform.

    private void Start()
    {
        // Load the recently saved positions of both players when the game starts.
        LoadPositions(); 
    }

    private void Update()
    {
        // Save the player's positions when 'V' is pressed.
        if (Input.GetKeyDown(KeyCode.V))
        {
            SavePositions();
        }
    }

    private void SavePositions()
    {
        string filePath = Application.persistentDataPath + "/PositionData.json";

        PlayerPositionData positionData = new PlayerPositionData
        {
            player1Position = PlayerOneTransform.position,
            player2Position = PlayerTwoTransform.position
        };

        string json = JsonUtility.ToJson(positionData);
        File.WriteAllText(filePath, json);
        Debug.Log("Position data saved to " + filePath);

    }

    private void LoadPositions()
    {
        string filePath = Application.persistentDataPath + "/PositionData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerPositionData positionData = JsonUtility.FromJson<PlayerPositionData>(json);

            if (PlayerOneTransform != null)
            {
                PlayerOneTransform.position = positionData.player1Position;
            }

            if (PlayerTwoTransform != null)
            {
                PlayerTwoTransform.position = positionData.player2Position;
            }

            Debug.Log("Position data loaded from " + filePath);
        }
        else
        {
            Debug.LogWarning("Position data file does not exist, no positions were loaded.");
        }
    }
}
