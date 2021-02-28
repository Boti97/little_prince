using Bolt;
using System;
using UnityEngine;

public abstract class CharacterBehavior : EntityBehaviour<IPlayerState>
{
    //basic objects
    [HideInInspector]
    public Guid planet;

    protected Animator animator;

    private Transform model;

    private GravityBody gravityBody;

    //vectors for movement
    protected Vector3 moveDir;

    protected Vector3 finalDir;

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

    public override void Attached()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        state.SetDynamic("ModelRotation", transform.Find("Model").rotation);

        InitializeCharacterSpecificFields();

        animator = GetComponentInChildren<Animator>();
        gravityBody = GetComponent<GravityBody>();
        animator.SetInteger("isWalking", 0);
        model = transform.Find("Model");
    }

    private void Update()
    {
        if (!entity.IsOwner)
        {
            transform.Find("Model").rotation = (Quaternion)state.GetDynamic("ModelRotation");
            return;
        }

        CalculateMovingDirection();

        HandleJump();

        HandleLanding();
    }

    private void LateUpdate()
    {
        if (!entity.IsOwner) return;

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

            Quaternion targetRotation = Quaternion.LookRotation(finalDir, transform.up);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, turnSmoothTime * BoltNetwork.FrameDeltaTime);
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + (finalDir * moveSpeed) * BoltNetwork.FrameDeltaTime);

            isMooving = true;
        }
        else
        {
            animator.SetInteger("isWalking", 0);

            isMooving = false;
        }

        state.SetDynamic("ModelRotation", transform.Find("Model").rotation);
    }

    protected abstract void CalculateMovingDirection();

    protected abstract void HandleJump();

    protected abstract void InitializeCharacterSpecificFields();

    private void HandleLanding()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if (!isJumping && !isMooving && Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
        {
            grounded = true;
            isJumpEnabled = true;

            animator.SetInteger("isGrounded", 1);
            animator.SetBool("isJumped", false);

            if (hit.collider.gameObject.GetComponentInParent<GravityAttractor>() != null
                && hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid != planet)
            {
                planet = hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid;
            }
        }
    }
}