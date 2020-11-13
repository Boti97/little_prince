using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    //basic objects
    private Animator playerAnimator;

    private Transform playerModel;
    private Transform parent;
    private GravityBody gravityBody;

    //vectors for movement
    [SerializeField]
    private Transform camera;

    private Vector3 moveAmount;
    private Vector3 moveDir;
    private Vector3 cameraRelMoveDir;
    private Vector3 previousParentPos;
    private Vector3 deltaParentPos;

    private readonly float turnSmoothTime = 8f;
    private readonly float moveSpeed = 8f;

    //variables for jump
    [SerializeField]
    private float jumpForce = 2000f;

    [SerializeField]
    private LayerMask groundedMask;

    private int numberOfJumps = 0;

    private bool isJumpEnabled;
    private bool grounded;
    private bool isMooving;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        gravityBody = GetComponent<GravityBody>();
        playerAnimator.SetInteger("isWalking", 0);
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

        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            playerAnimator.SetBool("isJumped", true);
            playerAnimator.SetInteger("isGrounded", 0);
            transform.parent = null;
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            numberOfJumps++;
            grounded = false;
        }

        if (numberOfJumps > 1)
        {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }

        Ray ray = new Ray(transform.position, -transform.up);
        if (!isMooving && Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;

            playerAnimator.SetInteger("isGrounded", 1);
            playerAnimator.SetBool("isJumped", false);

            parent = hit.transform;
            transform.parent = parent;
        }
    }

    private void LateUpdate()
    {
        /* move only when:
         * - player is grounded and
         * - user said so and
         * - there are at least one attractor
         */
        if (grounded && moveDir.magnitude >= 0.1f && gravityBody.AttractorCount() > 0)
        {
            if (grounded)
            {
                playerAnimator.SetInteger("isWalking", 1);
            }

            deltaParentPos = parent.position - previousParentPos;
            previousParentPos = parent.position;
            transform.parent = null;

            Quaternion targetRotation = Quaternion.LookRotation(cameraRelMoveDir, transform.up);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetRotation, turnSmoothTime * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + deltaParentPos + moveAmount * Time.deltaTime);

            isMooving = true;
        }
        else
        {
            playerAnimator.SetInteger("isWalking", 0);

            isMooving = false;
        }
    }
}