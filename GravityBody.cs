﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{

    [SerializeField]
    private GameObject initialAttractor;

    private GravityAttractor gravityAttractor;
    private Rigidbody rigidbody;

    private void Awake()
    {
        //it is not mandatory to add an initial attractor
        if(initialAttractor != null)
        {
            gravityAttractor = initialAttractor.GetComponent<GravityAttractor>();
        }
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        if(gravityAttractor != null)
        {
            gravityAttractor.Attract(transform.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        gravityAttractor = other.gameObject.GetComponent<GravityAttractor>();
    }

}
