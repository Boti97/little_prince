﻿using Bolt;
using System;
using UnityEngine;

public abstract class CharacterBehaviour : EntityBehaviour<IPlayerState>
{
    //basic objects
    [HideInInspector]
    public Guid planet;

    protected Animator animator;

    private Transform model;

    protected GravityBody gravityBody;

    //variables for health
    protected float health = 1f;

    //variables for movement
    [SerializeField]
    protected float sprintSpeed = 30f;

    [SerializeField]
    protected float walkSpeed = 0f;

    protected float stamina = 1f;

    protected float moveSpeed = 0f;

    protected Vector3 moveDir;

    protected Vector3 finalDir;

    private readonly float turnSmoothTime = 8f;

    protected bool isMoving;

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

        animator = GetComponentInChildren<Animator>();
        gravityBody = GetComponent<GravityBody>();
        model = transform.Find("Model");

        InitializeCharacterSpecificFields();

        SetAnimation("isWalking", 0);
    }

    protected void Update()
    {
        CheckOnGround();

        if (!entity.IsOwner)
        {
            transform.Find("Model").rotation = (Quaternion)state.GetDynamic("ModelRotation");
            GetAnimation("isGrounded");
            GetAnimation("isWalking");
            GetAnimation("isJumped");
            return;
        }

        CalculateMovingDirection();

        HandleJump();

        HandleSprint();

        CheckHealth();
    }

    protected void LateUpdate()
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
                SetAnimation("isWalking", 1);
            }

            Quaternion targetRotation = Quaternion.LookRotation(finalDir, transform.up);
            model.rotation = Quaternion.Slerp(model.rotation, targetRotation, turnSmoothTime * BoltNetwork.FrameDeltaTime);
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + (finalDir * moveSpeed) * BoltNetwork.FrameDeltaTime);

            isMoving = true;
        }
        else
        {
            SetAnimation("isWalking", 0);

            isMoving = false;
        }

        state.SetDynamic("ModelRotation", transform.Find("Model").rotation);
    }

    protected abstract void CheckHealth();

    protected abstract void CalculateMovingDirection();

    protected abstract void HandleJump();

    protected abstract void HandleSprint();

    protected abstract void InitializeCharacterSpecificFields();

    protected void SetAnimation(string nameOfAnimation, int value)
    {
        animator.SetInteger(nameOfAnimation, value);
        switch (nameOfAnimation)
        {
            case "isGrounded":
                state.isGrounded = value;
                break;

            case "isWalking":
                state.isWalking = value;
                break;

            case "isJumped":
                state.isJumped = value;
                break;
        };
    }

    protected void GetAnimation(string nameOfAnimation)
    {
        int value = 0;
        switch (nameOfAnimation)
        {
            case "isGrounded":
                value = state.isGrounded;
                break;

            case "isWalking":
                value = state.isWalking;
                break;

            case "isJumped":
                value = state.isJumped;
                break;
        };

        animator.SetInteger(nameOfAnimation, value);
    }

    private void CheckOnGround()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
        {
            if (!isJumping && !isMoving)
            {
                grounded = true;
                isJumpEnabled = true;

                SetAnimation("isGrounded", 1);
                SetAnimation("isJumped", 0);
            }

            if (hit.collider.gameObject.GetComponentInParent<GravityAttractor>() != null
                && hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid != planet)
            {
                planet = hit.collider.gameObject.GetComponentInParent<GravityAttractor>().guid;
            }
        }
    }
}