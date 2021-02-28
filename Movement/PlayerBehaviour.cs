using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : CharacterBehavior
{
    [SerializeField]
    private Transform camera;

    protected override void CalculateMovingDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 cameraRelFaceDir = Vector3.ProjectOnPlane(camera.forward, transform.up).normalized;
        float anglePlayerForwCameraForw = Vector3.SignedAngle(cameraRelFaceDir, transform.forward, transform.up);
        finalDir = (Quaternion.AngleAxis(-anglePlayerForwCameraForw, transform.up) * transform.TransformDirection(moveDir)).normalized;
    }

    protected override void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (grounded || isJumpEnabled))
        {
            animator.SetBool("isJumped", true);
            animator.SetInteger("isGrounded", 0);

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

        camera = Camera.main.gameObject.transform;

        CinemachineFreeLook cinemachineVirtualCamera = GameObject.Find("Third Person Camera").GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCamera.LookAt = gameObject.transform;
        cinemachineVirtualCamera.Follow = gameObject.transform;
    }
}