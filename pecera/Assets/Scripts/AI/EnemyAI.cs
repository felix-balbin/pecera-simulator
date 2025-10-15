using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField] private List<Detector> detectors;

    [SerializeField] private AIData aiData;

    [SerializeField] private float detectionDelay = 0.05f;

    [SerializeField] private Transform pezSprite;

    private int direcion = 1;
    [SerializeField] private Vector2 movementInput;
    private float tamano;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag("Rock"))
        {
            Destroy(gameObject);
            GameManager.Instancia.ActualizarPeces(-1);
        }
        if (collision.gameObject.CompareTag("Pez"))
        {
            EnemyAI pezAi = collision.gameObject.GetComponent<EnemyAI>();
            if (tamano >= pezAi.GetTamano())
            {
                transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                tamano = transform.localScale.x;
                Destroy(collision.gameObject);
                GameManager.Instancia.ActualizarPeces(-1);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
}