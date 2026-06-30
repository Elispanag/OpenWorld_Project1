using UnityEngine;
using System.Collections.Generic;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Environment Prefabs")]
    public GameObject[] environmentPrefabs;
    public GameObject heartPrefab;
    public GameObject starPrefab;

    [Header("The ground")]
    public GameObject groundObject;

    [Header("Spawn Limits (Safe Area)")]
    [Tooltip (" Min and Max values of X and Z where objects can be spawned")]
    public float minX = -125f;
    public float maxX = 125f;
    public float minZ = -125f;
    public float maxZ = 125f;


    [Header("Spawn Settings")]
    public int totalObjectsToSpawn = 200; 
    public int totalHearts = 10;
    public int totalStars = 10;

    public static List<Transform> activeStars =  new List<Transform>();

    void Start()
    {
        activeStars.Clear();
        SpawnEnvironment();
    }

    void SpawnEnvironment()
    {
        
        if (groundObject == null)
        {
            Debug.LogError("Ground object is not assigned in the EnvironmentSpawner script.");
            return;
        }
        Renderer groundRenderer = groundObject.GetComponent<Renderer>();
        Bounds bounds = groundRenderer.bounds;

        // Create an empty parent for better organization in the Hierarchy
        GameObject envParent = new GameObject("Spawned_Environment");

        if (environmentPrefabs != null && environmentPrefabs.Length> 0)
        {
            for (int i = 0; i < totalObjectsToSpawn; i++)
            {
                // Selects a random object from your list
                int randomIndex = Random.Range(0, environmentPrefabs.Length);
                GameObject prefabToSpawn = environmentPrefabs[randomIndex];

                // Generates random X and Z coordinates within the specified range
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);

                // Sets the Y position to the ground's height so the objects stand on the ground 
                Vector3 spawnPosition = new Vector3(randomX, bounds.max.y, randomZ);

                // Creates the object in the scene
                GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                
                // Gives it a random scale between 3.5 and 4.5
                float randomScale = Random.Range(3.5f, 4.5f);
                spawnedObj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);   
                // Gives it a random rotation 
                float randomYRotation = Random.Range(0f, 360f);
                spawnedObj.transform.Rotate(Vector3.up, randomYRotation);

                // Makes the spawned object a child of the parent object
                spawnedObj.transform.parent = envParent.transform;
            }
        }
        // Spawns hearts in the environment
        if(heartPrefab != null)
        {
            for (int i = 0; i < totalHearts; i++)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                Vector3 spawnPosition = new Vector3(randomX, bounds.max.y + 1.5f, randomZ);
                
                GameObject spawnedHeart = Instantiate(heartPrefab, spawnPosition, Quaternion.identity);
                spawnedHeart.tag = "Heart";
                spawnedHeart.transform.parent = envParent.transform;
            }
        }
        // Spawns stars in the environment
        if(starPrefab != null)
        {
            for (int i = 0; i < totalStars; i++)
            {
                float randomX = Random.Range(minX, maxX);
                float randomZ = Random.Range(minZ, maxZ);
                Vector3 spawnPosition = new Vector3(randomX, bounds.max.y + 0.5f, randomZ);
                
                GameObject spawnedStar = Instantiate(starPrefab, spawnPosition, Quaternion.identity);
                spawnedStar.tag = "Star";
                spawnedStar.transform.parent = envParent.transform;
                
                activeStars.Add(spawnedStar.transform);
            }
        }
    }
}