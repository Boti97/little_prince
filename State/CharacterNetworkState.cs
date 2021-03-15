using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterNetworkState : EntityBehaviour<ICharacterState>
{
    //basic objects
    [HideInInspector]
    public Guid id;

    protected Animator animator;

    protected GravityBody gravityBody;

    private bool guidSynced = false;

    //variables for health
    protected float health = 1f;

    public override void Attached()
    {
        if (entity.IsOwner)
        {
            id = Guid.NewGuid();
            state.CharacterId = id;

            gravityBody = GetComponent<GravityBody>();
        }

        AdditionalSetup();

        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!entity.IsOwner)
        {
            if (!guidSynced)
            {
                if (!state.CharacterId.Equals(Guid.Empty))
                {
                    id = state.CharacterId;
                    guidSynced = true;
                }
            }

            transform.Find("Model").rotation = state.ModelRotation;
            SetAnimations();
        }
        else
        {
            CheckHealth();
        }
    }

    protected abstract void CheckHealth();

    protected abstract void AdditionalSetup();

    private void SetAnimations()
    {
        animator.SetInteger("isGrounded", state.isGrounded);
        animator.SetInteger("isWalking", state.isWalking);
        animator.SetInteger("isJumped", state.isJumped);
    }
}