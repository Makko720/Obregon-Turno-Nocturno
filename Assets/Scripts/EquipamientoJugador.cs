
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipamientoJugador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TiendaHerramientas tiendaHerramientas;
    [SerializeField] private JugadorFPS jugadorFPS;

    [Header("Herramientas equipadas")]
    [SerializeField] private GameObject escobaEquipada;
    [SerializeField] private GameObject palaEquipada;

    private void Start()
    {
        // Comenzar sin ninguna herramienta visible.
        GuardarHerramientas();
    }

    private void Update()
    {
        if (MenuPausa.EstaPausado ||
            SistemaDialogos.DialogoActivo ||
            jugadorFPS == null ||
            !jugadorFPS.enabled ||
            Keyboard.current == null)
        {
            return;
        }

        // TECLA 1: guardar todas las herramientas.
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            GuardarHerramientas();
            return;
        }

        // TECLA 2: equipar la escoba.
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            if (tiendaHerramientas == null ||
                !tiendaHerramientas.EscobaComprada)
                return;

            EquiparHerramienta(escobaEquipada);
            return;
        }

        // TECLA 3: equipar la pala.
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            if (tiendaHerramientas == null ||
                !tiendaHerramientas.PalaComprada)
                return;

            EquiparHerramienta(palaEquipada);
        }
    }

    private void GuardarHerramientas()
    {
        if (escobaEquipada != null)
            escobaEquipada.SetActive(false);

        if (palaEquipada != null)
            palaEquipada.SetActive(false);
    }

    private void EquiparHerramienta(GameObject herramienta)
    {
        if (herramienta == null)
            return;

        // Nunca permitir dos herramientas visibles.
        GuardarHerramientas();

        herramienta.SetActive(true);
    }
}
