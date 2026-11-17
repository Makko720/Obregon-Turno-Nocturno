
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorMochila : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private JugadorFPS jugadorFPS;

    private bool mochilaAbierta = false;
    private bool restaurarControles = false;

    private void Start()
    {
        // La mochila comienza cerrada.
        if (panelInventario != null)
            panelInventario.SetActive(false);
    }

    private void Update()
    {
        // Si se abre el menu de pausa, cerrar la mochila.
        if (MenuPausa.EstaPausado)
        {
            if (mochilaAbierta)
            {
                mochilaAbierta = false;
                panelInventario.SetActive(false);
                restaurarControles = true;
            }

            return;
        }

        // Recuperar controles despues de salir de pausa.
        if (restaurarControles)
        {
            restaurarControles = false;

            if (jugadorFPS != null)
                jugadorFPS.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            AlternarMochila();
    }

    private void AlternarMochila()
    {
        if (panelInventario == null || jugadorFPS == null)
        {
            Debug.LogWarning(
                "Faltan referencias del controlador de mochila."
            );
            return;
        }

        mochilaAbierta = !mochilaAbierta;

        panelInventario.SetActive(mochilaAbierta);

        // Evitar caminar o mover la camara con la mochila abierta.
        jugadorFPS.enabled = !mochilaAbierta;

        // Liberar o bloquear el cursor.
        Cursor.lockState = mochilaAbierta
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = mochilaAbierta;
    }
}
