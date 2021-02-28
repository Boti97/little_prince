using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    private Vector3 axis = -Vector3.up;

    [SerializeField]
    private float selfRotationSpeed;

    private void Update()
    {
        transform.RotateAround(transform.position, axis, selfRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}