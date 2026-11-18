using UnityEngine;
using UnityEngine.UI;

public class InterfazEstaminaJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private Text textoEstamina;
    [SerializeField] private RectTransform rellenoBarraEstamina;

    private int estaminaAnterior = -1;
    private int estaminaMaximaAnterior = -1;

    private void Start()
    {
        ActualizarInterfaz();
    }

    private void Update()
    {
        if (jugadorFPS == null)
            return;

        int actualRedondeada = Mathf.CeilToInt(jugadorFPS.EstaminaActual);
        int maximaRedondeada = Mathf.CeilToInt(jugadorFPS.EstaminaMaxima);

        if (actualRedondeada != estaminaAnterior ||
            maximaRedondeada != estaminaMaximaAnterior)
        {
            ActualizarInterfaz();
        }
    }

    private void ActualizarInterfaz()
    {
        if (jugadorFPS == null || rellenoBarraEstamina == null)
            return;

        int actual = Mathf.CeilToInt(jugadorFPS.EstaminaActual);
        int maxima = Mathf.CeilToInt(jugadorFPS.EstaminaMaxima);

        if (textoEstamina != null)
        {
            textoEstamina.text = "ESTAMINA: " + actual + " / " + maxima;
        }

        float porcentaje = jugadorFPS.EstaminaMaxima > 0f
            ? Mathf.Clamp01(jugadorFPS.EstaminaActual / jugadorFPS.EstaminaMaxima)
            : 0f;

        Vector2 anclaMaxima = rellenoBarraEstamina.anchorMax;
        anclaMaxima.x = porcentaje;
        rellenoBarraEstamina.anchorMax = anclaMaxima;

        estaminaAnterior = actual;
        estaminaMaximaAnterior = maxima;
    }
}