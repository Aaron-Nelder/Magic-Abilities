using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScaleGun : MonoBehaviour
{
	bool shrinking = false;
	bool growing = false;
	[SerializeField] float range = 10f;
	[SerializeField] float shrinkAndGrowSpeed = 5f;
	[SerializeField] float minimumSize = 0.05f;

	Transform _cameraTransform;

	[SerializeField] Image crosshair;

	// Start is called before the first frame update
	void Start()
	{
		_cameraTransform = Camera.main.transform;
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
				if (hit.transform.localScale.x <= minimumSize)
				{
					hit.transform.localScale = new Vector3(minimumSize, minimumSize, minimumSize);
					rigidbody.mass = (hit.transform.localScale.x - startScale) + startMass;
					return;
				}

				hit.transform.localScale -= Vector3.one * shrinkAndGrowSpeed * Time.deltaTime;

				rigidbody.mass = (hit.transform.localScale.x - startScale) + startMass;
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

	void OnShrink(InputValue value)
	{
		if (!this.enabled) return;

		shrinking = value.isPressed;
	}

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
