using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [SerializeField]
    private float gravity = -10f;
    [SerializeField]
    private float turnSpeed = 0.1f;


    public void Attract(GameObject body)
    {
        Vector3 targetDir = (body.transform.position - transform.position).normalized;
        Vector3 bodyUp = body.transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.transform.rotation;

        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, turnSpeed);
        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }
}
