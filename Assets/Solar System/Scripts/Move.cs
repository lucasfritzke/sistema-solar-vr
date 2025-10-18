using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))] //Adds Character Controller component is this is needed for the script to work.
public class Move : MonoBehaviour
{
    public float currentSpeed = 10;			//The speed the object is currently moving. This should not be modified and can be made private.
    public float gravity = 0.0f;				//The ammount of gravity imposed on the object. O, of course, means none and therefore is not affected by gravity.
    public float rotateSpeed = 150.0f;			//The ammount of left/right turning power when the RMB is held down.
    public float rotateLocal = 2;				//The ammount of roll imposed on the object.
    public float mouseTurnMultiplier = 0.05f;	//The ammount of pitch and yaw imposed on the object when the RMB is held down.
    public float mouseSpeedMultiplier = 5f;	//The ammount of pitch and yaw imposed on the object when the RMB is held down.

    private Vector3 moveDirection = Vector3.zero; //The direction of movement the object will be heading.

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));     //This moves the object in the direction directly from it's axis
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= currentSpeed;

        if (Input.GetKey(KeyCode.Q))                //rotates the object left around its Z axis at (rotateLocal).
            transform.Rotate(Vector3.forward * rotateLocal);

        if (Input.GetKey(KeyCode.E))                //rotate the object right around its Z axis at (rotateLocal).
            transform.Rotate(Vector3.forward * rotateLocal * -1);

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
            currentSpeed = currentSpeed - mouseSpeedMultiplier;

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
            currentSpeed = currentSpeed + mouseSpeedMultiplier;

        if (Input.GetMouseButton(1))                // Check to see if the right mouse button is pressed, and, if so, get horizontal rotation. This allows mouse cursor use for GUI when not in use and movment when in use.
        {
            float x = Input.GetAxis("Mouse X") * rotateSpeed * mouseTurnMultiplier;
            transform.Rotate(0, x, 0);

            float y = Input.GetAxis("Mouse Y") * rotateSpeed * mouseTurnMultiplier;
            transform.Rotate(-y, 0, 0);
        }

        moveDirection.y -= gravity * Time.deltaTime;                // Applies gravity
        controller.Move(moveDirection * Time.deltaTime);            // Move the controller
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width / 2, 25, 90, 25), "Speed: " + currentSpeed);
    }
}