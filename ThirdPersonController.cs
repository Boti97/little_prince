using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 8f;

    [SerializeField]
    private float jumpForce = 300;

    [SerializeField]
    private float mouseSensitivity = 250f;

    [SerializeField]
    private LayerMask groundedMask;

    [SerializeField]
    private Transform camera;

    private Vector3 moveAmount;
    private Vector3 moveDir;
    private Rigidbody rigidbody;
    private bool grounded;
    private bool isJumpEnabled;
    private int numberOfJumps = 0;
    private Vector3 smoothMoveVelocity;
    private Animator playerAnimator;
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerAnimator.SetInteger("isWalking", 0);
        rigidbody = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 targetAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetAmount, ref smoothMoveVelocity, .15f);

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
            Debug.DrawRay(transform.position, transform.Find("Player Model").TransformDirection(moveDir) * 10, Color.black);

            Vector3 moveDirWP = transform.TransformDirection(moveDir);

            //rotate the model the way we're going
            float targetAngle = Mathf.Atan2(moveDirWP.x, moveDirWP.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.Find("Player Model").eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            //transform.Find("Player Model").rotation = Quaternion.FromToRotation(transform.Find("Player Model").position, moveDirWP);

            //transform.Find("Player Model").rotation = Quaternion.Euler(transform.Find("Player Model").eulerAngles.x, angle, transform.Find("Player Model").eulerAngles.z);

            Debug.DrawRay(transform.Find("Player Model").position, transform.Find("Player Model").forward * 10, Color.green);

            //move the body to that position
            rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
        }
        else
        {
            playerAnimator.SetInteger("isWalking", 0);
        }
    }
}