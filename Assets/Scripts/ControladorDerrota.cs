
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorDerrota : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VidaJugador vidaJugador;
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private GameObject panelDerrota;

    [Header("Botones")]
    [SerializeField] private Button buttonReintentar;
    [SerializeField] private Button buttonMenuDerrota;

    private bool derrotaMostrada = false;

    private void Awake()
    {
        if (buttonReintentar != null)
            buttonReintentar.onClick.AddListener(Reintentar);

        if (buttonMenuDerrota != null)
            buttonMenuDerrota.onClick.AddListener(VolverMenu);
    }

    private void Start()
    {
        // La pantalla de derrota comienza oculta.
        if (panelDerrota != null)
            panelDerrota.SetActive(false);
    }

    private void Update()
    {
        if (derrotaMostrada || vidaJugador == null)
            return;

        // Comprobar si el jugador fue derrotado.
        if (vidaJugador.EstaDerrotado)
        {
            MostrarDerrota();
        }
    }

    private void MostrarDerrota()
    {
        derrotaMostrada = true;

        // Bloquear el movimiento del jugador.
        if (jugadorFPS != null)
            jugadorFPS.enabled = false;

        // Mostrar la pantalla de derrota.
        if (panelDerrota != null)
            panelDerrota.SetActive(true);

        // Liberar el cursor para usar los botones.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Detener el juego.
        Time.timeScale = 0f;

        Debug.Log("Pantalla de derrota activada.");
    }

    public void Reintentar()
    {
        RestaurarTiempo();

        // Reiniciar la escena actual.
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void VolverMenu()
    {
        // Restaurar la velocidad normal del juego.
        Time.timeScale = 1f;

        // Liberar el cursor para usar el menu principal.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Regresar al menu principal.
        SceneManager.LoadScene("MenuPrincipal");
    }


    private void RestaurarTiempo()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
