using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
*	When holding down the left mouse button, the player can grow an object as long as it has a rigidbody.
*	When holding down the right mouse button, the player can shrink an object as long as it has a rigidbody.
*/
public class ScaleGun : MonoBehaviour
{
	bool shrinking = false;
	bool growing = false;
	[SerializeField] float range = 10f;
	[SerializeField] float shrinkAndGrowSpeed = 1f;
	[SerializeField] float minimumSize = 0.05f;

	Transform _cameraTransform;

	[SerializeField] Image crosshair;

	// Start is called before the first frame update
	void Start()
	{
		_cameraTransform = Camera.main.transform;
		crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<Image>();
	}

	// Update is called once per frame
	void Update()
	{
		RaycastHit hit;
		if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, range))
		{
			if (!hit.transform.GetComponent<Rigidbody>()) return;

			crosshair.color = Color.green;

			Rigidbody rigidbody = hit.transform.GetComponent<Rigidbody>();

			float startMass = rigidbody.mass;
			float startScale = hit.transform.localScale.x;

			// Shrinks the object and decreases its mass
			if (shrinking)
			{
				hit.transform.localScale -= Vector3.one * shrinkAndGrowSpeed * Time.deltaTime;

				rigidbody.mass = (hit.transform.localScale.x - startScale) + startMass;

				// If the object is too small, stop shrinking
				if (hit.transform.localScale.x <= minimumSize)
				{
					hit.transform.localScale = new Vector3(minimumSize, minimumSize, minimumSize);

					// Update the mass
					rigidbody.mass = (hit.transform.localScale.x - startScale) + startMass;
					return;
				}
			}

			// Grows the object and increases its mass
			else if (growing)
			{
				hit.transform.localScale += Vector3.one * shrinkAndGrowSpeed * Time.deltaTime;
				rigidbody.mass = (hit.transform.localScale.x - startScale) + startMass;
			}
		}
		else
			crosshair.color = Color.white;
	}

	// Sets the shrinking bool
	void OnShrink(InputValue value)
	{
		if (!this.enabled) return;

		shrinking = value.isPressed;
	}

	// Sets the growing bool
	void OnGrow(InputValue value)
	{
		if (!this.enabled) return;

		growing = value.isPressed;
	}

	void OnDisable()
	{
		crosshair.color = Color.white;
	}
}
