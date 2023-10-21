using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using static CharacterController;

/// <summary>
/// State Class.
/// </summary>
public class State
{
    public StateManager manager;
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Update() { }
}

/// <summary>
/// State Manager Class.
/// </summary>
public class StateManager 
{
    Dictionary<string, State> states = new Dictionary<string, State>();
    public State currentState = null;
    public string stateName = "";
    public CharacterController characterController;

    public void Start()
    {
        states.Add("Idle", new StateIdle(this));
        states.Add("Roaming", new StateRoaming(this));
        states.Add("Waving", new StateWaving(this));
        states.Add("Playing", new StatePlaying(this));
        states.Add("Fleeing", new StateFleeing(this));
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(string state)
    {
        stateName = state;
        State previousState = currentState;
        currentState = states[state];
        if (previousState != currentState)
        {
            previousState?.ExitState();
            currentState?.EnterState();
        }
    }

}

/// <summary>
/// Character Idle State.
/// </summary>
public class StateIdle : State
{ 
    public StateIdle(StateManager sm) { manager = sm; }

    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update() 
    { 
        if(Time.time > manager.characterController.currentIdleWaitTime)
        {
            manager.characterController.currentTargetPosition = manager.characterController.gameManager.ReturnRandomPositionOnField();
            manager.ChangeState("Roaming");
        }

        if (manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Idle)
        {
            manager.characterController.animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; // Set our animation to idle animation.
        } 

    }
}

/// <summary>
/// Character Roaming State.
/// </summary>
public class StateRoaming : State
{
    public StateRoaming(StateManager sm) { manager = sm; }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update()
    {
        float distanceToTarget = 0;

        if (manager.characterController.currentSoccerBall != null)
        {
            distanceToTarget = manager.characterController.soccerBallInteractDistance;
        }
        else
        {
            distanceToTarget = manager.characterController.minDistanceToTarget;
        }

        if (manager.stateName == "Roaming" && Vector3.Distance(manager.characterController.transform.position, manager.characterController.CurrentTargetPosition) > distanceToTarget)
        {
            if (manager.characterController.currentSoccerBall != null)
            {
                if (manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
                {
                    manager.characterController.animationHandler.CurrentState = AnimationHandler.AnimationState.Running;
                }

                manager.characterController.CurrentTargetPosition = manager.characterController.currentSoccerBall.position;
                Vector3 targetPosition = new Vector3(manager.characterController.CurrentTargetPosition.x, manager.characterController.transform.position.y, manager.characterController.CurrentTargetPosition.z); 
                Vector3 nextMovePosition = Vector3.MoveTowards(manager.characterController.transform.position, targetPosition, manager.characterController.moveSpeed * Time.deltaTime * 1.5f); 
                manager.characterController.rigidBody.MovePosition(nextMovePosition);
                manager.characterController.currentIdleWaitTime = Time.time + manager.characterController.idleTime;
            }
            else
            {
                if (manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Walking)
                {
                    manager.characterController.animationHandler.CurrentState = AnimationHandler.AnimationState.Walking; 
                }

                Vector3 targetPosition = new Vector3(manager.characterController.CurrentTargetPosition.x, manager.characterController.transform.position.y, manager.characterController.CurrentTargetPosition.z); // The positon we want to move towards.
                Vector3 nextMovePosition = Vector3.MoveTowards(manager.characterController.transform.position, targetPosition, manager.characterController.moveSpeed * Time.deltaTime); // The amount we should move towards that position.
                manager.characterController.rigidBody.MovePosition(nextMovePosition);
                manager.characterController.currentIdleWaitTime = Time.time + manager.characterController.idleTime;
            }

        }
        else if (manager.stateName == "Roaming")
        {
            if(manager.characterController.currentSoccerBall != null)
            {
                manager.ChangeState("Playing");
                manager.characterController.currentTimeTillPassingAnimationPlays = Time.time + manager.characterController.passingAnimationDelay;
            }
            else
            {
                manager.ChangeState("Idle");
            }
        }

    }
}

/// <summary>
/// Character Waving State.
/// </summary>
public class StateWaving : State
{
    public StateWaving(StateManager sm) { manager = sm; }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update()
    { 
        if(manager.characterController.ReturnCharacterTransformToWaveAt() != null & manager.stateName != "Waving" && Time.time > manager.characterController.currentTimeBetweenWaves && manager.stateName != "Fleeing" && manager.characterController.currentSoccerBall == null)
        {
            manager.ChangeState("Waving");
            manager.characterController.currentWaveTime = Time.time + manager.characterController.waveTime;
            manager.characterController.CurrentTargetPosition = manager.characterController.ReturnCharacterTransformToWaveAt().position;
            if(manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Waving)
            {
                manager.characterController.animationHandler.currentAnimationState = AnimationHandler.AnimationState.Waving;
            }
        }
        if (manager.stateName == "Waving" && Time.time > manager.characterController.currentWaveTime)
        {
            manager.characterController.CurrentTargetPosition = manager.characterController.previousTargetPosition;
            manager.characterController.currentTimeBetweenWaves = Time.time + manager.characterController.timeBetweenWaves;
            manager.ChangeState("Roaming");
        }
    
    }
}

/// <summary>
/// Character Playing State.
/// </summary>
public class StatePlaying : State
{
    public StatePlaying(StateManager sm) { manager = sm; }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update()
    { 
        if(manager.stateName == "Playing")
        {
            if(manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Passing)
            {
                manager.characterController.animationHandler.CurrentState = AnimationHandler.AnimationState.Passing;
            }

            if(Time.time > manager.characterController.currentTimeTillPassingAnimationPlays)
            {
                manager.characterController.KickSoccerBall();
                manager.characterController.CurrentTargetPosition = manager.characterController.currentSoccerBall.position;
                manager.ChangeState("Roaming");
            }
        }
    
    }
}

/// <summary>
/// Character Fleeing State.
/// </summary>
public class StateFleeing : State
{
    public StateFleeing(StateManager sm) { manager = sm; }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update()
    { 
        if(manager.stateName != "Fleeing" && manager.characterController.gameManager.IsPlayerToCloseToCharacter(manager.characterController.transform, manager.characterController.distanceThresholdOfPlayer))
        {
            manager.ChangeState("Fleeing");
            manager.characterController.gameManager.RunningAwayFromPlayer(true);
            if (manager.characterController.animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
            {
                manager.characterController.animationHandler.CurrentState = AnimationHandler.AnimationState.Running;
            }
        }
        else if (manager.stateName == "Fleeing" && manager.characterController.gameManager.IsPlayerToCloseToCharacter(manager.characterController.transform, manager.characterController.distanceThresholdOfPlayer))
        {
            if (manager.stateName == "Fleeing" && Vector3.Distance(manager.characterController.transform.position, manager.characterController.CurrentTargetPosition) > manager.characterController.minDistanceToTarget)
            {
                Vector3 targetPosition = new Vector3(manager.characterController.CurrentTargetPosition.x, manager.characterController.transform.position.y, manager.characterController.CurrentTargetPosition.z); // The positon we want to move towards.
                Vector3 nextMovePosition = Vector3.MoveTowards(manager.characterController.transform.position, targetPosition, manager.characterController.moveSpeed * Time.deltaTime * 1.5f); // The amount we should move towards that position.
                manager.characterController.rigidBody.MovePosition(nextMovePosition);
            }
            else
            {
                manager.characterController.CurrentTargetPosition = manager.characterController.gameManager.ReturnRandomPositionOnField();
            }
        }
        else if (manager.stateName == "Fleeing" && manager.characterController.gameManager.IsPlayerToCloseToCharacter(manager.characterController.transform, manager.characterController.distanceThresholdOfPlayer) == false)
        {
            manager.ChangeState("Roaming");
            manager.characterController.currentTargetPosition = manager.characterController.gameManager.ReturnRandomPositionOnField();
            manager.characterController.gameManager.RunningAwayFromPlayer(false);
        }
    
    }
}

public class CharacterController : MonoBehaviour
{


