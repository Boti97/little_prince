using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : CharacterBehavior
{
    [SerializeField]
    private Transform localCamera;

    private List<GameObject> players = new List<GameObject>();

    private Slider staminaBar;
    private Slider healthBar;
    private GameObject gameOverText;

    public void RefreshPlayerList()
    {
        players.Clear();
        players.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

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
        if (Input.GetKeyDown(KeyCode.Space) && (grounded || isJumpEnabled))
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

        CinemachineFreeLook cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("ThirdPersonCamera").GetComponent<CinemachineFreeLook>();
        cinemachineVirtualCamera.LookAt = gameObject.transform;
        cinemachineVirtualCamera.Follow = gameObject.transform;

        staminaBar = GameObject.FindGameObjectWithTag("StaminaBar").GetComponent<Slider>();
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();

        gameOverText = GameObject.FindGameObjectWithTag("GameOver");
        gameOverText.SetActive(false);
    }

    protected override void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f)
        {
            moveSpeed = sprintSpeed;
            stamina -= 0.001f;
            staminaBar.value = stamina;
        }
        else
        {
            if (stamina < 1f)
            {
                stamina += 0.0005f;
                staminaBar.value = stamina;
            }
            moveSpeed = walkSpeed;
        }
    }

    protected override void CheckHealth()
    {
        if (gravityBody.AttractorCount() == 0)
        {
            health -= 0.002f;
            healthBar.value = health;

            if (health < 0f) gameOverText.SetActive(true);
        }
    }
}