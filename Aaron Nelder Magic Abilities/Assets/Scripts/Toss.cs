using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
*	While looking at an object with a rigidbody hold the left mouse button to levitate it.
*	The object will be levitated in the direction of the camera.
*   Then release the left mouse button to stop levitating and throw the object.
*/
public class Toss : MonoBehaviour
{
	[SerializeField] GameObject heldObject;
	[SerializeField] Rigidbody heldRigidbody;
	[SerializeField] float throwForce = 50f;
	[SerializeField] float heldDistance = 5f;
	[SerializeField] float grabDistance = 5f;
	[SerializeField] float stabalizingForce = 10f;
	Transform _mainCameraTransform;

	// Start is called before the first frame update
	void Start()
	{
		_mainCameraTransform = Camera.main.transform;
	}

	// Update is called once per frame
	void Update()
	{
		MoveHeldObjectToPosition();
	}

	// Moves the held object to the position of the mouse cursor.
	void MoveHeldObjectToPosition()
	{
		if (heldObject == null) return;

		Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * heldDistance;
		heldRigidbody.velocity = (targetPos - heldObject.transform.position) * stabalizingForce / heldRigidbody.mass;
		heldRigidbody.angularVelocity = Vector3.zero;


		heldObject.transform.LookAt(_mainCameraTransform);
	}

	void OnGrab(InputValue value)
	{
		if (!this.enabled) return;

		if (heldObject != null)
		{
			ThrowHeldObject();
			return;
		}

		RaycastHit hit;
		if (Physics.Raycast(_mainCameraTransform.position, _mainCameraTransform.forward, out hit, grabDistance))
		{
			if (hit.transform.gameObject.tag == "Throwable" && hit.transform.gameObject.GetComponent<Rigidbody>() != null)
				HoldGameObject(hit.transform.gameObject);
		}
	}

	// Holds the game object 
	void HoldGameObject(GameObject gameObjectToHold)
	{
		heldObject = gameObjectToHold;
		heldRigidbody = heldObject.GetComponent<Rigidbody>();
		heldRigidbody.useGravity = false;
	}

	// Release the held object and throw it.
	void ThrowHeldObject()
	{
		heldRigidbody.useGravity = true;
		heldRigidbody.constraints = RigidbodyConstraints.None;
		heldRigidbody.AddForce(_mainCameraTransform.forward * throwForce, ForceMode.Impulse);
		heldObject = null;
		heldRigidbody = null;
	}

	void OnDisable()
	{
		if (heldObject != null)
			ThrowHeldObject();
	}

}