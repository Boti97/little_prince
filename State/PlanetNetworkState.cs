using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetNetworkState : EntityBehaviour<IPlanetState>, IObjectState
{
    private Guid planetId;

    public Guid PlanetId { get => planetId; set => planetId = value; }

    public override void Attached()
    {
        state.SetTransforms(state.PlanetTransform, transform);
        state.PlanetRotation = transform.rotation;

        if (entity.IsOwner)
        {
            PlanetId = Guid.NewGuid();
            state.PlanetId = PlanetId;
        }
    }

    public Guid GetGuid()
    {
        return planetId;
    }

    private void Update()
    {
        if (!entity.IsOwner)
        {
            PlanetId = state.PlanetId;
            transform.rotation = state.PlanetRotation;
            return;
        }
    }
}