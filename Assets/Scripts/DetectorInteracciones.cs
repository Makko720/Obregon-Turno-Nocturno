
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DetectorInteracciones : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private Text textoPuntero;
    [SerializeField] private Text textoInteraccion;

    [Header("Sistemas")]
    [SerializeField] private SistemaDialogos sistemaDialogos;
    [SerializeField] private SistemaMisiones sistemaMisiones;

    [Header("Deteccion")]
    [SerializeField] private float distanciaInteraccion = 3f;
    private bool esperarLiberacionE = false;

    private readonly Color colorNormal =
        new Color32(240, 242, 239, 255);

    private readonly Color colorInteractuable =
        new Color32(197, 231, 182, 255);

    private void Update()
    {
        if (camaraJugador == null ||
            jugadorFPS == null ||
            textoPuntero == null ||
            textoInteraccion == null)
            return;

        bool interfazAbierta =
            MenuPausa.EstaPausado ||
            SistemaDialogos.DialogoActivo ||
            !jugadorFPS.enabled;

        
            if (interfazAbierta)
            {
                // Evitar que la misma pulsación de E
                // cierre un diálogo y vuelva a interactuar.
                esperarLiberacionE = true;

                textoPuntero.enabled = false;
                textoInteraccion.enabled = false;
                return;
            }

            // Si acabamos de cerrar una interfaz,
            // esperamos a que el jugador suelte E.
            if (esperarLiberacionE)
            {
                if (Keyboard.current != null &&
                    Keyboard.current.eKey.isPressed)
                {
                    return;
                }

                esperarLiberacionE = false;

                // Consumimos la liberación de la tecla
                // sin ejecutar otra interacción.
                if (Keyboard.current != null &&
                    Keyboard.current.eKey.wasReleasedThisFrame)
                {
                    return;
                }
            }


        textoPuntero.enabled = true;
        textoPuntero.color = colorNormal;
        textoInteraccion.enabled = false;

        Ray rayo = new Ray(
            camaraJugador.transform.position,
            camaraJugador.transform.forward
        );

        if (!Physics.Raycast(
            rayo,
            out RaycastHit impacto,
            distanciaInteraccion))
            return;

        
            // Detectar botellas y latas recolectables.
            BasuraRecolectable basura =
                impacto.collider.GetComponentInParent<BasuraRecolectable>();

            if (basura != null)
            {
                MostrarInteraccion("[E] Recoger");

                if (Keyboard.current != null &&
                    Keyboard.current.eKey.wasReleasedThisFrame)
                {
                    InventarioJugador inventario =
                        jugadorFPS.GetComponent<InventarioJugador>();

                    if (inventario != null)
                    {
                        basura.Recolectar(inventario);
                    }
                    else
                    {
                        Debug.LogWarning(
                            "No se encontro InventarioJugador."
                        );
                    }
                }

                return;
            }



        // Detectar baches reparables.
        BacheReparable bache =
            impacto.collider.GetComponentInParent<BacheReparable>();

        if (bache != null)
        {
            MostrarInteraccion(
                bache.ObtenerTextoInteraccion()
            );

            if (Keyboard.current != null &&
                Keyboard.current.eKey.wasReleasedThisFrame)
            {
                bache.IntentarReparar();
            }

            return;
        }


        // Detectar el punto de descanso.
        PuntoDescansoInteractivo descanso =
            impacto.collider.GetComponentInParent<PuntoDescansoInteractivo>();

        if (descanso != null)
        {
            if (descanso.PuedeDormir())
            {
                MostrarInteraccion("[E] Ir a dormir");

                if (Keyboard.current != null &&
                    Keyboard.current.eKey.wasReleasedThisFrame)
                {
                    descanso.IntentarDormir();
                }
            }
            else
            {
                MostrarInteraccion(
                    "Necesitas una escoba"
                );
            }

            return;
        }


        // Detectar al comerciante.
        ComercianteReciclaje comerciante =
            impacto.collider.GetComponentInParent<ComercianteReciclaje>();
        if (comerciante != null)
        {
            MostrarInteraccion("[E] Hablar");

            if (sistemaDialogos != null &&
                sistemaMisiones != null &&
                Keyboard.current != null &&
                Keyboard.current.eKey.wasReleasedThisFrame)
            {
                
                
            
                switch (sistemaMisiones.EtapaActual)
                {
                    // Primera conversacion del tutorial.
                    case EtapaMision.HablarComerciante:
                        sistemaDialogos.IniciarDialogoInicial();
                        break;

                    // Segunda conversacion del tutorial.
                    case EtapaMision.VolverComerciante:
                        sistemaDialogos.IniciarDialogoRegreso();
                        break;

                    // Durante la recoleccion inicial,
                    // todavia no se abre la tienda.
                    case EtapaMision.RecolectarResiduos:
                        break;

                    // Despues del tutorial, la tienda
                    // permanece disponible en cualquier mision.
                    default:
                        sistemaDialogos.AbrirTiendaHabitual();
                        break;
                }



            }
        }

    }

    private void MostrarInteraccion(string mensaje)
    {
        textoPuntero.color = colorInteractuable;
        textoInteraccion.text = mensaje;
        textoInteraccion.enabled = true;
    }
}
