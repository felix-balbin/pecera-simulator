using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class Player : MonoBehaviour
{
    //SerializeField
    [SerializeField] private Transform pezSprite;
    [SerializeField] private PlayerUI playerUi;
    [SerializeField] InputAction horizontalAction;
    [SerializeField] InputAction verticalAction;
    public float speed = 3.0f;
    private float tamano;

    private void OnEnable()
    {
        horizontalAction.Enable();
        verticalAction.Enable();
    }

    private void OnDisable()
    {
        horizontalAction.Disable();
        verticalAction.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        tamano = transform.localScale.x;
    }

    // Update is called once per frame
    private void Update()
    {
        //Movimiento
        float horizontal = horizontalAction.ReadValue<float>();
        float vertical = verticalAction.ReadValue<float>();

        Vector3 movement = new Vector3(horizontal, vertical, 0);
        transform.Translate(movement * Time.deltaTime * speed);

        //Evitar salir pantalla
        Vector2 limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, limitesPantalla.x * -1 + 0.5f, limitesPantalla.x - 0.5f),
            Mathf.Clamp(transform.position.y, limitesPantalla.y * -1 + 0.5f, limitesPantalla.y - 0.5f),
            0
            );
        if (playerUi.GetPuntos() >= 10)
        {
            GameManager.Instancia.ActualizarMaquinaDeEstados(MaquinaDeEstados.JuegoGanado);
            speed = 0;
        }
        //Rotacion
        if (horizontal == 0) return;
        
        if (horizontal < 0)
        {
            pezSprite.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else {
            pezSprite.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag("Pez"))
        {
            EnemyAI pezAi = collision.gameObject.GetComponent<EnemyAI>();
            if (tamano >= pezAi.GetTamano())
            {
                playerUi.ActualizarPuntos();
                transform.localScale = transform.localScale + new Vector3(0.1f, 0.1f, 0.1f);
                tamano = transform.localScale.x;
                Destroy(collision.gameObject);
                GameManager.Instancia.ActualizarPeces(-1);
            }
            else
            {
                GameManager.Instancia.ActualizarMaquinaDeEstados(MaquinaDeEstados.JuegoTerminado);
                Destroy(gameObject);
            }

        }
        if (collision.gameObject.CompareTag("Rock"))
        {
            GameManager.Instancia.ActualizarMaquinaDeEstados(MaquinaDeEstados.JuegoTerminado);
            Destroy(gameObject);
        }
    }
}
