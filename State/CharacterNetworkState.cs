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

    //variables for health
    protected float health = 1f;

    private bool guidSynced = false;

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

            transform.Find("Model").rotation = (Quaternion)state.GetDynamic("ModelRotation");
            GetAnimation("isGrounded");
            GetAnimation("isWalking");
            GetAnimation("isJumped");
        }
        else
        {
            CheckHealth();
        }
    }

    private void GetAnimation(string nameOfAnimation)
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

    protected abstract void CheckHealth();

    protected abstract void AdditionalSetup();
}