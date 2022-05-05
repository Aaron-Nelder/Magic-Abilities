using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* 	
*	Spawns a grenade infrot of the camera while 'E' is held.
*	The grenade will be thrown when 'E' is released.
*/
public class ThrowGreande : MonoBehaviour
{
	[SerializeField] GameObject currentGrenade;
	[SerializeField] float throwForce = 10f;
	[SerializeField] float maxScale = 2f;
	[SerializeField] float attractionRadiusMultiplier = 5f;
	[SerializeField] float scaleIncreasePerSec = 0.5f;
	[SerializeField] float throwCooldown = 1f;
	[SerializeField] float heldGrenadeDistance = 3f;
	[SerializeField] float stabalizingForce = 20f;
	Transform _cameraTransform;

	GameObject heldGrenade;
	Rigidbody heldRigidbody;
	bool canThrow = true;
	Coroutine GrowGrenadeRoutine;

	GravityGrenade grenadeStates;

	// Start is called before the first frame update
	void Start()
	{
		_cameraTransform = Camera.main.transform;
	}

	IEnumerator WaitForThrowDelay()
	{
		canThrow = false;
		yield return new WaitForSeconds(throwCooldown);
		canThrow = true;
	}

	// Grows the grenade over time as well as the grenades trigger collider
	IEnumerator GrowGrenade()
	{
		float currentScale = heldGrenade.transform.localScale.x;
		while (currentScale < maxScale)
		{
			currentScale += scaleIncreasePerSec * Time.deltaTime;
			heldGrenade.transform.localScale = Vector3.one * currentScale;
			grenadeStates.UpdateTriggerRadius(currentScale * attractionRadiusMultiplier);
			yield return null;
		}
		ThrowGrenade();
	}

	// Update is called once per frame
	void Update()
	{
		// Moves the greande infront of the camera
		if (heldGrenade != null)
		{
			Vector3 targetPos = _cameraTransform.position + _cameraTransform.forward * heldGrenadeDistance;
			heldRigidbody.velocity = (targetPos - heldGrenade.transform.position) * stabalizingForce / heldRigidbody.mass;
			heldRigidbody.angularVelocity = Vector3.zero;
			heldGrenade.transform.LookAt(_cameraTransform);
		}
	}

	void OnThrowGrenade(InputValue value)
	{
		if (!this.enabled) return;

		if (heldGrenade != null && value.Get<float>() <= 0)
		{
			ThrowGrenade();
			return;
		}

		// Spawns a grenade and begins growing it
		if (canThrow && value.Get<float>() >= 1)
		{
			if (currentGrenade == null)
			{
				Debug.LogError("No grenade prefab set!");
				return;
			}
			heldGrenade = Instantiate(currentGrenade, _cameraTransform.position + _cameraTransform.forward * heldGrenadeDistance, transform.rotation);
			heldRigidbody = heldGrenade.GetComponent<Rigidbody>();
			heldRigidbody.useGravity = false;

			grenadeStates = heldGrenade.GetComponent<GravityGrenade>();

			if (GrowGrenadeRoutine == null)
				GrowGrenadeRoutine = StartCoroutine(GrowGrenade());
		}
	}

	// Throws the grenade from the camera and starts the fuse
	void ThrowGrenade()
	{
		if (GrowGrenadeRoutine != null)
			StopCoroutine(GrowGrenadeRoutine);

		GrowGrenadeRoutine = null;

		heldRigidbody.AddForce(_cameraTransform.transform.forward * throwForce, ForceMode.Impulse);
		heldRigidbody.useGravity = true;

		heldGrenade = null;
		heldRigidbody = null;

		grenadeStates.StartFuse();

		StartCoroutine(WaitForThrowDelay());
	}
}
