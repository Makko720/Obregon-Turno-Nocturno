
using UnityEngine;

public class BacheReparable : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private TiendaHerramientas tiendaHerramientas;
    [SerializeField] private SistemaMisiones sistemaMisiones;

    [Header("Apariencia")]
    [SerializeField] private GameObject bacheDaniado;
    [SerializeField] private GameObject bacheReparado;

    private bool reparado = false;
    private Collider colisionPrincipal;

    private void Awake()
    {
        colisionPrincipal = GetComponent<Collider>();

        if (sistemaMisiones == null)
        {
            sistemaMisiones =
                FindAnyObjectByType<SistemaMisiones>();
        }

        // Buscar los objetos visuales de este bache.
        Transform objetoDaniado =
            transform.Find("BacheDaniado");

        Transform objetoReparado =
            transform.Find("BacheReparado");

        if (objetoDaniado != null)
            bacheDaniado = objetoDaniado.gameObject;

        if (objetoReparado != null)
            bacheReparado = objetoReparado.gameObject;
    }

    private void Start()
    {
        ActualizarApariencia();
    }

    public bool PuedeReparar()
    {
        return !reparado &&
               inventario != null &&
               tiendaHerramientas != null &&
               tiendaHerramientas.PalaComprada &&
               inventario.ObtenerCantidad(
                   TipoObjetoInventario.Asfalto
               ) >= 1;
    }

    public string ObtenerTextoInteraccion()
    {
        if (reparado)
            return "";

        if (tiendaHerramientas == null ||
            !tiendaHerramientas.PalaComprada)
            return "Necesitas una pala";

        if (inventario == null ||
            inventario.ObtenerCantidad(
                TipoObjetoInventario.Asfalto
            ) < 1)
            return "Necesitas asfalto";

        return "[E] Reparar bache";
    }

    public void IntentarReparar()
    {
        if (!PuedeReparar())
            return;

        if (bacheDaniado == null || bacheReparado == null)
        {
            Debug.LogError(
                gameObject.name +
                ": Faltan objetos visuales."
            );
            return;
        }

        // Consumir un saco de asfalto.
        if (!inventario.QuitarObjeto(
            TipoObjetoInventario.Asfalto, 1))
            return;

        // Entregar los escombros en el espacio liberado.
        bool escombroGuardado = inventario.AgregarObjeto(
            TipoObjetoInventario.Escombros, 1
        );

        if (!escombroGuardado)
        {
            // Si falla la entrega, devolver el asfalto.
            inventario.AgregarObjeto(
                TipoObjetoInventario.Asfalto, 1
            );

            Debug.LogWarning(
                "No fue posible guardar los escombros."
            );
            return;
        }

        // Marcar la reparacion como completada.
        reparado = true;

        ActualizarApariencia();

        // Avanzar la mision del Dia 2.
        if (sistemaMisiones != null)
        {
            sistemaMisiones.RegistrarBacheReparado();
        }

        Debug.Log(
            gameObject.name +
            " reparado. Obtuviste 1 Escombros."
        );
    }

    private void ActualizarApariencia()
    {
        if (bacheDaniado != null)
            bacheDaniado.SetActive(!reparado);

        if (bacheReparado != null)
            bacheReparado.SetActive(reparado);

        if (colisionPrincipal != null)
            colisionPrincipal.enabled = !reparado;
    }
}
