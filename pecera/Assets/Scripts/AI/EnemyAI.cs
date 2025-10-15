using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField] private List<Detector> detectors;

    [SerializeField] private AIData aiData;

    [SerializeField] private float detectionDelay = 0.05f, aiUpdateDelay = 0.06f;

    [SerializeField] private Transform pezSprite;
    private int direcion = 1;

    // Inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    [SerializeField] private Vector2 movementInput;

    [SerializeField] private ContextSolver movementDirectionSolver;

    bool following = false;
    private float tamano;
    Vector2 limitesPantalla;

    private void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
        //Tamano
        float tamanoAleatorio = Random.Range(0.3f, 2f);
        tamano = tamanoAleatorio;
        transform.localScale = new Vector3(tamanoAleatorio, tamanoAleatorio, tamanoAleatorio);
    
    }
    public float GetTamano()
    {
        return tamano;
    }
    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach (SteeringBehaviour behaviour in steeringBehaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest, aiData);
        }
    }

    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.position);
            if (!following)
            {
                following = true;
                StartCoroutine(Chase());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }

        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);

        // 🔄 Girar el sprite según la dirección del movimiento
        if (movementInput.x > 0.01f)
        {
            pezSprite.rotation = Quaternion.Euler(0, 0, 0);
            direcion = 1;
        }
        else if (movementInput.x < -0.01f)
        {
            pezSprite.rotation = Quaternion.Euler(0, 180, 0);
            direcion = -1;
        }

        // Limitar posición a la pantalla
        limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, limitesPantalla.x * -1 + 0.5f, limitesPantalla.x - 0.5f),
            Mathf.Clamp(transform.position.y, limitesPantalla.y * -1 + 0.5f, limitesPantalla.y - 0.5f),
            0
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag("Rock"))
        {
            Destroy(gameObject);
            GameManager.Instancia.ActualizarPuntos();
            GameManager.Instancia.ActualizarPeces(-1);
        }
        
    }

    private IEnumerator Chase()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        else
        {
            movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
            yield return new WaitForSeconds(aiUpdateDelay);

        }

    }
}
