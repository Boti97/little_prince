using Bolt;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : EntityBehaviour<ICharacterState>
{
    [SerializeField]
    private GameObject initialAttractor;

    private Dictionary<Guid, KeyValuePair<GravityAttractor, float>> gravityAttractorDictionary;

    public override void Attached()
    {
        if (!entity.IsOwner) return;

        gravityAttractorDictionary = new Dictionary<Guid, KeyValuePair<GravityAttractor, float>>();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void FixedUpdate()
    {
        if (!entity.IsOwner) return;

        //CheckGravityAttractors();
        if (gravityAttractorDictionary.Count != 0)
        {
            gravityAttractorDictionary.Select(entry => entry.Value.Key).ToList().ForEach(gravityAttractor => gravityAttractor.Attract(transform.gameObject));
        }
        else
        {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity / 1.02f;
            GetComponent<Rigidbody>().angularVelocity = GetComponent<Rigidbody>().angularVelocity / 1.02f;
        }
    }

    public int AttractorCount()
    {
        if (!entity.IsOwner) return 0;
        return gravityAttractorDictionary.Count;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!entity.IsOwner) return;

        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("GravityField")))
        {
            IObjectState objectState = other.gameObject.GetComponentInParent<IObjectState>();
            GravityAttractor gravityAttractor = other.gameObject.GetComponentInParent<GravityAttractor>();
            if (gravityAttractorDictionary.ContainsKey(objectState.GetGuid()))
            {
                //if it's already in the list we only need to set the time to 0
                gravityAttractorDictionary[objectState.GetGuid()] = new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f);
            }
            else
            {
                //if it's not already in the list we  need to add a new entry
                gravityAttractorDictionary.Add(objectState.GetGuid(), new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!entity.IsOwner) return;

        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("GravityField")))
        {
            IObjectState objectState = other.gameObject.GetComponentInParent<IObjectState>();
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("GravityField")))
            {
                gravityAttractorDictionary.Remove(objectState.GetGuid());
            }
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
}