using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RandomPlacementOnPlane : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject objectToPlacePrefab;
   [SerializeField] private TMP_Text scoreText;

   [SerializeField] private List<Vector3> storePositions; 
    List<ARPlane> planes = new List<ARPlane>();
    ARPlane currentPlane;
   Vector3 randomPosition;
    private int score = 0;
    private GameObject placedObject;
    private float minWaitTime = 3f; // Minimum time to wait before repositioning
    private float maxWaitTime = 5f; // Maximum time to wait before repositioning

    void Start()
    {
        StartCoroutine(RepositionObjectRoutine());
    }

    IEnumerator RepositionObjectRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            RepositionObject();
        }
    }

   public void RepositionObject()
    {
        if (planeManager.trackables.count == 0) return;

if(planes.Count < 1)
{
        ARPlane randomPlane = GetRandomPlane();        
        // ARPlane randomPlane = GetLargestPlane();
        planes.Add(randomPlane);
        currentPlane = randomPlane;
}
else
{
    currentPlane = planes[0];
}

// if(planes[0].size.x > 0.5f || planes[0].size.y > 0.5f)
// {
//     planeManager.enabled = false;
// }
        
        if (currentPlane == null) return;
        
        if(storePositions.Count < 2)
        {
           randomPosition = GetRandomPositionInPlane(currentPlane); 
           storePositions.Add(randomPosition);  
        }
        else
        {
            randomPosition = storePositions[Random.Range(0, storePositions.Count)];
            // storePositions.Remove(randomPosition);            
        }
           PlaceObject();        

        
    }

    void PlaceObject()
    {
        if (placedObject == null)
        {
            placedObject = Instantiate(objectToPlacePrefab, randomPosition, Quaternion.identity);
            placedObject.GetComponent<ObjectTouchDetection>().ScaleUp();
            // placedObject.GetComponent<ObjectTouchDetection>().SetObjectPlacement(this);
        }
        else
        {

            SetRandomPos();
            // placedObject.GetComponent<ObjectTouchDetection>().ScaleUp();           
            // placedObject.GetComponent<ObjectTouchDetection>().SetObjectPlacement(this);
        }
    }
async void SetRandomPos()
{
    placedObject.GetComponent<ObjectTouchDetection>().ScaleDown();      
    await System.Threading.Tasks.Task.Delay(300);    
    placedObject.transform.position = randomPosition;
    placedObject.GetComponent<ObjectTouchDetection>().ScaleUp();           
    // placedObject.GetComponent<ObjectTouchDetection>().SetObjectPlacement(this);            
}

     public void ObjectTouched(GameObject touchedObject)
    {
        // Destroy the touched object
        Destroy(touchedObject);

        // Increment the score
        score++;
        scoreText.text = "Score: " + score;         
        // Reset the placement flag to spawn another object
        // hasPlacedObject = false;

        // Spawn a new object on another random plane
        // Update();
        placedObject = null;
        // randomPosition = Vector3.zero;
        currentPlane = null;
        // RepositionObject();
        // StartCoroutine(RepositionObjectRoutine());
    }

    ARPlane GetRandomPlane()
    {
        List<ARPlane> planes = new List<ARPlane>();
        foreach (var plane in planeManager.trackables)
        {
            planes.Add(plane);
        }

        if (planes.Count == 0) return null;
        int randomIndex = Random.Range(0, planes.Count);
        return planes[randomIndex];
    }

    ARPlane GetLargestPlane()
{
    List<ARPlane> planes = new List<ARPlane>();

    if (planes.Count == 0) return null;

    ARPlane largestPlane = planes[0];
    float largestSize = largestPlane.size.x * largestPlane.size.y; // Assuming size is in the X-Y plane

    // Iterate through the planes to find the largest one
    for (int i = 1; i < planes.Count; i++)
    {
        float currentSize = planes[i].size.x * planes[i].size.y;

        if (currentSize > largestSize)
        {
            largestPlane = planes[i];
            largestSize = currentSize;
        }
    }

    return largestPlane;
}

    Vector3 GetRandomPositionInPlane(ARPlane plane)
    {
        Vector3 center = plane.center;
        Vector2 size = plane.size;

        float randomX = Random.Range(center.x - size.x / 4, center.x + size.x / 4);
        float randomY = center.y; // Assuming Y is up
        float randomZ = Random.Range(center.z - size.y / 4, center.z + size.y / 4);

        return plane.transform.TransformPoint(new Vector3(randomX, randomY, randomZ));
    }
}
