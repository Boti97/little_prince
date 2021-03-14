using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour
{
    [SerializeField]
    private float pushPower;

    [SerializeField]
    private float pushPowerAmplifier;

    private int numberOfPushes = 0;
    private GameObject playerToFollow;

    protected override void CalculateMovingDirection()
    {
        //if we did not choose a player to follow yet, or the player we followed jumped to another planet as us, we choose one
        if ((playerToFollow == null && GameObjectManager.Instance.Players.Count > 0) || (!IsPlayerOnSamePlanet(playerToFollow) && GameObjectManager.Instance.Players.Count > 1))
        {
            //find the players whos are on the same planet
            List<GameObject> followablePlayers = new List<GameObject>();
            foreach (GameObject player in GameObjectManager.Instance.Players)
            {
                if (IsPlayerOnSamePlanet(player))
                {
                    followablePlayers.Add(player);
                }
            }

            //choose a random player to follow
            if (followablePlayers.Count > 1)
            {
                playerToFollow = followablePlayers[Random.Range(0, followablePlayers.Count) - 1];
            }
            else if (followablePlayers.Count > 0)
            {
                playerToFollow = followablePlayers[0];
            }
        }
    }

    protected override void InitializeCharacterSpecificFields()
    {
        return;
    }

    private bool IsPlayerOnSamePlanet(GameObject player)
    {
        if (player != null)
        {
            return player.GetComponent<PlayerBehaviour>().planet == planet;
        }
        return false;
    }

    protected override void HandleSprint()
    {
        //TODO: implement enemy sprint
        moveSpeed = walkSpeed;
        return;
    }

    protected override void HandleJump()
    {
        //TODO: implement jumping
        return;
    }

    protected override void HandleThrust()
    {
        //TODO: implement thrusting
        return;
    }

    protected override void HandleAttack()
    {
        //if we have a player to follow, who's on the same planet as us, follow it
        if (playerToFollow != null && IsPlayerOnSamePlanet(playerToFollow))
        {
            Vector3 distanceFromPlayer = Vector3.ProjectOnPlane(playerToFollow.transform.position - transform.position, transform.up);
            if (distanceFromPlayer.magnitude > 1.5f)
            {
                finalDir = Vector3.ProjectOnPlane((playerToFollow.transform.position - transform.position).normalized, transform.up).normalized;
                moveDir = Vector3.forward;
            }
            //if we close enough we push them into space
            else
            {
                EventManager.Instance.SendCharacterPushedEvent(
                        transform.Find("Model").forward,
                        playerToFollow.GetComponent<PlayerNetworkState>().id,
                        pushPower + (numberOfPushes * pushPowerAmplifier));

                numberOfPushes++;
                moveDir = Vector3.zero;
            }
        }
        else
        {
            moveDir = Vector3.zero;
        }
    }
}