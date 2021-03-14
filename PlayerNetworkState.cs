using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkState : EntityBehaviour<IPlayerState>
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
            state.SetDynamic("PlayerId", id);

            GameObjectManager.Instance.CinemachineVirtualCamera.LookAt = gameObject.transform;
            GameObjectManager.Instance.CinemachineVirtualCamera.Follow = gameObject.transform;
        }

        gravityBody = GetComponent<GravityBody>();
        animator = GetComponentInChildren<Animator>();

        //replaces PlayerJoinedEvent
        GameObjectManager.Instance.RefreshPlayers();
    }

    public void AddHealth(float plusHealth)
    {
        if (health + plusHealth < 1f)
            health += plusHealth;
        else health = 1f;
        GameObjectManager.Instance.HealthBar.value = health;
    }

    private void Update()
    {
        if (!entity.IsOwner)
        {
            if (!guidSynced)
            {
                if (!state.PlayerId.Equals(Guid.Empty))
                {
                    id = state.PlayerId;
                    guidSynced = true;
                }
            }

            transform.Find("Model").rotation = (Quaternion)state.GetDynamic("ModelRotation");
            GetAnimation("isGrounded");
            GetAnimation("isWalking");
            GetAnimation("isJumped");
            return;
        }
        else
        {
            CheckHealth();
        }
    }

    private void CheckHealth()
    {
        if (gravityBody.AttractorCount() == 0)
        {
            health -= 0.002f;
            GameObjectManager.Instance.HealthBar.value = health;

            if (health < 0f)
            {
                GameObjectManager.Instance.CinemachineVirtualCamera.gameObject.SetActive(false);
                PlayerDiedEvent playerDiedEvent = PlayerDiedEvent.Create();
                playerDiedEvent.DeadPlayerId = id;
                playerDiedEvent.Send();
            }
        }
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
}