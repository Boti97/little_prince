using System;
using UnityEngine;


public class GravityAttractor : MonoBehaviour
{
    [HideInInspector]
    public Guid guid;
    [SerializeField]
    protected float gravity = -100f;
    [SerializeField]
    private float gravityPowerIndicator = 1;
    public float GravityPowerIndicator { 
        get { return gravityPowerIndicator; } 
        set { gravityPowerIndicator = value; } 
    }

    protected float attractTurnSpeed = 0.1f;

    private void Awake()
    {
        if(guid != null)
        {
            guid = Guid.NewGuid();
        }
    }

    //base method, this needs to be overridden for special gravity behavior
    public void Attract(GameObject body)
    {
        Vector3 targetDir = (body.transform.position - transform.position).normalized;
        Vector3 bodyUp = body.transform.up;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.transform.rotation;

        body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, attractTurnSpeed);

        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity / GetGravityPowerIndicator(body));
    }

    //if only the power of the gravity needs to be modified, only this method needs to be overridden
    protected virtual float GetGravityPowerIndicator(GameObject body)
    {
        return GravityPowerIndicator;
    }
}
