using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravityAttractor : GravityAttractor
{
    protected override float GetGravityPowerIndicator(GameObject body)
    {
        float dist = Vector3.Distance(transform.position, body.transform.position) / GravityPowerIndicator;
        return dist;
    }
}