using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    State currentState = null;
    public string stateName = "";
    public CharacterController characterController;

    public void Start()
    {
        states.Add("Idle", new StateIdle());
        states.Add("Roaming", new StateRoaming());
        states.Add("Waving", new StateWaving());
        states.Add("Playing", new StatePlaying());
        states.Add("Fleeing", new StateFleeing());
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
    public StateIdle() { }

    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update() { }
}
/// <summary>
/// Character Roaming State.
/// </summary>
public class StateRoaming : State
{
    CharacterController characterController;
    public StateRoaming() { }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update()
    {
        
    }
}
/// <summary>
/// Character Waving State.
/// </summary>
public class StateWaving : State
{
    public StateWaving() { }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update() { }
}
/// <summary>
/// Character Playing State.
/// </summary>
public class StatePlaying : State
{
    public StatePlaying() { }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update() { }
}
/// <summary>
/// Character Fleeing State.
/// </summary>
public class StateFleeing : State
{
    public StateFleeing() { }
    public override void EnterState() { }
    public override void ExitState() { }
    public override void Update() { }
}

public class CharacterController : MonoBehaviour
{

    /// <summary>
    /// The different states that our character can be in
    /// </summary>
    public enum CharacterStates {Idle, Roaming, Waving, Playing, Fleeing}

    public CharacterStates currentCharacterState; // The current state that our character is in.



    public StateManager stateManager = new StateManager(); // Reference to state manager.
    public GameManager gameManager; // Reference to our game manager.
    public Rigidbody rigidBody; // Reference to our rigidbody.

    // Roaming state variables.
    private Vector3 currentTargetPosition; // The target we are currently heading towards.
    private Vector3 previousTargetPosition; // The last target we were heading towards.

    public float moveSpeed = 3; // How fast our character is moving.
    public float minDistanceToTarget = 1; // How close we should get to our target.

    // Idle state variables.
    public float idleTime = 2; // Once we reach our target position, how long should we wait till we get another position.
    private float currentIdleWaitTime; // The time we are waiting till, we can move again.

    // Waving state varaiables.
    public float waveTime = 2; // The time spent waving.
    private float currentWaveTime; // The current time to wave till.
    public float distanceToStartWavingFrom = 4f; // The distance that will be checking to see if we are in range to wave at another character.
    private CharacterController[] allCharactersInScene; // A collection of references to all characters in our scene.
    public float timeBetweenWaves = 5; // The time between when we are allowed to wave again.
    private float currentTimeBetweenWaves; // The current time for our next wave to be iniated.

    // Fleeing state variables.
    public float distanceThresholdOfPlayer = 5;// The distance that is "to" close for the player to be to us.


    // Playing state variables
    private Transform currentSoccerBall = null; // A reference to the current soccerball.
    public GameObject selfIdentifier; // A reference to our identification colour.
    public GameObject myGoal; // Reference to this characters goal.
    public float soccerBallKickForce = 10; // The amount of force the character can use to kick the ball.
    public float soccerBallInteractDistance = 0.25f;// If the soccerball is close nough, then we can kick it.
    public float passingAnimationDelay = 0.5f; // A delay of the soccer animation before they kick.
    private float currentTimeTillPassingAnimationPlays; // The time at which the animation will play and we should kick.

    public AnimationHandler animationHandler; // A reference to our animation handler script.

