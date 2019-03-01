/*  A simple funtion that removes an object from the scene after X seconds
 *  Once done,this event will destroy this gameobject
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedRemoval : MonoBehaviour {

    public GameObject specifiedGameObject;

    [Header("General Variables")]
    public bool shouldDeactivate;
    [Range(0.1f, 10f)]
    public float timeToRemove;

    private void Start()
    {
        if(shouldDeactivate)
        {
            Invoke("DeactivateSpecifiedObject", timeToRemove);
        }
        else
        {
            Destroy(specifiedGameObject, timeToRemove);
        }
    }

	private void Update()
	{
        if(specifiedGameObject == null)
        {
            Destroy(gameObject);
        }
        else if(specifiedGameObject.activeInHierarchy == false)
        {
            Destroy(gameObject);
        }
	}

	private void DeactivateSpecifiedObject()
    {
        specifiedGameObject.SetActive(false);
    }
}
