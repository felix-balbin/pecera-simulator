using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{
    [Header("Seek Settings")]
    [SerializeField]
    private float targetReachedThreshold = 0.5f;

    [SerializeField]
    private float targetLostRange = 10f; // Nuevo: rango máximo de seguimiento

    [SerializeField]
    private bool showGizmo = true;

    private bool reachedLastTarget = true;

    // Gizmo parameters
    private Vector2 targetPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        // Si no hay targets visibles, detener y limpiar
        if (aiData.targets == null || aiData.targets.Count == 0)
        {
            aiData.currentTarget = null;
            reachedLastTarget = true;
            return (danger, interest);
        }

        // Si el target actual está fuera del rango o ya se alcanzó, buscar uno nuevo
        if (aiData.currentTarget == null || reachedLastTarget ||
            Vector2.Distance(transform.position, aiData.currentTarget.position) > targetLostRange ||
            !aiData.targets.Contains(aiData.currentTarget))
        {
            // Buscar el más cercano dentro del rango
            var newTarget = aiData.targets
                .Where(t => Vector2.Distance(t.position, transform.position) <= targetLostRange)
                .OrderBy(t => Vector2.Distance(t.position, transform.position))
                .FirstOrDefault();

            if (newTarget != null)
            {
                aiData.currentTarget = newTarget;
                reachedLastTarget = false;
            }
            else
            {
                aiData.currentTarget = null;
                reachedLastTarget = true;
                return (danger, interest);
            }
        }

        // Actualizar posición caché si el target sigue visible
        if (aiData.currentTarget != null && aiData.targets.Contains(aiData.currentTarget))
        {
            targetPositionCached = aiData.currentTarget.position;
        }

        // Comprobar si se alcanzó el objetivo
        if (Vector2.Distance(transform.position, targetPositionCached) < targetReachedThreshold)
        {
            reachedLastTarget = true;
            aiData.currentTarget = null;
            return (danger, interest);
        }

        // Calcular dirección hacia el objetivo
        Vector2 directionToTarget = (targetPositionCached - (Vector2)transform.position).normalized;

        for (int i = 0; i < interest.Length; i++)
        {
            float result = Vector2.Dot(directionToTarget, Directions.eightDirections[i]);
            if (result > 0)
            {
                if (result > interest[i])
                    interest[i] = result;
            }
        }

        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        // Dibujar posición del target
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPositionCached, 0.2f);

        // Dibujar líneas de interés
        if (Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestsTemp[i] * 2);
            }

            // Si tiene target activo, marcarlo en rojo
            if (!reachedLastTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetPositionCached);
            }
        }

        // Dibuja el rango de seguimiento
        Gizmos.color = new Color(0, 0.5f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, targetLostRange);
    }
}
