using UnityEngine;

public class Forearm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera mainCamera;

    public float zDistance = 10f;

    public float followSpeed = 15f;

    public float linearDrag = 2f;

    private Rigidbody2D rb;

    private Vector2 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.linearDamping = linearDrag;

        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
