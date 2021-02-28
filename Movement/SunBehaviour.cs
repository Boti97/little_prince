using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    private Vector3 axis = -Vector3.up;

    [SerializeField]
    private float selfRotationSpeed;

    public float SelfRotationSpeed { get => selfRotationSpeed; set => selfRotationSpeed = value; }

    private void Update()
    {
        transform.RotateAround(transform.position, axis, SelfRotationSpeed * Time.deltaTime);
    }
}