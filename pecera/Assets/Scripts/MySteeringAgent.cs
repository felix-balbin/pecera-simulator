using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MySteeringAgent : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Wander Settings")]
    [SerializeField] private float wanderJitter = 0.6f;
    [SerializeField] private float wanderRadius = 1.2f;
    [SerializeField] private float wanderDistance = 1.5f;

    private Rigidbody2D rb;
    private AIData aiData;
    private SteeringBehaviour[] steeringBehaviours;
    private Detector[] detectors;

    private Vector2 desiredDirection;
    private Vector2 lastDirection;
    private string currentState = "Wander";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aiData = GetComponent<AIData>();

        // Busca detectores y behaviours en hijos
        detectors = GetComponentsInChildren<Detector>();
        steeringBehaviours = GetComponentsInChildren<SteeringBehaviour>();

        desiredDirection = Random.insideUnitCircle.normalized;
        lastDirection = desiredDirection;
    }

    private void Update()
    {
        // --- 1 Detectar entorno (player / obstáculos)
        foreach (var detector in detectors)
            detector.Detect(aiData);

        // --- 2 Elegir comportamiento según detección
        SelectBehaviour();

        // --- 3 Aplicar movimiento
        MoveAgent();
    }

    private void SelectBehaviour()
    {
        Transform closestFish = null;
        float closestDist = Mathf.Infinity;

        // Buscar el pez más cercano dentro del rango, en el mismo layer
        Collider2D[] nearbyFish = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayer);

        foreach (var fish in nearbyFish)
        {
            if (fish.transform == transform) continue; // Ignora a sí mismo

            float dist = Vector2.Distance(transform.position, fish.transform.position);
            if (dist < closestDist)
            {
                closestFish = fish.transform;
                closestDist = dist;
            }
        }

        // Si no hay nadie cerca -> Wander
        if (closestFish == null)
        {
            currentState = "Wander";
            ApplyBehaviour(typeof(WanderBehaviour));
            return;
        }

        // Comparar tamaños
        float mySize = transform.localScale.x;
        float otherSize = closestFish.localScale.x;

        if (mySize > otherSize * 1.1f)
        {
            //  Más grande -> persigue al pequeño
            aiData.targets = new List<Transform> { closestFish };
            aiData.threats = new List<Transform>();
            currentState = "Seek";
            ApplyBehaviour(typeof(SeekBehaviour));
        }
        else if (mySize * 1.1f < otherSize)
        {
            // Más pequeño -> huye del grande
            aiData.threats = new List<Transform> { closestFish };
            aiData.targets = new List<Transform>();
            currentState = "Flee";
            ApplyBehaviour(typeof(FleeBehaviour));
            moveSpeed = 3.5f;
        }
        else
        {
            // Tamaños similares->sigue vagando
            aiData.targets.Clear();
            aiData.threats.Clear();
            currentState = "Wander";
            ApplyBehaviour(typeof(WanderBehaviour));
        }
    }

    // Variables para debug visual
    private Vector2 debugForward, debugLeft, debugRight;
    private float debugRayDist;

    private Vector2 lastAvoidanceDir = Vector2.zero;
    private float stuckTimer = 0f;

    private void ApplyBehaviour(System.Type behaviourType)
    {
        // --- 1️⃣ Ejecutar el behaviour activo (Wander / Seek / Flee) ---
        float[] danger = new float[Directions.eightDirections.Count];
        float[] interest = new float[Directions.eightDirections.Count];

        foreach (var behaviour in steeringBehaviours)
        {
            if (behaviour.GetType() == behaviourType)
            {
                (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
            }
        }

        // Dirección base del comportamiento actual
        Vector2 interestDir = Vector2.zero;
        for (int i = 0; i < Directions.eightDirections.Count; i++)
            interestDir += Directions.eightDirections[i] * interest[i];
        interestDir.Normalize();

        // --- 2️⃣ Evasión mejorada de obstáculos ---
        Vector2 forward = transform.up;
        Vector2 origin = transform.position;
        float rayDistance = detectionRange * 0.8f;

        Vector2[] directions =
        {
        forward,
        Quaternion.Euler(0,0,45)*forward,
        Quaternion.Euler(0,0,-45)*forward,
        Quaternion.Euler(0,0,90)*forward,
        Quaternion.Euler(0,0,-90)*forward
    };

        RaycastHit2D[] hits = new RaycastHit2D[directions.Length];
        bool obstacleDetected = false;
        Vector2 avoidanceDir = Vector2.zero;

        for (int i = 0; i < directions.Length; i++)
        {
            hits[i] = Physics2D.Raycast(origin, directions[i], rayDistance, obstacleLayer);
            if (hits[i].collider != null)
            {
                obstacleDetected = true;
                avoidanceDir += -directions[i]; // sumar dirección opuesta al obstáculo
            }
        }

        // Guardar para gizmos
        debugForward = forward;
        debugLeft = directions[1];
        debugRight = directions[2];
        debugRayDist = rayDistance;

        // --- 3️⃣ Si choca con muchos rayos → generar dirección de escape ---
        if (obstacleDetected)
        {
            if (avoidanceDir.magnitude < 0.1f)
            {
                // Encerrado → elige un ángulo aleatorio hacia atrás
                float randomAngle = Random.Range(120f, 160f);
                float side = Random.value > 0.5f ? 1f : -1f;
                avoidanceDir = Quaternion.Euler(0, 0, randomAngle * side) * -forward;
            }

            lastAvoidanceDir = avoidanceDir.normalized;
            stuckTimer = 0.5f; // mantiene la evasión por un corto tiempo
        }

        // --- 4️⃣ Mantener evasión si aún está saliendo de una esquina ---
        if (stuckTimer > 0)
        {
            stuckTimer -= Time.deltaTime;
            desiredDirection = Vector2.Lerp(interestDir, lastAvoidanceDir, 0.9f).normalized;
        }
        else
        {
            if (avoidanceDir != Vector2.zero)
                desiredDirection = Vector2.Lerp(interestDir, avoidanceDir.normalized, 0.8f).normalized;
            else
                desiredDirection = interestDir;
        }

        if (desiredDirection == Vector2.zero)
            desiredDirection = lastDirection;
    }

    private void MoveAgent()
    {
        if (desiredDirection == Vector2.zero)
            desiredDirection = lastDirection; // Evita que se quede quieto si no hay dirección

        // Rotar suavemente hacia la dirección deseada
        float angle = Mathf.Atan2(desiredDirection.y, desiredDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Avanzar en la dirección actual
        rb.linearVelocity = transform.up * moveSpeed;

        lastDirection = desiredDirection;
    }
    // --- 4️ Dibujar raycasts de depuración en escena ---
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, debugForward * debugRayDist);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, debugLeft * (debugRayDist * 0.8f));

        Gizmos.color = Color.rebeccaPurple;
        Gizmos.DrawRay(transform.position, debugRight * (debugRayDist * 0.8f));
    }
}
