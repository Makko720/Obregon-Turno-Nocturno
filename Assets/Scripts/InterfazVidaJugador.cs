
using UnityEngine;
using UnityEngine.UI;

public class InterfazVidaJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VidaJugador vidaJugador;
    [SerializeField] private Text textoVida;
    [SerializeField] private RectTransform rellenoBarraVida;

    private int vidaAnterior = -1;
    private int vidaMaximaAnterior = -1;

    private void Start()
    {
        ActualizarInterfaz();
    }

    private void Update()
    {
        if (vidaJugador == null)
            return;

        // Solo actualizar cuando cambie la vida.
        if (vidaJugador.VidaActual != vidaAnterior ||
            vidaJugador.VidaMaxima != vidaMaximaAnterior)
        {
            ActualizarInterfaz();
        }
    }

    private void ActualizarInterfaz()
    {
        if (vidaJugador == null ||
            textoVida == null ||
            rellenoBarraVida == null)
        {
            Debug.LogWarning(
                "Faltan referencias en InterfazVidaJugador."
            );
            return;
        }

        int vidaActual = vidaJugador.VidaActual;
        int vidaMaxima = vidaJugador.VidaMaxima;

        // Actualizar el texto.
        textoVida.text =
            "VIDA: " + vidaActual + " / " + vidaMaxima;

        // Calcular el porcentaje de vida.
        float porcentaje = vidaMaxima > 0
            ? Mathf.Clamp01(
                (float)vidaActual / vidaMaxima
            )
            : 0f;

        // Reducir el ancho del relleno desde la derecha.
        Vector2 anclaMaxima =
            rellenoBarraVida.anchorMax;

        anclaMaxima.x = porcentaje;

        rellenoBarraVida.anchorMax = anclaMaxima;

        vidaAnterior = vidaActual;
        vidaMaximaAnterior = vidaMaxima;
    }
}
