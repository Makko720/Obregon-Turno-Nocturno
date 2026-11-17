
using UnityEngine;
using UnityEngine.UI;

public class TiendaHerramientas : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SistemaDinero sistemaDinero;
    [SerializeField] private SistemaMisiones sistemaMisiones;
    [SerializeField] private SistemaDiaNoche sistemaDiaNoche;
    [SerializeField] private InventarioJugador inventarioJugador;

    [Header("Escoba")]
    [SerializeField] private Button buttonComprarEscoba;
    [SerializeField] private Text textoPrecioEscoba;
    [SerializeField] private Text textoBotonEscoba;
    [SerializeField] private int precioEscoba = 100;

    [Header("Pala")]
    [SerializeField] private Button buttonComprarPala;
    [SerializeField] private Text textoPrecioPala;
    [SerializeField] private Text textoBotonPala;
    [SerializeField] private int precioPala = 200;

    [Header("Asfalto")]
    [SerializeField] private Button buttonComprarAsfalto;
    [SerializeField] private Text textoPrecioAsfalto;
    [SerializeField] private Text textoBotonAsfalto;
    [SerializeField] private int precioAsfalto = 50;

    public bool EscobaComprada { get; private set; }
    public bool PalaComprada { get; private set; }

    private int ultimoDiaRevisado = -1;

    private void Awake()
    {
        if (buttonComprarEscoba != null)
            buttonComprarEscoba.onClick.AddListener(ComprarEscoba);

        if (buttonComprarPala != null)
            buttonComprarPala.onClick.AddListener(ComprarPala);

        if (buttonComprarAsfalto != null)
            buttonComprarAsfalto.onClick.AddListener(ComprarAsfalto);
    }

    private void OnEnable()
    {
        if (inventarioJugador != null)
            inventarioJugador.InventarioActualizado +=
                ActualizarInterfaz;

        ActualizarInterfaz();
    }

    private void OnDisable()
    {
        if (inventarioJugador != null)
            inventarioJugador.InventarioActualizado -=
                ActualizarInterfaz;
    }

    private void Update()
    {
        if (sistemaDiaNoche == null)
            return;

        if (ultimoDiaRevisado != sistemaDiaNoche.DiaActual)
        {
            ultimoDiaRevisado = sistemaDiaNoche.DiaActual;
            ActualizarInterfaz();
        }
    }

    public bool HerramientasAvanzadasDesbloqueadas()
    {
        return sistemaDiaNoche != null &&
               sistemaDiaNoche.DiaActual >= 2;
    }

    public void ComprarEscoba()
    {
        if (EscobaComprada)
            return;

        if (sistemaDinero == null || sistemaMisiones == null)
        {
            Debug.LogWarning(
                "Faltan referencias en TiendaHerramientas."
            );
            return;
        }

        if (sistemaMisiones.EtapaActual !=
            EtapaMision.ComprarEscoba)
        {
            Debug.Log("Todavia no puedes comprar la escoba.");
            return;
        }

        if (!sistemaDinero.GastarDinero(precioEscoba))
        {
            Debug.Log("Dinero insuficiente para la escoba.");
            return;
        }

        EscobaComprada = true;
        sistemaMisiones.RegistrarCompraEscoba();

        ActualizarInterfaz();

        Debug.Log("¡Escoba comprada correctamente!");
    }

    public void ComprarPala()
    {
        if (PalaComprada)
            return;

        if (sistemaDinero == null ||
            sistemaMisiones == null ||
            sistemaDiaNoche == null)
        {
            Debug.LogWarning(
                "Faltan referencias en TiendaHerramientas."
            );
            return;
        }

        if (!HerramientasAvanzadasDesbloqueadas())
        {
            Debug.Log("La pala se desbloquea en el Dia 2.");
            return;
        }

        if (sistemaMisiones.EtapaActual !=
            EtapaMision.ComprarPalaDia2)
        {
            Debug.Log("Todavia no puedes comprar la pala.");
            return;
        }

        if (!sistemaDinero.GastarDinero(precioPala))
        {
            Debug.Log(
                "Dinero insuficiente. Necesitas $" + precioPala
            );
            return;
        }

        PalaComprada = true;
        sistemaMisiones.RegistrarCompraPala();

        ActualizarInterfaz();

        Debug.Log("¡Pala comprada correctamente!");
    }

    public void ComprarAsfalto()
    {
        if (sistemaDinero == null ||
            sistemaDiaNoche == null ||
            inventarioJugador == null)
        {
            Debug.LogWarning(
                "Faltan referencias para comprar asfalto."
            );
            return;
        }

        // Solo permitir compras desde el Dia 2.
        if (!HerramientasAvanzadasDesbloqueadas())
        {
            Debug.Log(
                "El asfalto se desbloquea en el Dia 2."
            );
            return;
        }

        // Comprobar primero el espacio disponible.
        if (inventarioJugador.EspaciosDisponibles < 1)
        {
            Debug.Log(
                "No tienes espacio en la mochila para el asfalto."
            );
            ActualizarInterfaz();
            return;
        }

        // Verificar el dinero antes de agregar el saco.
        if (sistemaDinero.DineroActual < precioAsfalto)
        {
            Debug.Log(
                "Dinero insuficiente. Necesitas $" + precioAsfalto
            );
            return;
        }

        // Agregar el saco al inventario.
        if (!inventarioJugador.AgregarObjeto(
            TipoObjetoInventario.Asfalto, 1))
        {
            Debug.Log(
                "No fue posible agregar el asfalto."
            );
            return;
        }

        // Cobrar solamente si se agrego el objeto.
        if (!sistemaDinero.GastarDinero(precioAsfalto))
        {
            // Deshacer la entrega si falla el cobro.
            inventarioJugador.QuitarObjeto(
                TipoObjetoInventario.Asfalto, 1
            );

            Debug.LogWarning(
                "Compra cancelada: no se pudo cobrar."
            );
            return;
        }

        ActualizarInterfaz();

        Debug.Log(
            "¡Compraste un saco de asfalto! Total: " +
            inventarioJugador.ObtenerCantidad(
                TipoObjetoInventario.Asfalto
            )
        );
    }

    private void ActualizarInterfaz()
    {
        // ESCOBA
        if (textoPrecioEscoba != null)
            textoPrecioEscoba.text =
                "PRECIO: $" + precioEscoba;

        if (textoBotonEscoba != null)
            textoBotonEscoba.text =
                EscobaComprada ? "COMPRADO" : "COMPRAR";

        if (buttonComprarEscoba != null)
            buttonComprarEscoba.interactable = !EscobaComprada;

        bool desbloqueadas =
            HerramientasAvanzadasDesbloqueadas();

        // PALA
        if (textoPrecioPala != null)
            textoPrecioPala.text =
                "PRECIO: $" + precioPala;

        if (textoBotonPala != null)
        {
            if (PalaComprada)
                textoBotonPala.text = "COMPRADO";
            else if (!desbloqueadas)
                textoBotonPala.text = "DIA 2";
            else
                textoBotonPala.text = "COMPRAR";
        }

        if (buttonComprarPala != null)
        {
            buttonComprarPala.interactable =
                !PalaComprada &&
                desbloqueadas &&
                sistemaMisiones != null &&
                sistemaMisiones.EtapaActual ==
                    EtapaMision.ComprarPalaDia2;
        }

        // ASFALTO
        if (textoPrecioAsfalto != null)
            textoPrecioAsfalto.text =
                "PRECIO: $" + precioAsfalto;

        bool mochilaLlena =
            inventarioJugador != null &&
            inventarioJugador.EspaciosDisponibles == 0;

        if (textoBotonAsfalto != null)
        {
            if (!desbloqueadas)
                textoBotonAsfalto.text = "DIA 2";
            else if (mochilaLlena)
                textoBotonAsfalto.text = "MOCHILA LLENA";
            else
                textoBotonAsfalto.text = "COMPRAR";
        }

        if (buttonComprarAsfalto != null)
        {
            buttonComprarAsfalto.interactable =
                desbloqueadas &&
                inventarioJugador != null &&
                !mochilaLlena;
        }
    }
}
