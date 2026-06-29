using UnityEngine;
using UnityEngine.AI; //For NavMeshAgent movement
using System.Collections.Generic; //For using lists
using TMPro;

public class SmartAI : MonoBehaviour
{
    // --- REFERENCES ---
    private NavMeshAgent agent;
    public Transform playerTransform;   // The position of the player
    public ResourceNode resourceNode; // Reference to the resource node

    [Header("Rest Settings")]
    public List<Transform> restZones = new List<Transform>(); // List of possible rest zones
    public TextMeshProUGUI debugText; //Connection to the 3D text on the screen

    // --- AI STATS ---
    [Header("AI Stats")]
    public float speed = 6f;          // The speed of the NPC
    public float energy = 100f;        // The energy of the NPC (starts full)
    public float wealth = 0f;          // How many resources has collected
    public float detectionRadius = 4f; // How close the player needs to be for the NPC to flee

    // --- INTERNAL TRACKING ---
    private Transform targetStar; // The closest star to the NPC
    private Transform targetRestZone;   // The closest rest zone to the NPC

    // enumeration for the states of the NPC
    private enum AIState { Gathering, Resting, Fleeing }
    [SerializeField] private AIState currentState = AIState.Gathering;

    void Start()
    {
        // We get the NavMeshAgent component that we attached to the object
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    void Update()
    {
        // Decrease energy over time
        DecreaseEnergy();

        // Main Decision Making Mechanism 
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

        // Player proximity (Safety)
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < detectionRadius)
            {
                currentState = AIState.Fleeing;
                return; // Stops evaluating further conditions, safety takes precedence!
            }
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned in the SmartAI script.");
        }

        // Energy level (Resting)
        if (energy <= 20f && currentState != AIState.Resting)
        {
            currentState = AIState.Resting;
            FindClosestRestZone();
            return;
        }

        // Energy level (Gathering)
        if (energy >= 100f && currentState == AIState.Resting)
        {
            currentState = AIState.Gathering;
            return;
        }
        // Resource availability (Gathering)
        if (currentState == AIState.Gathering && EnvironmentSpawner.activeStars.Count == 0 && (resourceNode == null || resourceNode.IsEmpty()))
        {
            // If the resource node is empty and doesn"t need sleeping, the NPC will rest
            currentState = AIState.Resting; // Currently sending the NPC to the base to rest.
            FindClosestRestZone();
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
                FindClosestStar();

                if(targetStar != null)
                {
                    debugText.text = "Status: Hunting Stars\nCollected: " + Mathf.Round(wealth) + "\nStars Left: " + EnvironmentSpawner.activeStars.Count;
                    agent.SetDestination(targetStar.position);
                }
                else if (resourceNode != null && !resourceNode.IsEmpty())
                {
                    debugText.text = "Status: Harvesting Tree\nCollected: " + Mathf.Round(wealth) + "\nTree Resources: " + Mathf.Round(resourceNode.currentResources);
                    agent.SetDestination(resourceNode.transform.position);

                    if (!agent.pathPending && agent.remainingDistance < 1.8f)
                    {
                        float gathered = resourceNode.Harvest(Time.deltaTime * 5f);
                        wealth += gathered;
                        GameManager.Instance.CheckWinCondition(0, wealth); 
                    }
                }
                break;

            case AIState.Resting:
                 
                debugText.text = " Status: Heading to Rest Zone\nEnergy: " + Mathf.Round(energy);
                // If the NPC has a target rest zone, it will move towards it
                if (targetRestZone != null)
                {
                    agent.SetDestination(targetRestZone.position);
                }
                // If the NPC has reached the rest zone, it will start to regain energy
                if(!agent.pathPending && agent.remainingDistance < 1.5f)
                {
                
                    debugText.text = " Status: Resting\nEnergy: " + Mathf.Round(energy);
                    energy += Time.deltaTime * 15f; 
                }
                break;
                
            case AIState.Fleeing:
                // Update the UI
                debugText.text = " Status: Fleeing\nEnergy: " + Mathf.Round(energy);
                if (playerTransform != null)
                {
                    // Calculate the opposite direction from the player (Fleeing)
                    Vector3 dirToPlayer = transform.position - playerTransform.position;
                    Vector3 fleeTarget = transform.position + dirToPlayer.normalized * 6f;
                    
                    // Give the NavMeshAgent a destination to run to
                    agent.SetDestination(fleeTarget);
                }
                break;
        }
    }
    void FindClosestStar()
    {
        if (EnvironmentSpawner.activeStars.Count == 0)
        {
            targetStar = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestStar = null;

        foreach (Transform star in EnvironmentSpawner.activeStars)
        {
            if (star == null) continue; // Skip if the star has been destroyed
            float distance = Vector3.Distance(transform.position, star.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestStar = star;
            }
        }
        targetStar = closestStar;
    }

    void FindClosestRestZone()
    {
        if (restZones == null || restZones.Count == 0)
        {
            targetRestZone = null;
            return;
        }

        float closestDistance = Mathf.Infinity;
        Transform closestRest = null;

        foreach (Transform zone in restZones)
        {
            if (zone == null) continue; // Skip if the rest zone has been destroyed
            float distance = Vector3.Distance(transform.position, zone.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRest = zone;
            }
        }
        targetRestZone = closestRest;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the NPC collides with a star
        if (other.CompareTag("Star"))
        {
           wealth++;
           if (EnvironmentSpawner.activeStars.Contains(other.transform))
           {
               EnvironmentSpawner.activeStars.Remove(other.transform);
           }
           GameManager.Instance.CheckWinCondition(0, wealth); // Check win condition for the NPC
           Destroy(other.gameObject);
        }
    }
}