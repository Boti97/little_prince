﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehavior
{
    [SerializeField]
    private Transform localCamera;

    private List<GameObject> players = new List<GameObject>();

    protected override void CalculateMovingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 cameraRelFaceDir = Vector3.ProjectOnPlane(localCamera.forward, transform.up).normalized;
        float anglePlayerForwCameraForw = Vector3.SignedAngle(cameraRelFaceDir, transform.forward, transform.up);
        finalDir = (Quaternion.AngleAxis(-anglePlayerForwCameraForw, transform.up) * transform.TransformDirection(moveDir)).normalized;
    }

    protected override void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            SetAnimation("isJumped", 1);
            SetAnimation("isGrounded", 0);

            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            numberOfJumps++;
            grounded = false;
            isJumping = true;
        }
        else { isJumping = false; }

        if (numberOfJumps > 1)
        {
            isJumpEnabled = false;
            numberOfJumps = 0;
        }
    }

    protected override void InitializeCharacterSpecificFields()
    {
        if (!entity.IsOwner) return;

        localCamera = Camera.main.gameObject.transform;

        CinemachineFreeLook cinemachineVirtualCamera = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCamera.LookAt = gameObject.transform;
        cinemachineVirtualCamera.Follow = gameObject.transform;
    }

    public void RefreshPlayerList()
    {
        players.Clear();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }
}