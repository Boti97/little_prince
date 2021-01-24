using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehavior
{
    [SerializeField]
    private GameObject player;

    protected override void CalculateMovingDirection()
    {
        Vector3 distanceFromPlayer = Vector3.ProjectOnPlane((player.transform.position - transform.position), transform.up);
        if (isPlayerOnSamePlanet() && distanceFromPlayer.magnitude > 1.5f)
        {
            finalDir = Vector3.ProjectOnPlane((player.transform.position - transform.position).normalized, transform.up).normalized;
            moveDir = Vector3.forward;
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

    private bool isPlayerOnSamePlanet()
    {
        return player.GetComponent<PlayerBehaviour>().planet == planet;
    }
}