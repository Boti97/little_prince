using Bolt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : EntityBehaviour<IPlayerState>
{
    [SerializeField]
    private GameObject initialAttractor;

    private Dictionary<Guid, KeyValuePair<GravityAttractor, float>> gravityAttractorDictionary;

    public override void Attached()
    {
        if (!entity.IsOwner) return;

        gravityAttractorDictionary = new Dictionary<Guid, KeyValuePair<GravityAttractor, float>>();
        //it is not mandatory to add an initial attractor
        if (initialAttractor != null)
        {
            GravityAttractor gravityAttractor = initialAttractor.GetComponent<GravityAttractor>();
            gravityAttractorDictionary.Add(gravityAttractor.guid, new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f));
        }
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void FixedUpdate()
    {
        if (!entity.IsOwner) return;

        //CheckGravityAttractors();
        if (gravityAttractorDictionary.Count != 0)
        {
            gravityAttractorDictionary.Select(gA => gA.Value.Key).ToList().ForEach(x => x.Attract(transform.gameObject));
        }
        else
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity / 1.02f;
            GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity / 1.02f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!entity.IsOwner) return;

        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("GravityField")))
        {
            GravityAttractor gravityAttractor = other.gameObject.GetComponentInParent<GravityAttractor>();
            if (gravityAttractorDictionary.ContainsKey(gravityAttractor.guid))
            {
                //if it's already in the list we only need to set the time to 0
                gravityAttractorDictionary[gravityAttractor.guid] = new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f);
            }
            else
            {
                //if it's not already in the list we  need to add a new entry
                gravityAttractorDictionary.Add(gravityAttractor.guid, new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!entity.IsOwner) return;

        GravityAttractor gravityAttractor = other.gameObject.GetComponentInParent<GravityAttractor>();

        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("GravityField")))
        {
            gravityAttractorDictionary.Remove(gravityAttractor.guid);
        }
    }

    //checks if one or more attractors are out of our object's scope (it left them a while ago) and removes them
    private void CheckGravityAttractors()
    {
        List<Guid> attractorsToRemove = gravityAttractorDictionary
            .Where(gA => gA.Value.Value != 0f)
            .Where(gA => Time.realtimeSinceStartup - gA.Value.Value > 1)
            .Select(gA => gA.Key).ToList();
        attractorsToRemove.ForEach(guid => gravityAttractorDictionary.Remove(guid));
    }

    public int AttractorCount()
    {
        if (!entity.IsOwner) return 0;
        return gravityAttractorDictionary.Count;
    }
}