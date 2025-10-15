using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PezAI : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    Vector2 limitesPantalla;
    private int direcion = 1;
    private float tamano;
    [SerializeField] private Transform pezSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        limitesPantalla = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x <= limitesPantalla.x/2)
        {
            direcion = 1;
            pezSprite.rotation = pezSprite.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            direcion = -1;
            pezSprite.rotation = pezSprite.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        //Tamano
        float tamanoAleatorio = Random.Range(0.5f, 2.5f);
        tamano = tamanoAleatorio;
        transform.localScale = new Vector3(tamanoAleatorio, tamanoAleatorio, tamanoAleatorio);
    }

    // Update is called once per frame
    private void Update()
    {
        print(direcion);
        transform.position = transform.position + Vector3.right * direcion * Time.deltaTime * speed;
        if (transform.position.x <= -limitesPantalla.x - 2 || transform.position.x > limitesPantalla.x + 2) { Destroy(gameObject); }
    }

    public float GetTamano()
    {
        return tamano;
    }
}
