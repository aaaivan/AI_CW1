using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
	[SerializeField] string characterType;
	[SerializeField] string entryState;
	[SerializeField] float sightDistance = 60;


	int id;
	Dictionary<string, AIState> aiStates = new Dictionary<string, AIState>();
	AIState activeState = null;

	static int idCounter = 0;

	public int ID { get { return id; } }
	public string CharacterType { get { return characterType; } }
	public string CurrentState { get { return activeState == null ? "" : activeState.StateName; } }
	public float SightDistance { get { return sightDistance; } }


	private void Awake()
	{
		id = GenerateId();
		AIState[] states = GetComponents<AIState>();
		foreach (AIState state in states)
		{
			state.SetInactive(null);
			state.enabled = false;
			aiStates.Add(state.StateName, state);
		}
		if(aiStates.ContainsKey(entryState))
		{
			SetActiveState(aiStates[entryState]);
		}
		else
		{
			Debug.LogError("Invalid fsm entry state: " + entryState);
		}
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
				activeState.SetInactive(newState);
			}
			newState.SetActive(activeState);
			activeState = newState;
		}
	}

	public AIState GetStateByName(string name)
	{
		if(aiStates.ContainsKey(name))
		{
			return aiStates[name];
		}
		return null;
	}

	static int GenerateId()
	{
		return idCounter++;
	}
}
