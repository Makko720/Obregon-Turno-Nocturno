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
            esperarLiberacionE = true;

            textoPuntero.enabled = false;
            textoInteraccion.enabled = false;
            return;
        }

        if (esperarLiberacionE)
        {
            if (Keyboard.current != null &&
                Keyboard.current.eKey.isPressed)
            {
                return;
            }

            esperarLiberacionE = false;

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
            string mensajeDescanso = descanso.ObtenerTextoInteraccion();

            if (!string.IsNullOrEmpty(mensajeDescanso))
            {
                MostrarInteraccion(mensajeDescanso);
            }

            if (descanso.PuedeDormir() &&
                Keyboard.current != null &&
                Keyboard.current.eKey.wasReleasedThisFrame)
            {
                descanso.IntentarDormir();
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
                    case EtapaMision.HablarComerciante:
                        sistemaDialogos.IniciarDialogoInicial();
                        break;

                    case EtapaMision.VolverComerciante:
                        sistemaDialogos.IniciarDialogoRegreso();
                        break;

                    case EtapaMision.RecolectarResiduos:
                        break;

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