using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

// ArmDrag inherits Limb and adds a drag mode: while left mouse is held, the arm
// translates with the mouse where X follows mouse X and Y moves opposite to mouse Y.
[RequireComponent(typeof(Rigidbody2D))]
public class Arm : Limb
{
    // previous mouse world position used to compute delta
    private Vector3 previousMouseWorld;
    // delta in world units computed each Update
    private Vector2 dragDelta;

    new void Start()
    {
        previousMouseWorld = GetMouseWorldPoint();
    }

    protected override void UpdateTarget()
    {
        // Compute current mouse world point and delta since last frame.
        Vector3 currentMouseWorld = GetMouseWorldPoint();
        Vector3 delta = currentMouseWorld - previousMouseWorld;
        previousMouseWorld = currentMouseWorld;

        bool isPressed = IsLeftMousePressed();

        if (isPressed)
        {
            // X follows, Y inverted
            dragDelta = new Vector2(delta.x, -delta.y);
            // Do not update targetPosition; Drag mode will move directly
        }
        else
        {
            // Clear drag delta and let base compute target for normal follow
            dragDelta = Vector2.zero;
            // call base behavior to set targetPosition normally
            base.UpdateTarget();
        }
    }

    protected override void MovePhysics()
    {
        if (dragDelta != Vector2.zero)
        {
            Vector2 next = rb.position + dragDelta;
            rb.MovePosition(next);
        }
        else
        {
            base.MovePhysics();
        }
    }

    private Vector3 GetMouseWorldPoint()
    {
        Vector2 mouseScreen;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        mouseScreen = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        mouseScreen = Input.mousePosition;
#endif
        float zDistance = -mainCamera.transform.position.z;
        Vector3 screenPoint = new Vector3(mouseScreen.x, mouseScreen.y, zDistance);
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
        return worldPoint;
    }

    private bool IsLeftMousePressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
#else
        return Input.GetMouseButton(0);
#endif
    }
}
