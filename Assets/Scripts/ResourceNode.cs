using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    //Maximum and current amount of resources
    public float maxResources = 100f;
    public float currentResources;

    //Starting scale of cube
    private Vector3 originalScale;

    void Start()
    {
        currentResources = maxResources;
        originalScale = transform.localScale;

    }
    
    void Update()
    {
        // If the resource node is empty, it slowly replenishes over time
        if (currentResources <= 0)
        {
            // Slowly replenishes resources over time
            currentResources += Time.deltaTime * 2f;
            // Changes the color back to white when replenishing
            GetComponent<Renderer>().material.color = Color.white;
            float resourcePercentage = currentResources / maxResources;
            transform.localScale = originalScale * Mathf.Max(resourcePercentage, 0.1f);
            // If it is completely replenished, it stops
            if (currentResources >= maxResources)
            {
                currentResources = maxResources;
                Debug.Log(gameObject.name + " fully replenished!");
            }
        }
    }

    // It is called when someone steals resources
    public float Harvest(float amount)
    {   // If there are no resources left, return 0
        if (currentResources <= 0) return 0;
        // Reduce the amount of resources
        currentResources -= amount;
        float resourcePercentage = currentResources / maxResources;
        transform.localScale = originalScale * Mathf.Max(resourcePercentage, 0.1f); // Prevents the scale from going to 0 
        
        // If it is empty, change the color to grey
        if (currentResources <= 0)
        {
            currentResources = 0;
            GetComponent<Renderer>().material.color = Color.gray;
            Debug.Log(gameObject.name + " is empty!");
        }


        return amount; // Returns the amount of resources taken
    }

    // Returns true if the resource node is empty
    public bool IsEmpty()
    {
        return currentResources <= 0;
    }

}
