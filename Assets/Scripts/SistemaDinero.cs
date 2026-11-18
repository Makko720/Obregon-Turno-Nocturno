
using UnityEngine;
using UnityEngine.UI;

public class SistemaDinero : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private Text textoDinero;
    [SerializeField] private Text textoDineroTienda;
    [SerializeField] private Text textoPuntuacion;

    [Header("Economia del jugador")]
    public int DineroActual { get; private set; }
    public int PuntuacionTotal { get; private set; }

    private void Start()
    {
        DineroActual = 0;
        PuntuacionTotal = 0;

        ActualizarInterfaz();
    }

    public void GanarDinero(int cantidad)
    {
        if (cantidad <= 0) return;

        DineroActual += cantidad;
        PuntuacionTotal += cantidad;

        ActualizarInterfaz();
    }

    public bool GastarDinero(int cantidad)
    {
        if (cantidad <= 0 || DineroActual < cantidad)
            return false;

        DineroActual -= cantidad;

        ActualizarInterfaz();
        return true;
    }

    private void ActualizarInterfaz()
    {
        // Dinero disponible en el HUD
        if (textoDinero != null)
        {
            textoDinero.text = "DINERO: $" + DineroActual;
        }

        // Dinero disponible en la tienda
        if (textoDineroTienda != null)
        {
            textoDineroTienda.text = "DINERO: $" + DineroActual;
        }

        // Puntuación acumulada en Estadísticas
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text =
                "PUNTUACIÓN: $" + PuntuacionTotal;
        }
    }

    // Funcion temporal para comprobar el sistema.
    [ContextMenu("Prueba - Ganar 1000 pesos")]
    private void ProbarDinero()
    {
        GanarDinero(1000);
    }
}