    /// <summary>
    /// Returns the currentTargetPosition and sets the new current position. 
    /// </summary>
    private Vector3 CurrentTargetPosition
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
        currentCharacterState = CharacterStates.Roaming; // Set the character by default to start roaming.
        selfIdentifier.SetActive(false);
        animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; // Set our animation to idle.
    }

    // Update is called once per frame
    void Update()
    {
        stateManager.Update();
    }

    /// <summary>
    /// Handles the Roaming state of our character.
    /// </summary>
    private void HandleRoamingState()
    {
        float distanceToTarget = 0;

        if (currentSoccerBall != null)
        {
            distanceToTarget = soccerBallInteractDistance;
        }
        else
        {
            distanceToTarget = minDistanceToTarget;
        }

        /// If we are still too far away move closer.
        if (currentCharacterState == CharacterStates.Roaming && Vector3.Distance(transform.position, CurrentTargetPosition) > distanceToTarget)
        {
            if(currentSoccerBall != null)
            {
                // When running.
                if (animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
                {
                    animationHandler.CurrentState = AnimationHandler.AnimationState.Running; // Set our animation to running animation.
                }

                CurrentTargetPosition = currentSoccerBall.position;
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // The positon we want to move towards.
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime * 1.5f); // The amount we should move towards that position.
                rigidBody.MovePosition(nextMovePosition);
                currentIdleWaitTime = Time.time + idleTime;
            }
            else
            {
                // When walking.
                if (animationHandler.CurrentState != AnimationHandler.AnimationState.Walking)
                {
                    animationHandler.CurrentState = AnimationHandler.AnimationState.Walking; // Set our animation to walking animation.
                }
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // The positon we want to move towards.
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime); // The amount we should move towards that position.
                rigidBody.MovePosition(nextMovePosition);
                currentIdleWaitTime = Time.time + idleTime;
            }
          
        }
        else if (currentCharacterState == CharacterStates.Roaming) // So check to see if we're roaming.
        {
            if (currentSoccerBall != null)
            {
                currentCharacterState = CharacterStates.Playing; // Start playing with the ball.
                currentTimeTillPassingAnimationPlays = Time.time + passingAnimationDelay; // Sets the time to wait till until we play the animation.
            }
            else
            {
                currentCharacterState = CharacterStates.Idle; // Start idling.

            }
        }
    }

    /// <summary>
    /// Handle the idle state of our character.
    /// </summary>
    private void HandleIdleState()
    {
        if(currentCharacterState == CharacterStates.Idle)
        {
            // We must be close enough to our target position.
            // We wait a couple seconds.
            // Then find a new position to move to.
            if (Time.time > currentIdleWaitTime)
            {
                // Lets find a new position.
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
                currentCharacterState = CharacterStates.Roaming; // Start roaming again.
            }
            // When running.
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Idle)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Idle; // Set our animation to idle animation.
            }
        }
    }

    /// <summary>
    /// Handles the fleeing state of our character.
    /// </summary>
    private void HandleFleeingState()
    {
        if (currentCharacterState != CharacterStates.Fleeing && gameManager.IsPlayerToCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // When fleeing.
            currentCharacterState = CharacterStates.Fleeing;
            gameManager.RunningAwayFromPlayer(true); // We are fleeing from the player play our music.
            // When running.
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Running)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Running; // Set our animation to running animation.
            }

        }
        else if (currentCharacterState == CharacterStates.Fleeing && gameManager.IsPlayerToCloseToCharacter(transform, distanceThresholdOfPlayer))
        {
            // If we are still too far away move closer.
            if (currentCharacterState == CharacterStates.Fleeing && Vector3.Distance(transform.position, CurrentTargetPosition) > minDistanceToTarget)
            {
                Vector3 targetPosition = new Vector3(CurrentTargetPosition.x, transform.position.y, CurrentTargetPosition.z); // The positon we want to move towards.
                Vector3 nextMovePosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime * 1.5f); // The amount we should move towards that position.
                rigidBody.MovePosition(nextMovePosition);
            }
            else
            {
                CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
            }

        }
        else if (currentCharacterState == CharacterStates.Fleeing && gameManager.IsPlayerToCloseToCharacter(transform, distanceThresholdOfPlayer) == false)
        {
            // If we are still fleeing, then we want to transition back to our roaming state.
            currentCharacterState = CharacterStates.Roaming;
            CurrentTargetPosition = gameManager.ReturnRandomPositionOnField();
            gameManager.RunningAwayFromPlayer(false); // Stop fleeing from the player so go back to the original music.
          
        }
    }

    /// <summary>
    /// Handles the playing state of our character.
    /// </summary>
    private void HandlePlayingState()
    {
        // We want to kick the ball cause are close enough.
        if(currentCharacterState == CharacterStates.Playing)
        {
            // Here would should be running.
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Passing)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Passing; // Set our animation to running animation.
            }

            if (Time.time > currentTimeTillPassingAnimationPlays)
            {
                KickSoccerBall(); // Kick our ball.
                // Set our target to the soccer ball again, and start moving towards the ball again.
                CurrentTargetPosition = currentSoccerBall.position; 
                currentCharacterState = CharacterStates.Roaming;
            }        
        }
    }

    /// <summary>
    /// Handles the waving state.
    /// </summary>
    private void HandleWavingState()
    {
        if (ReturnCharacterTransformToWaveAt() != null && currentCharacterState != CharacterStates.Waving && Time.time > currentTimeBetweenWaves && currentCharacterState != CharacterStates.Fleeing && currentSoccerBall == null)
        {
            // We should start waving.
            currentCharacterState = CharacterStates.Waving;
            currentWaveTime = Time.time + waveTime; // Set up the time we should be waving till.
            CurrentTargetPosition = ReturnCharacterTransformToWaveAt().position; // Set the current target position  to the closest transform, so that way we also rotate towards it.
            // Here would should be Waving.
            if (animationHandler.CurrentState != AnimationHandler.AnimationState.Waving)
            {
                animationHandler.CurrentState = AnimationHandler.AnimationState.Waving; // Set our animation to waving animation.
            }
        }
        if (currentCharacterState == CharacterStates.Waving && Time.time > currentWaveTime)
        {
            // Stop waving.
            CurrentTargetPosition = previousTargetPosition; // Resume moving towards our random target position.
            currentTimeBetweenWaves = Time.time + timeBetweenWaves; // Set the next time for when we can wave again.
            currentCharacterState = CharacterStates.Roaming; // Start roaming again.
        }

    }

    /// <summary>
    /// Returns a transform if they are in range of the player to be waved at.
    /// </summary>
    /// <returns></returns>
    private Transform ReturnCharacterTransformToWaveAt()
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
    private void LookAtTargetPosition()
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
        currentCharacterState = CharacterStates.Roaming; // Using our roaming state to start moving towards our soccerball.
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
