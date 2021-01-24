using System;
using UnityEngine;

public abstract class CharacterBehavior : MonoBehaviour
{
    //basic objects
    [HideInInspector]
    public Guid planet;

    protected Animator animator;

    private Transform model;
    protected Transform parent;
    private GravityBody gravityBody;

    //vectors for movement
    protected Vector3 moveDir;

    protected Vector3 finalDir;
    private Vector3 previousParentPos;
    private Vector3 deltaParentPos;

    private readonly float turnSmoothTime = 8f;
    protected readonly float moveSpeed = 8f;

    protected bool isMooving;

    //variables for jump
    [SerializeField]
    protected float jumpForce = 2200f;

    [SerializeField]
    protected LayerMask groundedMask;

    protected int numberOfJumps = 0;

    protected bool isJumpEnabled;
    protected bool grounded;
    protected bool isJumping;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        gravityBody = GetComponent<GravityBody>();
        animator.SetInteger("isWalking", 0);
        model = transform.Find("Model");
    }

    private void Update()
    {
        CalculateMovingDirection();

        HandleJump();

        HandleLanding();
    }

    private void LateUpdate()
    {
        /* move only when:
         * - character is grounded and
         * - user said so and
         * - there are at least one attractor
         */
        if (grounded && moveDir.magnitude >= .1f && gravityBody.AttractorCount() > 0)
        {
            if (grounded)
            {
                animator.SetInteger("isWalking", 1);
            }

            deltaParentPos = parent.position - previousParentPos;
            previousParentPos = parent.position;
            transform.parent = null;

            Quaternion targetRotation = Quaternion.LookRotation(finalDir, transform.up);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, turnSmoothTime * Time.deltaTime);
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + deltaParentPos + (finalDir * moveSpeed) * Time.deltaTime);

            isMooving = true;
        }
        else
        {
            animator.SetInteger("isWalking", 0);

            isMooving = false;
        }
    }

    protected abstract void CalculateMovingDirection();

    protected abstract void HandleJump();

    private void HandleLanding()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if (!isJumping && !isMooving && Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;

            animator.SetInteger("isGrounded", 1);
            animator.SetBool("isJumped", false);

            parent = hit.transform;
            transform.parent = parent;

            if (hit.collider.gameObject.GetComponentInParent<GravityAttractor>() != null
                && hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid != planet)
            {
                planet = hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid;
                Debug.Log("CHANGED");
            }
        }
    }
}