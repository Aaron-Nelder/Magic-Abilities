using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/*
*	A very simple script that allows the player to switch between different abilities using the scroll wheel
*/
public class AbilitySwitcher : MonoBehaviour
{
	int currentAbility = 0;
	public Text abilityText;
	public ThrowGreande throwGreande;
	public Toss toss;
	public ScaleGun scaleGun;

	// Start is called before the first frame update
	void Awake()
	{
		abilityText = GameObject.FindGameObjectWithTag("UI text").GetComponent<Text>();
		throwGreande = GetComponent<ThrowGreande>();
		toss = GetComponent<Toss>();
		scaleGun = GetComponent<ScaleGun>();
		SetActiveAbility(currentAbility);
	}

	void OnScroll(InputValue value)
	{
		if (value.Get<float>() > 0)
		{
			currentAbility++;
			if (currentAbility > 2)
				currentAbility = 0;
		}
		if (value.Get<float>() < 0)
		{
			currentAbility--;
			if (currentAbility < 0)
				currentAbility = 2;
		}
		SetActiveAbility(currentAbility);
	}

	void SetActiveAbility(int ability)
	{

		if (abilityText == null)
		{
			print("Ability text object has not been set");
			return;
		}

		toss.enabled = false;
		throwGreande.enabled = false;
		scaleGun.enabled = false;
		switch (ability)
		{
			case 0:
				abilityText.text = "Levitation: Use the left moust button to pick up a red cube and use the left mouse button to throw a held object";
				toss.enabled = true;
				break;
			case 1:
				abilityText.text = "Gravity Grenade: Hold 'E' to equipt and grow the grenade, then release 'E'' to throw the grenade";
				throwGreande.enabled = true;
				break;
			case 2:
				abilityText.text = "Scaling Gun: Hold left mouse button to grow an object and Hold right mouse button to shrink it.";
				scaleGun.enabled = true;
				break;
		}
	}
}
