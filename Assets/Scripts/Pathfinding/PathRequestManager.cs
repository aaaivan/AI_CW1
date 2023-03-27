using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
	Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();
	PathRequest currentPathRequets;

	static PathRequestManager instance;
	public static PathRequestManager Instance { get { return instance; } }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void Update()
	{
		TryProcessNext();
	}

	public void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>> callback, PathfinderAgent agent, bool simplifyPath)
	{
		// if the agent is already in the queue,
		// just update its request instead of creating a new one
		bool found = false;
		foreach(var path in pathRequestsQueue)
		{
			if(path.agent == agent)
			{
				path.pathStart = pathStart;
				path.pathEnd = pathEnd;
				path.callback = callback;
				path.simplifyPath = simplifyPath;
				found = true;
				break;
			}
		}
		if(!found)
		{
			PathRequest pathRequest = new PathRequest(pathStart, pathEnd, callback, agent, simplifyPath);
			pathRequestsQueue.Enqueue(pathRequest);
		}
	}

	void TryProcessNext()
	{
		if(pathRequestsQueue.Count > 0)
		{
			currentPathRequets = pathRequestsQueue.Dequeue();

			if(currentPathRequets.agent != null)
			{
				List<Vector3> path = currentPathRequets.agent.FindPath(currentPathRequets.pathStart, currentPathRequets.pathEnd, currentPathRequets.simplifyPath);
				currentPathRequets.callback(path);
			}
		}
	}

	class PathRequest
	{
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<List<Vector3>> callback;
		public PathfinderAgent agent;
		public bool simplifyPath;

		public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<List<Vector3>> callback, PathfinderAgent agent, bool simplifyPath)
		{
			this.pathStart = pathStart;
			this.pathEnd = pathEnd;
			this.callback = callback;
			this.agent = agent;
			this.simplifyPath = simplifyPath;
		}
	}
}
