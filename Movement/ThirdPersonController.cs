using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    //basic objects
    private Rigidbody rigidbody;

    private Animator playerAnimator;
    private Transform playerModel;
    private GravityBody gravityBody;

    //vectors for movement
    [SerializeField]
    private Transform camera;

    private Vector3 moveAmount;
    private Vector3 moveDir;
    private Vector3 cameraRelMoveDir;

    private float turnSmoothTime = 8f;
    private float moveSpeed = 8f;

    //variables for jump
    [SerializeField]
    private float jumpForce = 2000f;

    [SerializeField]
    private LayerMask groundedMask;

    private bool isJumpEnabled;
    private int numberOfJumps = 0;
    private bool grounded;
    private bool justJumped;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        gravityBody = GetComponent<GravityBody>();
        playerAnimator.SetInteger("isWalking", 0);
        rigidbody = transform.GetComponent<Rigidbody>();
        playerModel = transform.Find("Player Model");
    }

    private void Update()
    {
        //set all special animation to zero
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 cameraRelFaceDir = Vector3.ProjectOnPlane(camera.forward, transform.up).normalized;
        float anglePlayerForwCameraForw = Vector3.SignedAngle(cameraRelFaceDir, transform.forward, transform.up);
        cameraRelMoveDir = (Quaternion.AngleAxis(-anglePlayerForwCameraForw, transform.up) * transform.TransformDirection(moveDir)).normalized;

        moveAmount = cameraRelMoveDir * moveSpeed;

        justJumped = false;
        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            playerAnimator.SetBool("isJumped", true);
            playerAnimator.SetInteger("isGrounded", 0);
            rigidbody.AddForce(transform.up * jumpForce);
            numberOfJumps++;
            justJumped = true;
        }

        if (numberOfJumps > 1)
        {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }

        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        if (!justJumped && Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;
            playerAnimator.SetInteger("isGrounded", 1);
            playerAnimator.SetBool("isJumped", false);
            if (transform.parent != hit.transform)
            {
                transform.parent = hit.transform;
            }
        }
        else
        {
            transform.parent = null;
        }
    }

    private void FixedUpdate()
    {
        /* move only when:
         * user said so
         * there are at least one attractor
         */
        if (moveDir.magnitude >= 0.1f && gravityBody.AttractorCount() > 0)
        {
            if (grounded)
            {
                playerAnimator.SetInteger("isWalking", 1);
            }

            Quaternion targetRotation = Quaternion.LookRotation(cameraRelMoveDir, transform.up);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, turnSmoothTime * Time.deltaTime);

            rigidbody.MovePosition(rigidbody.position + moveAmount * Time.deltaTime);
        }
        else
        {
            playerAnimator.SetInteger("isWalking", 0);
        }
    }
}