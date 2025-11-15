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

    protected Rigidbody2D rb;

    protected Vector2 targetPosition;

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
        UpdateTarget();
    }

    /// <summary>
    /// Updates the targetPosition from input. Subclasses can override to change how the target is computed.
    /// </summary>
    protected virtual void UpdateTarget()
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
        //float zDistance = -mainCamera.transform.position.z;
        Vector3 screenPoint = new Vector3(mouseScreen.x, mouseScreen.y, 0);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        targetPosition = worldPoint;
    }

    void FixedUpdate()
    {
        MovePhysics();
    }

    /// <summary>
    /// Performs the physics movement. Subclasses can override to provide custom movement (e.g., drag behavior).
    /// </summary>
    protected virtual void MovePhysics()
    {
        // Move the Rigidbody2D toward the target position. Use MovePosition so we
        // update the physics body directly and avoid any possibility of moving the camera.
        Vector2 next = Vector2.MoveTowards(rb.position, targetPosition, followSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // You can also check the object's Tag
    //     if (collision.gameObject.tag == "")
    //     {
            
    //     }
    // }
}