    public StateManager stateManager = new StateManager(); // Reference to state manager.
    public State currentState;
    public GameManager gameManager; // Reference to our game manager.
    public Rigidbody rigidBody; // Reference to our rigidbody.

    // Roaming state variables.
    public Vector3 currentTargetPosition; // The target we are currently heading towards.
    public Vector3 previousTargetPosition; // The last target we were heading towards.

    public float moveSpeed = 3; // How fast our character is moving.
    public float minDistanceToTarget = 1; // How close we should get to our target.

    // Idle state variables.
    public float idleTime = 2; // Once we reach our target position, how long should we wait till we get another position.
    public float currentIdleWaitTime; // The time we are waiting till, we can move again.

    // Waving state varaiables.
    public float waveTime = 2; // The time spent waving.
    public float currentWaveTime; // The current time to wave till.
    public float distanceToStartWavingFrom = 4f; // The distance that will be checking to see if we are in range to wave at another character.
    public CharacterController[] allCharactersInScene; // A collection of references to all characters in our scene.
    public float timeBetweenWaves = 5; // The time between when we are allowed to wave again.
    public float currentTimeBetweenWaves; // The current time for our next wave to be iniated.

    // Fleeing state variables.
    public float distanceThresholdOfPlayer = 5;// The distance that is "to" close for the player to be to us.


