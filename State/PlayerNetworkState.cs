using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkState : CharacterNetworkState
{
    public void AddHealth(float plusHealth)
    {
        if (health + plusHealth < 1f)
            health += plusHealth;
        else health = 1f;
        GameObjectManager.Instance.HealthBar.value = health;
    }

    protected override void AdditionalSetup()
    {
        if (entity.IsOwner)
        {
            GameObjectManager.Instance.CinemachineVirtualCamera.LookAt = gameObject.transform;
            GameObjectManager.Instance.CinemachineVirtualCamera.Follow = gameObject.transform;
        }

        //replaces PlayerJoinedEvent
        GameObjectManager.Instance.RefreshPlayers();
    }

    protected override void CheckHealth()
    {
        if (gravityBody.AttractorCount() == 0)
        {
            health -= 0.002f;
            GameObjectManager.Instance.HealthBar.value = health;

            if (health < 0f)
            {
                GameObjectManager.Instance.CinemachineVirtualCamera.gameObject.SetActive(false);
                EventManager.Instance.SendPlayerDiedEvent(id);
            }
        }
    }
}