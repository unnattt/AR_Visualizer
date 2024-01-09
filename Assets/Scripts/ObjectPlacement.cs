using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private int score = 0;
    public GameObject objectToPlace;
    private ARPlaneManager planeManager;
    private bool hasPlacedObject = false;
    private float repositionTimer = 0f;
    private float repositionInterval = 3f; // Change this value to set the time interval for repositioning
    private ARPlane currentPlane;

    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        ResetRepositionTimer();
    }

    void Update()
    {
        scoreText.text = "Score: " + score;

        repositionTimer -= Time.deltaTime;

        if (repositionTimer <= 0f)
        {
            RepositionObject();
            ResetRepositionTimer();
        }

        if (!hasPlacedObject && planeManager.trackables.count > 0)
        {
            // Get a random detected plane
            ARPlane randomPlane = GetRandomPlane();

            if (randomPlane != null)
            {
                currentPlane = randomPlane;
                Vector3 hitPosition = randomPlane.center;
                Quaternion hitRotation = Quaternion.identity;

                // Instantiate object on the random plane
                GameObject placedObject = Instantiate(objectToPlace, hitPosition, hitRotation);

                // Attach a script to the instantiated object for touch detection
                // placedObject.GetComponent<ObjectTouchDetection>().SetObjectPlacement(this);

                hasPlacedObject = true; // Set the flag to true to prevent further instantiations
            }
        }

        if (currentPlane != null)
        {
            // Randomly reposition the object within the current plane
            objectToPlace.transform.position = GetRandomPointInPlane(currentPlane);
        }
    }

    public void ObjectTouched(GameObject touchedObject)
    {
        // Destroy the touched object
        Destroy(touchedObject);

        // Increment the score
        score++;

        // Reset the placement flag to spawn another object
        hasPlacedObject = false;

        // Spawn a new object on another random plane
        Update();
    }

    private ARPlane GetRandomPlane()
    {
        // Get all detected planes
        List<ARPlane> allPlanes = new List<ARPlane>();
        foreach (ARPlane plane in planeManager.trackables)
        {
            allPlanes.Add(plane);
        }

        // Check if there are any planes
        if (allPlanes.Count > 0)
        {
            // Get a random index
            int randomIndex = Random.Range(0, allPlanes.Count);

            // Return the random plane
            return allPlanes[randomIndex];
        }

        // Return null if no planes are available
        return null;
    }

    private void RepositionObject()
    {
        // Randomly reposition the object within the current plane
        if (currentPlane != null)
        {
            objectToPlace.transform.position = GetRandomPointInPlane(currentPlane);
        }
    }

    private Vector3 GetRandomPointInPlane(ARPlane plane)
    {
        // Get a random point within the bounds of the plane
        Vector3 planeCenter = plane.center;
        Vector2 planeExtents = plane.extents;

        float randomX = Random.Range(-planeExtents.x, planeExtents.x);
        float randomZ = Random.Range(-planeExtents.y, planeExtents.y);

        return planeCenter + new Vector3(randomX, 0f, randomZ);
    }

    private void ResetRepositionTimer()
    {
        // Reset the reposition timer
        repositionTimer = repositionInterval;
    }
}
