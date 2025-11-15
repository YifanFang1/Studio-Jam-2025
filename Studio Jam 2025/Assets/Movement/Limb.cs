using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class Limb : MonoBehaviour
{
    [Tooltip("The camera to use. Defaults to Camera.main if not set.")]
    public Camera mainCamera;

    [Header("Target (optional)")]
    [Tooltip("Optional: assign a Rigidbody2D on another object to move instead of this object.")]
    public Rigidbody2D targetRigidbody;
    [Tooltip("Optional: if target Rigidbody is not set and you want to move a Transform directly, assign it here.")]
    public Transform targetTransform;

    [Tooltip("How strongly the object is pulled towards the mouse.")]
    public float followSpeed = 15f;

    [Tooltip("How much the movement is dampened. Higher = less 'slippery'.")]
    public float linearDrag = 2f;

    // The Rigidbody2D that will actually be moved. This will be targetRigidbody if set,
    // otherwise the object's own Rigidbody2D component.
    protected Rigidbody2D rb;

    // Fallback transform to move if no Rigidbody is available on the target.
    protected Transform transformToMove;

    protected Vector2 targetPosition;

    protected virtual void Start()
    {
        // If a target Rigidbody is assigned, move that one; otherwise use local Rigidbody2D
        if (targetRigidbody != null)
        {
            rb = targetRigidbody;
        }
        else
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Apply linear drag to the selected rigidbody (if any)
        if (rb != null)
        {
            rb.linearDamping = linearDrag;
        }

        // Determine the transform to move if no Rigidbody is present
        if (targetTransform != null)
        {
            transformToMove = targetTransform;
        }
        else if (rb != null)
        {
            transformToMove = rb.transform;
        }
        else
        {
            transformToMove = transform;
        }

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
        // Move the selected target toward the target position. Prefer Rigidbody2D.MovePosition
        // when a Rigidbody is present so physics remains stable; otherwise set the Transform.position.
        Vector2 currentPos = (rb != null ? rb.position : (Vector2)transformToMove.position);
        Vector2 next = Vector2.MoveTowards(currentPos, targetPosition, followSpeed * Time.fixedDeltaTime);

        if (rb != null)
        {
            rb.MovePosition(next);
        }
        else
        {
            transformToMove.position = next;
        }
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // You can also check the object's Tag
    //     if (collision.gameObject.tag == "")
    //     {
            
    //     }
    // }
}
