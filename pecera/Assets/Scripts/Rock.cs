using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Rock : MonoBehaviour
{
    Vector2 limitesPantalla;
    private float tamano;
    [SerializeField] private Transform rockSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Tamano
        float tamanoAleatorio = Random.Range(1f, 2.5f);
        tamano = tamanoAleatorio;
        transform.localScale = new Vector3(tamanoAleatorio, tamanoAleatorio, tamanoAleatorio);
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.position.y <= -limitesPantalla.y + 1.5f || transform.position.y > limitesPantalla.y + 2) { Destroy(gameObject); }
    }
}
