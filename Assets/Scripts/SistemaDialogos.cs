
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SistemaDialogos : MonoBehaviour
{
    [Header("Interfaz")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private Text textoNombre;
    [SerializeField] private Text textoDialogo;
    [SerializeField] private GameObject panelTienda;

    [Header("Referencias")]
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private SistemaMisiones sistemaMisiones;
    [SerializeField] private ComercianteReciclaje comerciante;

    public static bool DialogoActivo { get; private set; }

    private int indiceActual;
    private bool dialogoRegresoFinalizado = false;
    private bool tiendaAbierta = false;
    private bool dialogoDespuesTiendaPendiente = true;

    private struct LineaDialogo
    {
        public string personaje;
        public string mensaje;

        public LineaDialogo(string personaje, string mensaje)
        {
            this.personaje = personaje;
            this.mensaje = mensaje;
        }
    }

    private readonly LineaDialogo[] dialogoInicial =
    {
        new LineaDialogo("COMERCIANTE",
            "¡Eh, muchacho! ¿Acabas de llegar?"),

        new LineaDialogo("JUGADOR",
            "Sí, justo llegué."),

        new LineaDialogo("COMERCIANTE",
            "Debes tener cuidado. Esta ciudad antes estaba llena de gente, pero últimamente un misterioso monstruo nos arrebató la ciudad. Por eso no encontrarás a mucha gente por las calles."),

        new LineaDialogo("JUGADOR",
            "¿Y tú qué haces aquí?"),

        new LineaDialogo("COMERCIANTE",
            "Muchacho, a mí no hay nada que me asuste. Después de 50 años trabajando en estas calles, aprendes a plantarle cara a la suciedad."),

        new LineaDialogo("COMERCIANTE",
            "Pero ya en serio, estoy viejo para seguir limpiando. Hace muchos años podía controlar la suciedad, pero desde que mis rodillas no funcionan igual, esto se ha descontrolado."),

        new LineaDialogo("COMERCIANTE",
            "Como puedes ver, hay basura por todas partes. Esto hace que ellos se vuelvan más fuertes."),

        new LineaDialogo("JUGADOR",
            "¿Ellos? ¿Quiénes?"),

        new LineaDialogo("COMERCIANTE",
            "Los monstruos de la basura, muchacho."),

        new LineaDialogo("JUGADOR",
            "Ja, ja, sí, claro."),

        new LineaDialogo("COMERCIANTE",
            "Bueno, si no quieres creerme, pruébalo por ti mismo. Pero, para que no sea tan peligroso para ti, quiero que me hagas un favor."),

        new LineaDialogo("COMERCIANTE",
            "Recoge la basura por aquí. Hay muchas botellas y latas que puedes juntar con las manos. Tráelas y yo te las compraré por un buen precio."),

        new LineaDialogo("JUGADOR",
            "¿Me vas a pagar por recoger la basura?"),

        new LineaDialogo("COMERCIANTE",
            "¡Claro! Prefiero pagarte por hacer algo sencillo que seguir dejando que ellos se hagan más fuertes."),

        new LineaDialogo("JUGADOR",
            "Bueno, pero estás un poco chiflado.")
    };

    private readonly LineaDialogo[] dialogoRegreso =
    {
        new LineaDialogo("COMERCIANTE",
            "¿Cómo te fue, muchacho?"),

        new LineaDialogo("JUGADOR",
            "Es sencillo, no requiere mucha ciencia."),

        new LineaDialogo("COMERCIANTE",
            "Bueno, veo que solo traes cinco objetos."),

        new LineaDialogo("JUGADOR",
            "Sí, es todo lo que me cabe en la mochila."),

        new LineaDialogo("COMERCIANTE",
            "Está bien, te compraré todo esto por $ {TOTAL}."),

        new LineaDialogo("JUGADOR",
            "¡Ufff, sí que pagas bien!"),

        new LineaDialogo("COMERCIANTE",
            "Te dije, es mejor pagarte para que limpies y evitar que ellos se hagan más fuertes."),

        new LineaDialogo("COMERCIANTE",
            "Pero si te interesa, también tengo algunas cosas para vender.")
    };

    private readonly LineaDialogo[] dialogoDespuesTienda =
    {
        new LineaDialogo("COMERCIANTE",
            "Ya sabes, muchacho, el trabajo es sencillo. Tú recoges basura, me la traes a vender y puedes seguir equipándote para sobrevivir la noche."),

        new LineaDialogo("JUGADOR",
            "¿Sobrevivir? ¿Quieres decir que sí hay peligros?"),

        new LineaDialogo("COMERCIANTE",
            "Ya te dije, muchacho. Tú equípate lo más que puedas. Puedes comprar una escoba para limpiar las manchas de basura y una pala para reparar estas calles."),

        new LineaDialogo("COMERCIANTE",
            "Por la noche estas herramientas también te servirán como armas. Todo ayuda para acabar con esos malditos.")
    };

    private void Awake()
    {
        DialogoActivo = false;
    }

    private void Start()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (panelTienda != null)
            panelTienda.SetActive(false);
    }

    private void Update()
    {
        if (!DialogoActivo || MenuPausa.EstaPausado)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            AvanzarDialogo();
        }
    }

    
    private enum TipoConversacion
    {
        Inicial,
        Regreso,
        DespuesTienda
    }

    private TipoConversacion conversacionActual;

    private LineaDialogo[] ObtenerLineasActuales()
    {
        switch (conversacionActual)
        {
            case TipoConversacion.Regreso:
                return dialogoRegreso;

            case TipoConversacion.DespuesTienda:
                return dialogoDespuesTienda;

            default:
                return dialogoInicial;
        }
    }

    [ContextMenu("Prueba - Iniciar dialogo inicial")]
    public void IniciarDialogoInicial()
    {
        if (sistemaMisiones != null &&
            sistemaMisiones.EtapaActual !=
            EtapaMision.HablarComerciante)
            return;

        IniciarConversacion(TipoConversacion.Inicial);
    }

    [ContextMenu("Prueba - Iniciar dialogo de regreso")]
    
    [ContextMenu("Prueba - Iniciar dialogo de regreso")]
    public void IniciarDialogoRegreso()
    {
        // No repetir una conversacion ya terminada.
        if (dialogoRegresoFinalizado)
            return;

        if (sistemaMisiones == null ||
            sistemaMisiones.EtapaActual !=
            EtapaMision.VolverComerciante)
        {
            Debug.Log(
                "Todavia no corresponde hablar de los residuos."
            );
            return;
        }

        IniciarConversacion(TipoConversacion.Regreso);
    }


    private void IniciarConversacion(TipoConversacion tipo)
    {
        if (DialogoActivo || MenuPausa.EstaPausado)
            return;

        if (panelDialogo == null ||
            textoNombre == null ||
            textoDialogo == null)
        {
            Debug.LogWarning(
                "Faltan referencias del sistema de dialogos."
            );
            return;
        }

        conversacionActual = tipo;
        indiceActual = 0;
        DialogoActivo = true;

        panelDialogo.SetActive(true);

        if (jugadorFPS != null)
            jugadorFPS.enabled = false;

        MostrarLinea();
    }

    private void MostrarLinea()
    {
        LineaDialogo[] lineas = ObtenerLineasActuales();
        LineaDialogo linea = lineas[indiceActual];

        textoNombre.text = linea.personaje;

        // Sustituir el marcador por el precio real.
        string mensaje = linea.mensaje;

        if (mensaje.Contains("{TOTAL}"))
        {
            int total = comerciante != null
                ? comerciante.CalcularTotalVenta()
                : 0;

            mensaje = mensaje.Replace(
                "$ {TOTAL}",
                "$" + total
            );
        }

        textoDialogo.text = mensaje;

        if (linea.personaje == "COMERCIANTE")
        {
            textoNombre.color =
                new Color32(197, 231, 182, 255);
        }
        else
        {
            textoNombre.color =
                new Color32(212, 187, 138, 255);
        }
    }

    
    private void AvanzarDialogo()
    {
        // Cuando avanzamos despues de la oferta,
        // el comerciante compra los residuos.
        if (conversacionActual == TipoConversacion.Regreso &&
            indiceActual == 4)
        {
            if (comerciante == null)
            {
                Debug.LogWarning(
                    "No hay comerciante asignado para realizar la venta."
                );
                return;
            }

            comerciante.VenderResiduos();
        }

        indiceActual++;

        if (indiceActual >= ObtenerLineasActuales().Length)
        {
            FinalizarDialogo();
            return;
        }

        MostrarLinea();
    }


    private void FinalizarDialogo()
    {
        TipoConversacion conversacionTerminada =
            conversacionActual;

        DialogoActivo = false;

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        // Al terminar la conversación de regreso,
        // abrimos la tienda sin devolver todavía
        // el control al jugador.
        if (conversacionTerminada == TipoConversacion.Regreso)
        {
            dialogoRegresoFinalizado = true;
            AbrirTienda();
            return;
        }

        // Para los demás diálogos, devolvemos
        // el control al jugador.
        if (jugadorFPS != null)
            jugadorFPS.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (conversacionTerminada == TipoConversacion.Inicial)
        {
            if (sistemaMisiones != null)
                sistemaMisiones.FinalizarPrimerDialogo();
        }
        
        // Actualizar la misión al terminar la conversación
        // que ocurre después de cerrar la tienda.
        if (conversacionTerminada == TipoConversacion.DespuesTienda)
        {
            if (sistemaMisiones != null)
                sistemaMisiones.FinalizarTutorialComerciante();
        }
    }

    private void AbrirTienda()
    {
        if (panelTienda == null)
        {
            Debug.LogError(
                "Falta asignar PanelTienda en SistemaDialogos."
            );

            if (jugadorFPS != null)
                jugadorFPS.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        tiendaAbierta = true;

        if (jugadorFPS != null)
            jugadorFPS.enabled = false;

        panelTienda.SetActive(true);

        // Liberamos el cursor para usar los botones.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Tienda del comerciante abierta.");
    }

    public void AbrirTiendaHabitual()
    {
        // No abrir mientras haya un diálogo,
        // otra tienda abierta o el juego pausado.
        if (!dialogoRegresoFinalizado ||
            tiendaAbierta ||
            DialogoActivo ||
            MenuPausa.EstaPausado)
        {
            return;
        }

        AbrirTienda();
    }

    public void CerrarTienda()
    {
        if (!tiendaAbierta)
            return;

        tiendaAbierta = false;

        if (panelTienda != null)
            panelTienda.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Solo la primera vez continuamos
        // la conversación del tutorial.
        if (dialogoDespuesTiendaPendiente)
        {
            dialogoDespuesTiendaPendiente = false;

            IniciarConversacion(
                TipoConversacion.DespuesTienda
            );

            return;
        }

        // En visitas posteriores simplemente
        // cerramos la tienda y volvemos al juego.
        if (jugadorFPS != null)
            jugadorFPS.enabled = true;
    }
}
