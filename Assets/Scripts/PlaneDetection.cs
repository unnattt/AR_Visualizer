using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneDetection : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public float minPlaneSize = 0.5f; // Minimum size of the plane
    public ObjectTouchDetection modelToMove; // Reference to the model you want to move
    public float delayBetweenPositions = 2.0f; // Delay between model movements

   [SerializeField] private TMP_Text scoreText;
private int score = 0;

    private Vector3[] randomPoints = new Vector3[2];
    private int pointsFound = 0;

    void Awake()
    {
        planeManager.planesChanged += OnPlanesChanged;
        modelToMove.SetObjectPlacement(this);
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (pointsFound < 2)
        {
            foreach (var plane in args.added)
            {
                CheckAndPlaceObjects(plane);
            }

            foreach (var plane in args.updated)
            {
                CheckAndPlaceObjects(plane);
            }
        }
        else
        {
            // Disable the plane manager and planes rendering
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            // Start moving the model
            StartCoroutine(MoveModelBetweenPoints());
        }
    }
  

    void CheckAndPlaceObjects(ARPlane plane)
{
    if (plane.size.x >= minPlaneSize && plane.size.y >= minPlaneSize && pointsFound == 0)
    {
        Vector3 center = plane.center;

        // Generate two random points near the center of the same plane
        randomPoints[0] = center + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        randomPoints[1] = center + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));

        pointsFound = 2; // Update points found to 2 as we have found both points

        // After finding both points, disable plane detection
        DisablePlaneDetection();
    }
}

void DisablePlaneDetection()
{
    // Disable the plane manager and planes rendering
    planeManager.enabled = false;
    foreach (var plane in planeManager.trackables)
    {
        plane.gameObject.SetActive(false);
    }

    // Start moving the model
    StartCoroutine(MoveModelBetweenPoints());
}

    IEnumerator MoveModelBetweenPoints()
    {
        int currentPoint = 0;

        while (true)
        {
            // Move the model to the next point
            modelToMove.ScaleUp();
            modelToMove.transform.position = randomPoints[currentPoint];

            // Wait for the specified delay
            yield return new WaitForSeconds(delayBetweenPositions);
            // modelToMove.GetComponent<ObjectTouchDetection>().ScaleDown();
            // Switch to the other point
            currentPoint = (currentPoint + 1) % 2;
            // modelToMove.GetComponent<ObjectTouchDetection>().ScaleUp();

        }
    }

    public void RepositionModelRandomly()
    {
        if (randomPoints.Length < 2)
        {
            Debug.LogWarning("Not enough points to reposition model.");
            return;
        }

        // Select one of the two points at random
        Vector3 newPosition = randomPoints[Random.Range(0, 2)];
        // modelToMove.GetComponent<ObjectTouchDetection>().ScaleUp();
        modelToMove.transform.position = newPosition;
    }

     public void ObjectTouched(ObjectTouchDetection touchedObject)
    {
        // Destroy the touched object
        // Destroy(touchedObject);
        // touchedObject.SetActive(false);
        touchedObject.ScaleDown();

        Invoke(nameof(RepositionModelRandomly), 0.3f);
        // Increment the score
        score++;
        scoreText.text = "Score: " + score;         
        // Reset the placement flag to spawn another object
        // hasPlacedObject = false;
    }

    void OnDestroy()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }
}
