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
        moveDir = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 targetAmount = moveDir * moveSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetAmount, ref smoothMoveVelocity, .15f);

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
        if (Physics.Raycast(ray, out _, 2 + .1f , groundedMask))
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
            rigidbody.MovePosition(rigidbody.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
        } 
        else
        {
            playerAnimator.SetInteger("isWalking", 0);
        }
    }
}
