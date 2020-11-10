using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    [SerializeField]
    private GameObject initialAttractor;

    [SerializeField]
    private float attractorPowerIndicator = 1;

    private Dictionary<Guid, KeyValuePair<GravityAttractor, float>> gravityAttractorDictionary;
    private Rigidbody rigidbody;

    private void Start()
    {
        gravityAttractorDictionary = new Dictionary<Guid, KeyValuePair<GravityAttractor, float>>();
        //it is not mandatory to add an initial attractor
        if (initialAttractor != null)
        {
            GravityAttractor gravityAttractor = initialAttractor.GetComponent<GravityAttractor>();
            gravityAttractorDictionary.Add(gravityAttractor.guid, new KeyValuePair<GravityAttractor, float>(gravityAttractor, 0f));
        }
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        //CheckGravityAttractors();
        if (gravityAttractorDictionary.Count != 0)
        {
            gravityAttractorDictionary.Select(gA => gA.Value.Key).ToList().ForEach(x => x.Attract(transform.gameObject));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
}