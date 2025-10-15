using System.Collections.Generic;
using UnityEngine;

public class FishDetector : Detector
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask fishLayer;
    [SerializeField] private bool showGizmos = true;

    private Collider2D[] detectedFishes;

    public override void Detect(AIData aiData)
    {
        // Limpia listas previas
        aiData.targets.Clear();
        aiData.threats.Clear();

        // Detecta todos los peces cercanos
        detectedFishes = Physics2D.OverlapCircleAll(transform.position, detectionRadius, fishLayer);

        foreach (var col in detectedFishes)
        {
            if (col.gameObject == gameObject) continue; // ignora a s� mismo

            float myScale = transform.localScale.x;
            float otherScale = col.transform.localScale.x;

            if (otherScale < myScale)
            {
                // Soy m�s grande -> es presa
                aiData.targets.Add(col.transform);
            }
            else if (otherScale > myScale)
            {
                // Es m�s grande -> es amenaza
                aiData.threats.Add(col.transform);
            }
        }

        // Limpia el target actual si desapareci�
        if (aiData.currentTarget != null && !aiData.targets.Contains(aiData.currentTarget))
            aiData.currentTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
