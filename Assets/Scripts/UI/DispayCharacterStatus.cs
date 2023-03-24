using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DispayCharacterStatus : MonoBehaviour
{
	[SerializeField] TMP_Text idTextField;
	[SerializeField] TMP_Text hpTextField;
	[SerializeField] TMP_Text stateTextField;

	readonly string idTextFormat = "ID: {0}{1}";
	readonly string hpTextFormat = "HP: {0}";
	readonly string stateTextFormat = "{0}";

	[SerializeField] FiniteStateMachine fsm;
	[SerializeField] DamageableObject health;

	private void Update()
	{
		idTextField.text = string.Format(idTextFormat, fsm.CharacterType, fsm.ID);
		hpTextField.text = string.Format(hpTextFormat, health.CurrentHealth);
		stateTextField.text = string.Format(stateTextFormat, fsm.CurrentState);
	}
}
