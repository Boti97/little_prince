using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunBehaviour : MonoBehaviour
{
    private Vector3 axis = -Vector3.up;

    private float selfRotationSpeed;

    private void Start()
    {
        selfRotationSpeed = Random.Range(0.0f, 10.0f);
    }

    private void Update()
    {
        transform.RotateAround(transform.position, axis, selfRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}