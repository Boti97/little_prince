using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundedMask;

    [SerializeField]
    private Transform camera;

    //basic objects
    private Rigidbody rigidbody;

    private Animator playerAnimator;
    private Transform playerModel;

    //vectors for movement
    private Vector3 moveAmount;

    private Vector3 moveDir;
    private Vector3 cameraRelMoveDir;

    private float turnSmoothTime = 8f;
    private float moveSpeed = 8f;

    //variables for jump
    private bool grounded;

    private float jumpForce = 2000f;
    private bool isJumpEnabled;
    private int numberOfJumps = 0;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerAnimator.SetInteger("isWalking", 0);
        rigidbody = transform.GetComponent<Rigidbody>();
        playerModel = transform.Find("Player Model");
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 cameraRelFaceDir = Vector3.ProjectOnPlane(camera.forward, transform.up).normalized;
        float anglePlayerForwCameraForw = Vector3.SignedAngle(cameraRelFaceDir, transform.forward, Vector3.forward);
        cameraRelMoveDir = (Quaternion.AngleAxis(-anglePlayerForwCameraForw, transform.up) * transform.TransformDirection(moveDir)).normalized;

        moveAmount = cameraRelMoveDir * moveSpeed;

        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            rigidbody.AddForce(transform.up * jumpForce);
            numberOfJumps++;
        }

        if (numberOfJumps > 1)
        {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }

        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out _, 2 + .1f, groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (moveDir.magnitude >= 0.1f)
        {
            playerAnimator.SetInteger("isWalking", 1);

            Quaternion targetRotation = Quaternion.LookRotation(cameraRelMoveDir, transform.up);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, turnSmoothTime * Time.deltaTime);

            //move the body to that position
            rigidbody.MovePosition(rigidbody.position + moveAmount * Time.deltaTime);
        }
        else
        {
            playerAnimator.SetInteger("isWalking", 0);
        }
    }
}