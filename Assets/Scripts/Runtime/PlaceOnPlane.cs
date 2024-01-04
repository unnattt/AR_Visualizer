using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : PressInputBase
    {
        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        GameObject m_PlacedPrefab;

        //[SerializeField]
        //private Dictionary<string, GameObject> prefabDictionary;      

       [SerializeField]
       GameObject text;
       

        [SerializeField]
        GameObject[] prefabs;

        [SerializeField]
        private InputAction swipeAction;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject spawnedObject { get; private set; }

        bool m_Pressed;        
        private Vector2 m_SwipeDelta;

        PlaneDetectionController m_PlaneDetection;
        protected override void Awake()
        {
            base.Awake();
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_PlaneDetection = GetComponent<PlaneDetectionController>();
            swipeAction.performed += OnSwipe;
            swipeAction.Enable();
            isGameStarted = true;
        }



        public void ChangePrefabOnButtonPress(int num)
        {
            m_PlacedPrefab = prefabs[num];
            m_PlaneDetection.SetAllPlanesActive(true);
            text.SetActive(true);           
            // Destroy the current spawned object if it exists
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
                spawnedObject = null;
            }
            else
            {
                //Debug.LogError("Prefab not found for button: " + buttonName);
            }
        }


        void Update()
        {
            if (IsPointerOverUI())
            {

                isGameStarted = false;
            }
            else
            {
                isGameStarted = true;
            }            
           
            if (isGameStarted)
            {
                if (Pointer.current == null || m_Pressed == false)
                    return;

                var touchPosition = Pointer.current.position.ReadValue();
                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    // Raycast hits are sorted by distance, so the first one
                    // will be the closest hit.
                    var hitPose = s_Hits[0].pose;

                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation);
                        m_PlaneDetection.SetAllPlanesActive(false);
                        text.SetActive(false);
                    }
                    //else
                    //{
                    //    spawnedObject.transform.position = hitPose.position;
                    //}
                }
                RotateOnSwipe();
            }
        }


        private bool IsPointerOverUI()
        {
            // Check if any UI element is under the pointer
            return EventSystem.current.IsPointerOverGameObject();
        }

        void OnSwipe(InputAction.CallbackContext context)
        {
            m_SwipeDelta = context.ReadValue<Vector2>();
        }

        void RotateOnSwipe()
        {
            if (m_SwipeDelta.x > 0.1f) // Swipe right
            {
                spawnedObject.transform.Rotate(Vector3.up, -Time.deltaTime * rotationSpeed);
            }
            else if (m_SwipeDelta.x < -0.1f) // Swipe left
            {
                spawnedObject.transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed);
            }
        }

        protected override void OnPress(Vector3 position)
        {
            m_Pressed = true;            
            m_SwipeDelta = Vector2.zero;
        }

        protected override void OnPressCancel()
        {
            m_Pressed = false;           
        }

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
        ARRaycastManager m_RaycastManager;
        [SerializeField] private float rotationSpeed;
        private bool isGameStarted;
    }
}

