using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIReverse : MonoBehaviour
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
    Vector2 limitesPantalla;

    private void Start()
    {
        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }
    }

    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.position);
            if (following == false)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }
        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);

        limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x <= limitesPantalla.x / 2)
        {
            direcion = 1;
            pezSprite.rotation = pezSprite.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            direcion = -1;
            pezSprite.rotation = pezSprite.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, limitesPantalla.x * -1 + 0.5f, limitesPantalla.x - 0.5f),
            Mathf.Clamp(transform.position.y, limitesPantalla.y * -1 + 0.5f, limitesPantalla.y - 0.5f),
            0
            );
    }

    private IEnumerator ChaseAndAttack()
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
            StartCoroutine(ChaseAndAttack());

        }

    }
}
