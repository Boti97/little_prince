using UnityEngine;

public class PlanetGravityAttractor : GravityAttractor
{
    protected override float GetGravityPowerIndicator(GameObject body)
    {
        return Vector3.Distance(transform.position, body.transform.position) / GravityPowerIndicator;
    }

    protected override Vector3 GetGravityDirection(GameObject body)
    {
        return (body.transform.position - transform.position).normalized;
    }
}