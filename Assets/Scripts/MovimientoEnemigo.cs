
using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    public float velocidad = 2f;
    public float distancia = 5f;

    private Vector3 posicionInicial;
    private Vector3 direccion;

    void Start()
    {
        posicionInicial = transform.position;
        direccion = transform.forward;
    }

    void Update()
    {
        float avance = Mathf.PingPong(
            Time.time * velocidad,
            distancia
        );

        transform.position =
            posicionInicial + direccion * avance;
    }
}
