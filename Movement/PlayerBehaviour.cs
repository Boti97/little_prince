using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviour : CharacterBehaviour
{
    [SerializeField]
    private Transform localCamera;

    private List<GameObject> players = new List<GameObject>();

    private bool hitZeroStamina;

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

        Debug.Log("Second");
        Slider a = GameObjectManager.Instance.StaminaBar;
        GameObjectManager.Instance.CinemachineVirtualCamera.LookAt = gameObject.transform;
        GameObjectManager.Instance.CinemachineVirtualCamera.Follow = gameObject.transform;
    }

    protected override void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0f && !hitZeroStamina)
        {
            moveSpeed = sprintSpeed;
            stamina -= 0.003f;
            GameObjectManager.Instance.StaminaBar.value = stamina;

            if (stamina < 0.005)
            {
                hitZeroStamina = true;
            }
        }
        else
        {
            if (stamina < 1f)
            {
                stamina += 0.0005f;
                GameObjectManager.Instance.StaminaBar.value = stamina;
            }
            if (stamina > 0.3f)
            {
                hitZeroStamina = false;
            }
            moveSpeed = walkSpeed;
        }
    }

    protected override void CheckHealth()
    {
        if (gravityBody.AttractorCount() == 0)
        {
            health -= 0.002f;
            GameObjectManager.Instance.HealthBar.value = health;

            if (health < 0f) GameObjectManager.Instance.GameOverText.SetActive(true);
        }
    }

    protected override void HandleThrust()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<Rigidbody>().AddForce(localCamera.forward * thrustPower);
        }
    }
}