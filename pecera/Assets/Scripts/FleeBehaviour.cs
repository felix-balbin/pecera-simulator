using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FleeBehaviour : SteeringBehaviour
{
    [Header("Flee Settings")]
    [SerializeField] private float fleeRange = 8f;          //Rango en el que el agente reacciona al perseguidor
    [SerializeField] private float safeDistance = 12f;      //Distancia donde se considera a salvo
    [SerializeField] private bool showGizmo = true;         //Mostrar visualización en escena

    private Vector2 threatPositionCached;
    private float[] interestsTemp;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        //Si no hay amenazas detectadas, salir
        if (aiData.threats == null || aiData.threats.Count == 0)
        {
            return (danger, interest);
        }

        //Buscar el perseguidor más cercano
        Transform closestThreat = aiData.threats
            .OrderBy(t => Vector2.Distance(t.position, transform.position))
            .FirstOrDefault();

        if (closestThreat == null)
            return (danger, interest);

        float distanceToThreat = Vector2.Distance(transform.position, closestThreat.position);

        //Si la amenaza está fuera del rango de acción, no huir
        if (distanceToThreat > fleeRange)
        {
            return (danger, interest);
        }

        //Guardar la posición del perseguidor para dibujar gizmos
        threatPositionCached = closestThreat.position;

        //Calcular dirección opuesta al perseguidor
        Vector2 directionAway = ((Vector2)transform.position - (Vector2)closestThreat.position).normalized;

        //Reforzar interés en direcciones opuestas al perseguidor
        for (int i = 0; i < interest.Length; i++)
        {
            float dot = Vector2.Dot(directionAway, Directions.eightDirections[i]);

            if (dot > 0 && dot > interest[i])
            {
                interest[i] = dot;
            }
        }

        interestsTemp = interest;
        return (danger, interest);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        //Dibujar el rango de huida
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, fleeRange);

        //Dibujar la posición del perseguidor
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(threatPositionCached, 0.2f);

        //Dibujar direcciones de interés
        if (Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(transform.position, Directions.eightDirections[i] * interestsTemp[i] * 2);
            }

            //Línea de huida (del agente alejándose de la amenaza)
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, threatPositionCached);
        }
    }
}
