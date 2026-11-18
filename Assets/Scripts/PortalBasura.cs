using UnityEngine;

public class PortalBasura : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private SistemaMisiones sistemaMisiones;

    [Header("Visuales del Portal")]
    [SerializeField] private GameObject visualPortalApagado;
    [SerializeField] private GameObject visualPortalActivo;

    [Header("Requisitos de activacion")]
    [SerializeField, Min(1)] private int residuosRequeridos = 6;

    public bool PortalActivado { get; private set; }

    private void Start()
    {
        ActualizarVisuales();
    }

    public int ContarResiduosDisponibles()
    {
        if (inventario == null)
            return 0;

        return inventario.ObtenerCantidad(TipoObjetoInventario.Botella) +
               inventario.ObtenerCantidad(TipoObjetoInventario.Lata) +
               inventario.ObtenerCantidad(TipoObjetoInventario.ResiduosBarrido) +
               inventario.ObtenerCantidad(TipoObjetoInventario.Escombros);
    }

    public bool PuedeActivarse()
    {
        if (PortalActivado || sistemaMisiones == null || inventario == null)
            return false;

        if (sistemaMisiones.EtapaActual != EtapaMision.RecolectarRecursosDia3 &&
            sistemaMisiones.EtapaActual != EtapaMision.ActivarPortalDia3)
            return false;

        return ContarResiduosDisponibles() >= residuosRequeridos;
    }

    public string ObtenerTextoInteraccion()
    {
        if (sistemaMisiones == null)
            return "";

        if (PortalActivado)
            return "Portal de basura activado";

        if (sistemaMisiones.EtapaActual != EtapaMision.RecolectarRecursosDia3 &&
            sistemaMisiones.EtapaActual != EtapaMision.ActivarPortalDia3)
            return "Portal inactivo (Disponible en el Día 3)";

        int actuales = ContarResiduosDisponibles();
        if (actuales < residuosRequeridos)
            return "Necesitas " + residuosRequeridos + " residuos (" + actuales + "/" + residuosRequeridos + ")";

        return "[E] Activar Portal de Basura";
    }

    public void IntentarActivar()
    {
        if (!PuedeActivarse())
            return;

        int porConsumir = residuosRequeridos;
        porConsumir -= ConsumirTipo(TipoObjetoInventario.Botella, porConsumir);
        porConsumir -= ConsumirTipo(TipoObjetoInventario.Lata, porConsumir);
        porConsumir -= ConsumirTipo(TipoObjetoInventario.ResiduosBarrido, porConsumir);
        porConsumir -= ConsumirTipo(TipoObjetoInventario.Escombros, porConsumir);

        PortalActivado = true;
        ActualizarVisuales();

        if (sistemaMisiones != null)
        {
            sistemaMisiones.RegistrarPortalActivado();
        }

        Debug.Log("¡Portal de Basura activado! Ve a dormir para la última noche.");
    }

    private int ConsumirTipo(TipoObjetoInventario tipo, int maximoAConsumir)
    {
        if (maximoAConsumir <= 0 || inventario == null)
            return 0;

        int disponibles = inventario.ObtenerCantidad(tipo);
        int cantidad = Mathf.Min(disponibles, maximoAConsumir);

        if (cantidad > 0 && inventario.QuitarObjeto(tipo, cantidad))
            return cantidad;

        return 0;
    }

    private void ActualizarVisuales()
    {
        if (visualPortalApagado != null)
            visualPortalApagado.SetActive(!PortalActivado);

        if (visualPortalActivo != null)
            visualPortalActivo.SetActive(PortalActivado);
    }
}