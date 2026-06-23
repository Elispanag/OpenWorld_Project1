using UnityEngine;
using UnityEngine.AI; //For NavMeshAgent movement
using TMPro;

public class SmartAI : MonoBehaviour
{
    // --- REFERENCES ---
    private NavMeshAgent agent;
    public Transform playerTransform;   // The position of the player
    public ResourceNode resourceNode; // Reference to the resource node
    public Transform restZone;         // The position of the rest area
    public TextMeshProUGUI debugText; //Connection to the 3D text on the screen

    // --- AI STATS (Internal state of the AI) ---
    [Header("AI Stats")]
    public float energy = 100f;        // The energy of the NPC (starts full)
    public float wealth = 0f;          // How many resources has collected
    public float detectionRadius = 6f; // How close the player needs to be for the NPC to flee

    // enumeration for the states of the NPC
    private enum AIState { Gathering, Resting, Fleeing }
    [SerializeField] private AIState currentState = AIState.Gathering;

    void Start()
    {
        // We get the NavMeshAgent component that we attached to the object
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Decrease energy over time
        DecreaseEnergy();

        // Main Decision Making Mechanism (Decision Making / State Machine)
        EvaluateSituation();

        // Execute the decision
        ExecuteCurrentState();
    }

    // Decreases the energy of the NPC while it is moving
    void DecreaseEnergy()
    {
        if (currentState != AIState.Resting)
        {
            // Loses 3 units of energy per second
            energy -= Time.deltaTime * 3f; 
        }
    }

    // Evaluates the environment and changes the NPC's state
    void EvaluateSituation()
    {
        // EVALUATION 1: Player proximity (Safety)
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < detectionRadius)
        {
            currentState = AIState.Fleeing;
            return; // Stops evaluating further conditions, safety takes precedence!
        }

        // EVALUATION 2: Energy level (Resting)
        if (energy <= 20f && currentState != AIState.Resting)
        {
            currentState = AIState.Resting;
            return;
        }

        // EVALUATION 3: Energy level (Gathering)
        if (energy >= 100f && currentState == AIState.Resting)
        {
            currentState = AIState.Gathering;
            return;
        }
        // EVALUATION 4 : Resource availability (Gathering)
        if (currentState == AIState.Gathering && resourceNode.IsEmpty())
        {
            // If the resource node is empty and doesn"t need sleeping, the NPC will rest
            currentState = AIState.Resting; // Currently sending the NPC to the base to rest.
            return;
        }

        // If it's not working and not resting, it gathers resources
        if (currentState != AIState.Resting && currentState != AIState.Fleeing)
        {
            currentState = AIState.Gathering;
        }
    }

    // Executes the corresponding movement on the NavMesh based on the current state
    void ExecuteCurrentState()
    {
        switch (currentState)
        {
            case AIState.Gathering:
                debugText.text = " Status: Gathering\nWealth NPC: " + Mathf.Round(wealth) + "\nResources Left: " + Mathf.Round(resourceNode.currentResources);
                // The NPC moves autonomously towards the resource area
                agent.SetDestination(resourceNode.transform.position);
                
                // If it arrives, it gathers resources
                if (!agent.pathPending && agent.remainingDistance < 1.5f && !resourceNode.IsEmpty())
                {
                    float gathered = resourceNode.Harvest(Time.deltaTime * 5f);
                    wealth += gathered;
                }
                break;

            case AIState.Resting:
                // Update the UI
                debugText.text = " Status: Resting\nEnergy: " + Mathf.Round(energy);
                // The NPC goes to the rest zone
                agent.SetDestination(restZone.position);
                
                // If it arrives, it replenishes its energy
                if (!agent.pathPending && agent.remainingDistance < 1.5f)
                {
                    energy += Time.deltaTime * 15f; // Replenishes quickly
                }
                break;

            case AIState.Fleeing:
                // Update the UI
                debugText.text = " Status: Fleeing\nEnergy: " + Mathf.Round(energy);
                // Calculate the opposite direction from the player (Fleeing)
                Vector3 dirToPlayer = transform.position - playerTransform.position;
                Vector3 fleeTarget = transform.position + dirToPlayer.normalized * 5f;
                
                // Give the NavMeshAgent a destination to run to
                agent.SetDestination(fleeTarget);
                break;
        }
    }
}