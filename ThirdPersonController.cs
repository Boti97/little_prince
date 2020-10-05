using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 15;
    [SerializeField]
    private float jumpForce = 220;
    [SerializeField]
    private LayerMask groundedMask;

    private Vector3 moveDir;
    private Rigidbody rigidbody;
    private bool grounded;
    private bool isJumpEnabled;
    private int numberOfJumps = 0;

    private void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            rigidbody.AddForce(transform.up * jumpForce);
            numberOfJumps++;
        }

        if(numberOfJumps > 1) {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }

        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray, out raycastHit, 2 + .1f , groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveDir) * moveSpeed * Time.deltaTime);
    }
}
