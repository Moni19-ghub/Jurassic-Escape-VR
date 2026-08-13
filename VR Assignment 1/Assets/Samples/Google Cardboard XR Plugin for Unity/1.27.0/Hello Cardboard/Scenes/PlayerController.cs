using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float RotateSpeed = 45;
    public float ForwardSpeed = 1;
    public Transform Cursor;
    public Transform VRCamera;

    // Update is called once per frame
    void Update()
    {
        // Capture controller interactions
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Get direction that left joystick is pointing. x and y both
        // range from -1 to +1
        Vector2 direction = gamepad.leftStick.ReadValue();

        // Rotate left/right based on joystick's left/right position
        transform.eulerAngles += Vector3.up * (direction.x * RotateSpeed * Time.deltaTime);

        // Move forwards/backwards based on joystick's up/down position.
        transform.position += transform.forward * (direction.y * ForwardSpeed * Time.deltaTime);

        // Move cursor
        RaycastHit hit;
        GameObject firstHitObject = null;

        // Does it hit anything on the Interactive layer?
        if (Physics.Raycast(VRCamera.position, VRCamera.forward, out hit))
        {
            firstHitObject = hit.collider.gameObject;
            Cursor.transform.position = hit.point;
        }

        // Pressed right bumper, see if 
        if (gamepad.rightShoulder.wasPressedThisFrame && (firstHitObject != null))
        {
            // Get a ray starting at the camera and pointing in the forward direction.
            ObjectController objectController = firstHitObject.GetComponent<ObjectController>();
            objectController?.OnPointerClick();
        }
    }
}
