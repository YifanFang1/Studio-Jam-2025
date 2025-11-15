using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

// Arm inherits Limb and adds a drag mode: while left mouse is held, the limb
// freezes in place and a separate target GameObject is translated by mouse movement
// (X follows mouse X, Y moves opposite the mouse Y).
public class Arm : Limb
{
	// previous mouse world position used to compute delta
	private Vector3 previousMouseWorld;
	// delta in world units computed each Update
	private Vector2 dragDelta;

	[Header("Drag Other Target")]
	[Tooltip("Rigidbody2D to move while dragging. If set, MovePosition will be used on this Rigidbody2D.")]
	public Rigidbody2D otherTargetRigidbody;
	[Tooltip("Transform to move while dragging (used if otherTargetRigidbody is not set).")]
	public Transform otherTargetTransform;
	[Tooltip("Multiplier applied to X movement when dragging the other target.")]
	public float dragMultiplierX = 1f;
	[Tooltip("Multiplier applied to Y movement when dragging the other target.")]
	public float dragMultiplierY = 1f;

	// resolved runtime references for the other target
	private Rigidbody2D otherRb;
	private Transform otherTransformToMove;

	// whether we are currently dragging (left mouse held)
	private bool isDragging;

	protected override void Start()
	{
		// Run base initialization to set up rb, transformToMove, camera, etc.
		base.Start();

		// Resolve other target references
		otherRb = otherTargetRigidbody;
		if (otherTargetTransform != null)
			otherTransformToMove = otherTargetTransform;
		else if (otherRb != null)
			otherTransformToMove = otherRb.transform;
		else
			otherTransformToMove = null;

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
			isDragging = true;
			// Do not update targetPosition; drag mode moves the other target and limb will be frozen
		}
		else
		{
			// Exit drag mode and let base compute target for normal follow
			dragDelta = Vector2.zero;
			isDragging = false;
			base.UpdateTarget();
		}
	}

	protected override void MovePhysics()
	{
		if (isDragging)
		{
			// Freeze limb (do not call base.MovePhysics) and translate the other target by dragDelta
			if (otherRb != null)
			{
				Vector2 scaled = new Vector2(dragDelta.x * dragMultiplierX, dragDelta.y * dragMultiplierY);
				Vector2 next = otherRb.position + scaled;
				otherRb.MovePosition(next);
			}
			else if (otherTransformToMove != null)
			{
				Vector2 scaled = new Vector2(dragDelta.x * dragMultiplierX, dragDelta.y * dragMultiplierY);
				Vector3 next = otherTransformToMove.position + (Vector3)scaled;
				otherTransformToMove.position = next;
			}
			// If no other target assigned, do nothing (limb stays frozen)
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
