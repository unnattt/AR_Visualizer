using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDemo : MonoBehaviour
{
    [SerializeField] private GameObject object1;
    [SerializeField] private GameObject object2;

    void Start()
    {
        Invoke(nameof(HideObject), 2f);
    }

    public void HideObject()
    {
        object1.SetActive(false);
        object2.SetActive(true);
         
    }
}
