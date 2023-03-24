using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
	[SerializeField] string characterType;
	[SerializeField] string entryState;
	int id;
	AIState activeState;

	static int idCounter = 0;

	public int ID { get { return id; } }
	public string CharacterType { get { return characterType; } }
	public string CurrentState { get { return activeState != null ? activeState.StateName : ""; } }

	private void Awake()
	{
		id = GenerateId();
		AIState[] states = GetComponents<AIState>();
		AIState activeState = null;
		foreach (AIState state in states)
		{
			state.enabled = false;
			if(entryState == state.StateName)
			{
				activeState = state;
			}
		}
		SetActiveState(activeState);
	}

	void Update()
	{
		if (activeState == null)
			return;

		AIState newState = activeState.CheckConditions();
		if (newState != null && newState != activeState)
		{
			SetActiveState(newState);
		}
	}

	void SetActiveState(AIState newState)
	{
		if(activeState != newState && newState != null)
		{
			if (activeState != null)
			{
				activeState.enabled = false;
			}
			activeState = newState;
			activeState.enabled = true;
		}
	}

	static int GenerateId()
	{
		return idCounter++;
	}
}