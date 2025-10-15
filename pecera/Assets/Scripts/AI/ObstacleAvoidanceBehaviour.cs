using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField] private float radius = 1.8f;
    [SerializeField] private float agentColliderSize = 0.4f;
    [SerializeField] private bool showGizmo = true;

    private float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        if (aiData.obstacles == null || aiData.obstacles.Length == 0) return (danger, interest);

        for (int k = 0; k < aiData.obstacles.Length; k++)
        {
            var obstacleCollider = aiData.obstacles[k];
            if (obstacleCollider == null) continue;
            if (obstacleCollider.transform == transform) continue;

            Vector2 closest = obstacleCollider.ClosestPoint(transform.position);
            Vector2 dir = (closest - (Vector2)transform.position);
            float dist = dir.magnitude;
            if (dist <= 0.0001f) continue;

            float weight = dist <= agentColliderSize ? 1f : Mathf.Clamp01((radius - dist) / radius);
            Vector2 dirN = dir.normalized;

            for (int i = 0; i < Directions.eightDirections.Count; i++)
            {
                float dot = Vector2.Dot(dirN, Directions.eightDirections[i]);
                float val = Mathf.Clamp01(dot * weight);
                if (val > danger[i]) danger[i] = val;
            }
        }

        dangersResultTemp = danger;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo || dangersResultTemp == null || !Application.isPlaying) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < dangersResultTemp.Length; i++)
        {
            Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * dangersResultTemp[i] * 1.5f);
        }
    }
}

public static class Directions
{
    public static List<Vector2> eightDirections = new List<Vector2>{
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
}