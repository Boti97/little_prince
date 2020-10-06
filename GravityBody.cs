using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class GravityBody : MonoBehaviour
{
    GravityAttractor gravityAttractor;
    Rigidbody rigidbody;

    void Awake()
    {
        gravityAttractor = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    //all rigidbody calculation should be here
    void FixedUpdate()
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
