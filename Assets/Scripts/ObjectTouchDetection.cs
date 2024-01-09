using UnityEditor;
using UnityEngine;

public class ObjectTouchDetection : MonoBehaviour
{
    public Animation animation1;
    private PlaneDetection objectPlacement;

    public void SetObjectPlacement(PlaneDetection placement)
    {
        objectPlacement = placement;
    }

    void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Raycast to detect if the touch is on this object
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                // Notify the ObjectPlacement script that this object is touched
                // animation.Play();
                ScaleDown();
                DestroyObject();
            }
        }
    }
    public void ScaleUp()
    {
        animation1.Play("Scale-Up");
    }

    public void ScaleDown()
    {
        animation1.Play("Scale-Down");
    }

    async void DestroyObject()
    {
        await System.Threading.Tasks.Task.Delay(500);
        objectPlacement.ObjectTouched(this);        
    }
}
