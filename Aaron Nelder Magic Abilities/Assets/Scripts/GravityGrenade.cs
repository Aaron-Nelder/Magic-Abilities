using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Grows an attached sphere collider which is a trigger and if a gameObject with a rigidbody enters the trigger it will be attracted to the grenade.
* Once the grenades fuse has been startedit will explode after 5 seconds.
*/

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class GravityGrenade : MonoBehaviour
{
	[SerializeField] float attractionForce = 1;
	[SerializeField] SphereCollider trigger;
	[SerializeField] float explosionForceMultiplier = 50;

	List<Rigidbody> attractedRigidbodies = new List<Rigidbody>();

	void Start()
	{
		trigger.isTrigger = true;
	}

	public void StartFuse()
	{
		StartCoroutine(Fuse());
	}

	IEnumerator Fuse()
	{
		yield return new WaitForSeconds(5f);

		foreach (Rigidbody rb in attractedRigidbodies)
		{
			rb.velocity = Vector3.zero;

			// throws everything away from the grenade
			rb.AddForce((rb.transform.position - transform.position) * explosionForceMultiplier, ForceMode.Impulse);
		}
		Destroy(gameObject);
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) return;

		if (other.GetComponent<Rigidbody>())
		{
			if (!attractedRigidbodies.Contains(other.GetComponent<Rigidbody>()))
				attractedRigidbodies.Add(other.GetComponent<Rigidbody>());

			// Moves the object towards the grenade
			Vector3 targetPos = (transform.position - other.transform.position) * attractionForce;
			other.GetComponent<Rigidbody>().velocity = targetPos;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) return;

		if (other.GetComponent<Rigidbody>())
			attractedRigidbodies.Remove(other.GetComponent<Rigidbody>());
	}

	public void UpdateTriggerRadius(float newSize)
	{
		trigger.radius = newSize;
	}
}