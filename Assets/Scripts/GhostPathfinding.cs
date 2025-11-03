using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GhostPathfinding : MonoBehaviour {

    GridMap GridMap;

    Queue<GhostPath> GhostPathQueue = new Queue<GhostPath>();
    GhostPath currentGhostPath;

    static GhostPathfinding instance;
    AStar astar;

    bool isProcessingPath;


    private void Awake()
    {
        instance = this;
        astar = GetComponent<AStar>();
    }


    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        GhostPath newghostPath = new GhostPath(pathStart, pathEnd, callback);
        instance.GhostPathQueue.Enqueue(newghostPath);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && GhostPathQueue.Count > 0) 
        {
            currentGhostPath = GhostPathQueue.Dequeue();
            isProcessingPath = true;    
            astar.StartFindPath(currentGhostPath.pathStart, currentGhostPath.pathEnd);
        }
    }


    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentGhostPath.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct GhostPath
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public GhostPath(Vector3 startPos, Vector3 endPos, Action<Vector3[], bool> _callback)
        {
            pathStart = startPos;
            pathEnd = endPos;
            callback = _callback;
        }
    }
}
