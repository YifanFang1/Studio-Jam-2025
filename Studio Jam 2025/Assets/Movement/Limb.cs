using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class Limb : MonoBehaviour
{
    [Tooltip("The camera to use. Defaults to Camera.main if not set.")]
    public Camera mainCamera;

    [Tooltip("How strongly the object is pulled towards the mouse.")]
    public float followSpeed = 15f;

    [Tooltip("How much the movement is dampened. Higher = less 'slippery'.")]
    public float linearDrag = 2f;

    private Rigidbody2D rb;

    private Vector2 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.linearDamping = linearDrag;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Read mouse position using the active input system.
    #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Vector2 mouseScreen = Vector2.zero;
        if (Mouse.current != null)
            mouseScreen = Mouse.current.position.ReadValue();
    #else
        Vector2 mouseScreen = Input.mousePosition;
    #endif

        // For ScreenToWorldPoint we need a z distance from the camera. For a typical 2D scene
        // convert the screen point to world at the plane z = 0 by using the distance from camera to z=0.
        float zDistance = -mainCamera.transform.position.z;
        Vector3 screenPoint = new Vector3(mouseScreen.x, mouseScreen.y, zDistance);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        targetPosition = worldPoint;
    }

    void FixedUpdate()
    {
        Vector2 directionToTarget = targetPosition - rb.position;

        Vector2 targetVelocity = directionToTarget * followSpeed;

        rb.linearVelocity = targetVelocity;
    }
}
