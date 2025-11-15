using UnityEngine;

public class Forearm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Camera mainCamera;

    public float zDistance = 10f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        // 2. Tell the camera how "far" into the world to place the object
        mouseScreenPosition.z = zDistance;

        // 3. Convert from 2D pixel coordinates to 3D world coordinates
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // 4. Set this object's position to the new world position
        transform.position = mouseWorldPosition;
    }
}
