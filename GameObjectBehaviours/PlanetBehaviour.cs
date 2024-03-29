﻿using Photon.Bolt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBehaviour : EntityBehaviour<IPlanetState>
{
    [SerializeField]
    private Transform orbitCenter;

    protected float attractTurnSpeed = 0.1f;

    private Vector3 axis = Vector3.up;
    private readonly float rotationSpeed = 0f;
    private float selfRotationSpeed = 0f;
    private Vector3 selfRotationAxis = Vector3.up;

    public override void Attached()
    {
        if (!entity.IsOwner) return;
        selfRotationSpeed = Random.Range(0.0f, selfRotationSpeed);
        selfRotationAxis = VectorExtensions.RandomAxis();
        //orbitCenter = GameObjectManager.Instance.Sun.transform;
    }

    private void Update()
    {
        if (entity != null && !entity.IsOwner) return;
        if (orbitCenter != null)
        {
            transform.RotateAround(orbitCenter.position, axis, rotationSpeed * Time.deltaTime);
        }
        transform.RotateAround(transform.position, selfRotationAxis, selfRotationSpeed * Time.deltaTime);
    }
}