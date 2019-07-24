using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5; // units per second
    public float turnSpeed = 90; // degrees per second
    public float jumpSpeed = 8;
    public float gravity = 9.8f;
    public float vSpeed = 0; // current vertical velocity
    public float interactRange = 0f;

    public GameObject target;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleCursorLockState()
    {
        if (Cursor.lockState != CursorLockMode.None)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {
        Vector3 vel = transform.forward * Input.GetAxis("Vertical") * speed + transform.right * Input.GetAxis("Horizontal") * speed;
        var controller = GetComponent<CharacterController>();

        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleCursorLockState();

        if (controller.isGrounded)
        {
            vSpeed = 0; // grounded character has vSpeed = 0...
            if (Input.GetKeyDown("space"))
            { // unless it jumps:
                vSpeed = jumpSpeed;
            }
        }

        Camera.main.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));

        // apply gravity acceleration to vertical speed:
        vSpeed -= gravity * Time.deltaTime;
        vel.y = vSpeed; // include vertical speed in vel
                        // convert vel to displacement and Move the character:
        controller.Move(vel * Time.deltaTime);

        //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * interactRange, Color.red);
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.green);

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactRange))
        {
            target = hit.collider.gameObject;

            if(Input.GetMouseButtonDown(0))
            {
                if(target.GetComponent<VoxelChunk>())
                {
                    target.GetComponent<VoxelChunk>().RemoveVoxel(target.GetComponent<VoxelChunk>().GetRemoveAtPosition(hit.point, Camera.main.transform.forward, hit));
                }
            }
            else if(Input.GetMouseButtonDown(1))
            {
                if (target.GetComponent<VoxelChunk>())
                {
                    target.GetComponent<VoxelChunk>().AddVoxel(target.GetComponent<VoxelChunk>().GetAddAtPosition(hit.point, Camera.main.transform.forward, hit));
                }
            }
        }
        else
        {
            target = null;
        }
    }
}
