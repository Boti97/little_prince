﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehavior
{
    private List<GameObject> players = new List<GameObject>();

    private GameObject playerToFollow;

    protected override void CalculateMovingDirection()
    {
        //if we did not choose a player to follow yet, or the player we followed jumped to another planet as us, we choose one
        if ((playerToFollow == null && players.Count > 0) || (!IsPlayerOnSamePlanet(playerToFollow) && players.Count > 1))
        {
            //find the players whos are on the same planet
            List<GameObject> followablePlayers = new List<GameObject>();
            foreach (GameObject player in players)
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

        //if we have a player to follow, who's on the same planet as us, follow it
        if (playerToFollow != null && IsPlayerOnSamePlanet(playerToFollow))
        {
            Vector3 distanceFromPlayer = Vector3.ProjectOnPlane((playerToFollow.transform.position - transform.position), transform.up);
            if (distanceFromPlayer.magnitude > 1.5f)
            {
                finalDir = Vector3.ProjectOnPlane((playerToFollow.transform.position - transform.position).normalized, transform.up).normalized;
                moveDir = Vector3.forward;
            }
            else
            {
                moveDir = Vector3.zero;
            }
        }
        else
        {
            moveDir = Vector3.zero;
        }
    }

    protected override void HandleJump()
    {
        isJumping = false;

        if (numberOfJumps > 1)
        {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }
    }

    protected override void InitializeCharacterSpecificFields()
    {
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    private bool IsPlayerOnSamePlanet(GameObject player)
    {
        return player.GetComponent<PlayerBehaviour>().planet == planet;
    }

    public void RefreshPlayerList()
    {
        players.Clear();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }
}