using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPlanetGravityAttractor : PlanetGravityAttractor
{
    [SerializeField]
    private bool isGravityDirectionFixed = false;

    [SerializeField]
    private LayerMask groundedMask;

    protected override Vector3 GetGravityDirection(GameObject body)
    {
        if (isGravityDirectionFixed)
        {
            Ray ray = new Ray(body.transform.position, -body.transform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, 2 + .1f, groundedMask))
            {
                return hit.normal;
            }
            else
            {
                return (body.transform.position - transform.position).normalized;
            }
        }
        else
        {
            return (body.transform.position - transform.position).normalized;
        }
    }
}