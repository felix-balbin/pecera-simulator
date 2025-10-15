using System.Collections.Generic;
using UnityEngine;

public class TargetDetector : Detector
{
    [SerializeField] private float targetDetectionRange = 6f;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private bool useLineOfSight = false;
    [SerializeField] private bool showGizmos = true;

    private readonly List<Transform> detectedTargets = new();

    public override void Detect(AIData aiData)
    {
        detectedTargets.Clear();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetDetectionRange, targetLayerMask);

        foreach (var hit in hits)
        {
            if (hit.transform == transform)
                continue;

            // Si se requiere l�nea de visi�n, se hace un raycast
            if (useLineOfSight)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                RaycastHit2D sight = Physics2D.Raycast(transform.position, dir, dist, obstacleLayerMask);
                if (sight.collider != null)
                    continue; // obst�culo bloqueando
            }

            detectedTargets.Add(hit.transform);
        }

        aiData.targets = detectedTargets.Count > 0 ? new List<Transform>(detectedTargets) : null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, targetDetectionRange);

        if (detectedTargets == null) return;
        Gizmos.color = Color.magenta;
        foreach (var t in detectedTargets)
            Gizmos.DrawSphere(t.position, 0.2f);
    }
}
