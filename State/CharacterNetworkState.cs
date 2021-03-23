using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterNetworkState : EntityBehaviour<ICharacterState>, IObjectState
{
    //basic objects
    private Guid characterId;

    protected Animator animator;

    private Transform model;

    protected GravityBody gravityBody;

    private bool guidSynced = false;

    protected float health = 1f;

    public override void Attached()
    {
        state.SetTransforms(state.CharacterTransform, transform);

        if (entity.IsOwner)
        {
            characterId = Guid.NewGuid();
            state.CharacterId = characterId;

            gravityBody = GetComponent<GravityBody>();
        }
        AdditionalSetup();

        animator = GetComponentInChildren<Animator>();
        model = transform.Find("Model");
    }

    private void Update()
    {
        if (!entity.IsOwner)
        {
            if (!guidSynced)
            {
                if (!state.CharacterId.Equals(Guid.Empty))
                {
                    characterId = state.CharacterId;
                    guidSynced = true;
                }
            }

            transform.Find("Model").rotation = state.ModelRotation;
            SetAnimations();
            return;
        }
        else
        {
            CheckHealth();
        }

        state.ModelRotation = model.rotation;
        state.ParentId = transform.GetComponent<CharacterBehaviour>().GetParentId();
    }

    protected abstract void CheckHealth();

    protected abstract void AdditionalSetup();

    private void SetAnimations()
    {
        animator.SetInteger("isGrounded", state.isGrounded);
        animator.SetInteger("isWalking", state.isWalking);
        animator.SetInteger("isJumped", state.isJumped);
    }

    public Guid GetGuid()
    {
        return characterId;
    }
}