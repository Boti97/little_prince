using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    public float gravity = -10f;
    
    public void Attract(GameObject body)
    {
        Vector3 targetDir = (body.transform.position - transform.position).normalized;
        Vector3 bodyUp = body.transform.up;

        body.transform.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.transform.rotation;
        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }
}
