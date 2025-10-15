using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderBehaviour : SteeringBehaviour
{
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 2f;        // Radio del c�rculo donde se genera el punto de wander
    [SerializeField] private float wanderDistance = 3f;      // Distancia frente al agente donde se proyecta el c�rculo
    [SerializeField] private float wanderJitter = 1f;        // Intensidad del movimiento aleatorio
    [SerializeField] private bool showGizmo = true;          // Mostrar visualizaci�n Gizmo

    private Vector2 wanderTarget; // Objetivo temporal dentro del c�rculo
    private Vector2 lastDir;      // �ltima direcci�n, para suavizar el movimiento
    private float[] interestsTemp;

    private void Start()
    {
        // Inicializa el wander target en una direcci�n aleatoria
        wanderTarget = Random.insideUnitCircle * wanderRadius;
        lastDir = Random.insideUnitCircle.normalized;
    }

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        // Usar el transform ra�z (EnemyFish), no el del hijo
        Transform agent = transform.root;

        // Cambia ligeramente la direcci�n aleatoria con jitter (suavizado)
        wanderTarget += new Vector2(Random.Range(-1f, 1f) * wanderJitter, Random.Range(-1f, 1f) * wanderJitter);
        wanderTarget = wanderTarget.normalized * wanderRadius;

        // Calcula el punto final del c�rculo proyectado frente al agente
        Vector2 circleCenter = (Vector2)agent.position + (Vector2)agent.up * wanderDistance;
        Vector2 targetPosition = circleCenter + wanderTarget;

        // Direcci�n hacia el punto de wander
        Vector2 directionToTarget = (targetPosition - (Vector2)agent.position).normalized;

        // Suaviza la direcci�n final (reduce temblores)
        lastDir = Vector2.Lerp(lastDir, directionToTarget, 0.1f).normalized;

        // Actualiza inter�s en las 8 direcciones
        for (int i = 0; i < interest.Length; i++)
        {
            float dot = Vector2.Dot(lastDir, Directions.eightDirections[i]);
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

        // Usar transform.root tambi�n aqu�
        Transform agent = transform.root;
        Gizmos.color = Color.cyan;
        Vector2 circleCenter = (Vector2)agent.position + (Vector2)agent.up * wanderDistance;

        // Dibuja el c�rculo del wander
        Gizmos.DrawWireSphere(circleCenter, wanderRadius);

        // Dibuja el punto actual del wander target
        Gizmos.color = Color.magenta;
        Vector2 targetPosition = circleCenter + wanderTarget;
        Gizmos.DrawSphere(targetPosition, 0.1f);

        // Dibuja direcciones de inter�s
        if (Application.isPlaying && interestsTemp != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestsTemp.Length; i++)
            {
                Gizmos.DrawRay(agent.position, Directions.eightDirections[i] * interestsTemp[i] * 2);
            }
        }
    }
}
