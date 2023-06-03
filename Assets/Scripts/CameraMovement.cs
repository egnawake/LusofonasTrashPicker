using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float maxSpeed = 3f;

    private bool moving = false;
    private Vector3 holdPivot;

    private Vector3 NormalizedMousePosition =>
        new Vector3(Input.mousePosition.x / Screen.width,
            Input.mousePosition.y / Screen.height, 0f);

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            moving = true;
            holdPivot = NormalizedMousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            moving = false;
        }

        if (moving)
        {
            Vector3 velocity = NormalizedMousePosition - holdPivot;
            velocity = new Vector3(velocity.x, 0f, velocity.y);
            velocity = velocity * speed;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            transform.position = transform.position + velocity * Time.deltaTime;
        }
    }
}
