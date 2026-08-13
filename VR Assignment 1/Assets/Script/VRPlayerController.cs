using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float RotateSpeed = 45f;
    public float ForwardSpeed = 10f;
    public float LookSpeed = 45f;
    public float JumpForce = 5f;
    public float ClimbSpeed = 5f;
    public AudioSource footsteps;
    public AudioSource swim;

    [Header("References")]
    public Transform VRCamera;
    public CapsuleCollider capsuleCollider;

    private Rigidbody rigidBody;
    private float cameraPitch = 0f;

    private bool isInWater = false;
    private float waterSurfaceY;

    private bool isClimbing = false;
    private Vector3 climbDirection = Vector3.up;

    [Header("Scanner Settings")]
    public GameObject scanPlane;
    public Animator scanAnimator;
    public AudioSource scannerAudio;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        cameraPitch = VRCamera.localEulerAngles.x;
    }

    void OnTriggerEnter(Collider other)
    {
        //When the user touches the water rotation is freezed 
        if (other.CompareTag("Water"))
        {
            isInWater = true;
            waterSurfaceY = other.transform.position.y;
            rigidBody.useGravity = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        //When the user reaches the ladder they can move vertical
        if (other.CompareTag("Climbable"))
        {
            isClimbing = true;
            climbDirection = Vector3.up;
            rigidBody.useGravity = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Ensures that the swim sound stops and that the user is no longer swimming
        if (other.CompareTag("Water"))
        {
            isInWater = false;
            rigidBody.useGravity = true;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            if (swim.isPlaying) swim.Stop();
        }
        // Includes gravity and stops climbing
        if (other.CompareTag("Climbable"))
        {
            isClimbing = false;
            rigidBody.useGravity = true;
        }
    }

    void Update()
    {
        //Gets the gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        //Gets the left joystick
        Vector2 direction = gamepad.leftStick.ReadValue();
        bool isMoving = direction.magnitude > 0.1f;

        //Plays audio for footsteps when walking and for water when swimming
        if (isInWater)
        {
            if (footsteps.isPlaying) footsteps.Stop();

            if (isMoving)
            {
                if (!swim.isPlaying && swim.clip != null && swim.enabled)
                {
                    swim.loop = true;
                    swim.Play();
                }
            }
            else
            {
                if (swim.isPlaying) swim.Stop();
            }
        }
        else
        {
            if (swim.isPlaying) swim.Stop();

            if (isMoving)
            {
                if (!footsteps.isPlaying && footsteps.clip != null && footsteps.enabled)
                {
                    footsteps.loop = true;
                    footsteps.Play();
                }
            }
            else
            {
                if (footsteps.isPlaying) footsteps.Stop();
            }
        }
        //Gets the right joy stick and allows the user to look around
        Vector2 lookDirection = gamepad.rightStick.ReadValue();
        cameraPitch -= lookDirection.y * LookSpeed * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, -80f, 80f);
        VRCamera.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        transform.eulerAngles += Vector3.up * (lookDirection.x * LookSpeed * Time.deltaTime);

       //Allows the user to climb to only vertically climb up the ladder
        if (isClimbing)
        {
            float climbInput = direction.y;
            Vector3 climbVelocity = climbDirection * climbInput * ClimbSpeed;
            climbVelocity.x = 0f;
            climbVelocity.z = 0f;
            rigidBody.linearVelocity = climbVelocity;

            //Allows the user to jump of the ladder
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                isClimbing = false;
                rigidBody.useGravity = true;
                rigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }

            return;
        }

        if (isInWater)
        {
            //Floats the camera above the water 
            float cameraOffsetY = VRCamera.localPosition.y;
            float desiredPlayerY = waterSurfaceY - cameraOffsetY + 0.03f;
            Vector3 position = transform.position;
            position.y = Mathf.Max(position.y, desiredPlayerY);
            transform.position = position;

            //Allows the user to swim in a given direction
            Vector3 forward = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Vector3.forward;
            Vector3 swimVelocity = forward * direction.y * ForwardSpeed;
            swimVelocity.y = 0f;
            rigidBody.linearVelocity = swimVelocity;
        }
        else
        {
            //Allows the user to move in directions based on horisontal rotation
            Quaternion yawRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            Vector3 forward = yawRotation * Vector3.forward;
            Vector3 left = yawRotation * Vector3.left;
            Vector3 velocity = forward * direction.y * ForwardSpeed - left * direction.x * ForwardSpeed;
            velocity.y = rigidBody.linearVelocity.y;
            rigidBody.linearVelocity = velocity;
        }

        //  Jumps if B button is pressed if user is on ground
        if (gamepad.buttonEast.wasPressedThisFrame &&
            Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f))
        {
            rigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
        // Activates the scan animation and sound if X is pressed
        if (gamepad.buttonWest.wasPressedThisFrame)
        {
            scanPlane.SetActive(true);
            scanAnimator.SetBool("Scan", true);
            scannerAudio.enabled = true;
            StartCoroutine(FinishScan());
        }

        RaycastHit hit;
        if (Physics.Raycast(VRCamera.position, VRCamera.forward, out hit))
        {
            GameObject firstHitObject = hit.collider.gameObject;
            if (gamepad.rightShoulder.wasPressedThisFrame)
            {
                ObjectController objectController = firstHitObject.GetComponent<ObjectController>();
                objectController?.OnPointerClick();
            }
        }
    }
    //Stops scan audio and animation
    IEnumerator FinishScan()
    {
        yield return new WaitForSeconds(3f);
        scanAnimator.SetBool("Scan", false);
        scannerAudio.enabled = false;
        yield return new WaitForSeconds(1.5f);
        scanPlane.SetActive(false);
    }
}
