﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMovement : MonoBehaviour
{
    [SerializeField]
    private Transform orbitCenter;

    protected float attractTurnSpeed = 0.1f;

    private Vector3 axis = Vector3.up;
    private readonly float rotationSpeed = 1f;
    private float selfRotationSpeed;

    private void Start()
    {
        selfRotationSpeed = Random.Range(0.0f, 10.0f);
    }

    private void Update()
    {
        transform.RotateAround(orbitCenter.position, axis, rotationSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, axis, selfRotationSpeed * Time.deltaTime);
    }
}