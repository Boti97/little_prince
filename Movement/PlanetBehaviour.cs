using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform orbitCenter;

    protected float attractTurnSpeed = 0.1f;

    private Vector3 axis = Vector3.up;
    private readonly float rotationSpeed = 0f;
    private float selfRotationSpeed = 0f;
    private Vector3 selfRotationAxis = Vector3.up;

    private void Start()
    {
        selfRotationSpeed = Random.Range(0.0f, selfRotationSpeed);
        selfRotationAxis = VectorExtensions.RandomAxis();
    }

    private void Update()
    {
        //transform.RotateAround(orbitCenter.position, axis, rotationSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, selfRotationAxis, selfRotationSpeed * Time.deltaTime);
    }
}