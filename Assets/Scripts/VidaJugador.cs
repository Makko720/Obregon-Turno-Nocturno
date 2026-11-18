
using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Vida del jugador")]
    [SerializeField] private int vidaMaxima = 100;

    public int VidaActual { get; private set; }
    public int VidaMaxima => vidaMaxima;

    public bool EstaDerrotado { get; private set; }

    private void Awake()
    {
        VidaActual = vidaMaxima;
        EstaDerrotado = false;
    }

    public void RecibirDanio(int cantidad)
    {
        if (EstaDerrotado || cantidad <= 0)
            return;

        VidaActual = Mathf.Max(
            VidaActual - cantidad,
            0
        );

        Debug.Log(
            "El jugador recibió " + cantidad +
            " de daño. Vida restante: " + VidaActual
        );

        if (VidaActual <= 0)
        {
            EstaDerrotado = true;

            Debug.Log(
                "¡El jugador ha sido derrotado!"
            );
        }
    }

    public void Curar(int cantidad)
    {
        if (EstaDerrotado || cantidad <= 0)
            return;

        VidaActual = Mathf.Min(
            VidaActual + cantidad,
            vidaMaxima
        );
    }

    public void EstablecerVidaMaxima(int nuevaVidaMaxima, bool aumentarVidaActual = true)
    {
        int incremento = nuevaVidaMaxima - vidaMaxima;
        vidaMaxima = Mathf.Max(1, nuevaVidaMaxima);
        if (aumentarVidaActual && incremento > 0 && !EstaDerrotado)
        {
            VidaActual = Mathf.Min(VidaActual + incremento, vidaMaxima);
        }
    }

    [ContextMenu("Prueba - Recibir 20 de daño")]
    private void ProbarDanio()
    {
        RecibirDanio(20);
    }
}