    // Playing state variables
    public Transform currentSoccerBall = null; // A reference to the current soccerball.
    public GameObject selfIdentifier; // A reference to our identification colour.
    public GameObject myGoal; // Reference to this characters goal.
    public float soccerBallKickForce = 10; // The amount of force the character can use to kick the ball.
    public float soccerBallInteractDistance = 0.75f;// If the soccerball is close nough, then we can kick it.
    public float passingAnimationDelay = 0.5f; // A delay of the soccer animation before they kick.
    public float currentTimeTillPassingAnimationPlays; // The time at which the animation will play and we should kick.

    public AnimationHandler animationHandler; // A reference to our animation handler script.

    /// <summary>
    /// Returns the currentTargetPosition and sets the new current position. 
    /// </summary>
    public Vector3 CurrentTargetPosition
    {
        get
        {
            return currentTargetPosition; // Gets the current value.
        }
        set
        {
            previousTargetPosition = currentTargetPosition; // Assign our current position to our previous target position.
            currentTargetPosition = value; // Assign the new value to our current target position.
        }
    }

    // Called each time the script or the game object is disabled.
    private void OnDisable()
    {
        if(gameManager != null) // If the game is not null.
        {
            gameManager.RunningAwayFromPlayer(false); // Then tell it there are no characters in range.
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stateManager.Start();
        stateManager.characterController = this;
        CurrentTargetPosition = gameManager.ReturnRandomPositionOnField(); // Get a random starting position.
        allCharactersInScene = FindObjectsOfType<CharacterController>(); // Find the references to all characters in our scene.
        stateManager.stateName = "Roaming"; // Set the character by default to start roaming.
        selfIdentifier.SetActive(false);
        animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; // Set our animation to idle.
        stateManager.ChangeState("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.Update();
        LookAtTargetPosition();
    }


    /// <summary>
    /// Returns a transform if they are in range of the player to be waved at.
    /// </summary>
    /// <returns></returns>
    public Transform ReturnCharacterTransformToWaveAt()
    {
        // Looping through all the characters in our scene.
        for(int i = 0; i<allCharactersInScene.Length; i++)
        {
            // If the current element we are up to is not equal to this instance our character.
            if (allCharactersInScene[i] != this)
            {
                // Check the distance between them, if they are close enough return that other character.
                if(Vector3.Distance(transform.position, allCharactersInScene[i].transform.position) < distanceToStartWavingFrom)
                {                 
                    // But also let's return the character we should be waving at.
                    return allCharactersInScene[i].transform;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Rotates our character to always face the direction we are heading.
    /// </summary>
    public void LookAtTargetPosition()
    {
        Vector3 directionToLookAt = CurrentTargetPosition - transform.position; // Get the direction we should be lookin at.
        directionToLookAt.y = 0; // Don't change the Y position.
        Quaternion rotationOfDirection = Quaternion.LookRotation(directionToLookAt); // Get a rotation that we can use to look towards.
        transform.rotation = rotationOfDirection; // Set our current rotation to our rotation to face towards.
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a blue sphere on the position we are moving towards.
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CurrentTargetPosition, 0.5f);
    }

    /// <summary>
    /// Is called when the soccer ball is spawned in.
    /// </summary>
    /// <param name="SoccerBall"></param>
    public void SoccerBallSpawned(Transform SoccerBall)
    {
        currentSoccerBall = SoccerBall; // Assign the soccer ball to our reference.
        CurrentTargetPosition = currentSoccerBall.position; // Set our target position to our soccer ball.
        stateManager.stateName = "Roaming"; // Using our roaming state to start moving towards our soccerball.
        selfIdentifier.SetActive(true);
    }

    /// <summary>
    /// Handles Kicking the soccerball.
    /// </summary>
    public void KickSoccerBall()
    {
        Vector3 direction = myGoal.transform.position - transform.forward; // Get a directional vector that moves towards our goal post.
        currentSoccerBall.GetComponent<Rigidbody>().AddForce(direction * soccerBallKickForce * Random.Range(0.5f, 10f)); // Kick the ball towards our goal post. and add a little random force so the ball doesn't get stuck.
    }

}


