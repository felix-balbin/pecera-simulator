using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public List<Transform> targets = new();
    public List<Transform> threats = new();
    public Collider2D[] obstacles;


    public Transform currentTarget;

    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
    public int GetThreatsCount() => threats == null ? 0 : threats.Count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}