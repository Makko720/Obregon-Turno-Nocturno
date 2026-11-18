using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorVictoria : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private SistemaDinero sistemaDinero;
    [SerializeField] private GameObject panelVictoria;
    [SerializeField] private Text textoPuntuacionFinal;

    [Header("Botones")]
    [SerializeField] private Button buttonJugarDeNuevo;
    [SerializeField] private Button buttonMenuVictoria;

    private bool victoriaMostrada = false;

    private void Awake()
    {
        if (buttonJugarDeNuevo != null)
            buttonJugarDeNuevo.onClick.AddListener(JugarDeNuevo);

        if (buttonMenuVictoria != null)
            buttonMenuVictoria.onClick.AddListener(VolverMenu);
    }

    private void Start()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(false);
    }

    public void MostrarVictoria()
    {
        if (victoriaMostrada)
            return;

        victoriaMostrada = true;

        if (jugadorFPS != null)
            jugadorFPS.enabled = false;

        if (textoPuntuacionFinal != null && sistemaDinero != null)
        {
            textoPuntuacionFinal.text = "PUNTUACIÓN FINAL: $" + sistemaDinero.PuntuacionTotal;
        }

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        Debug.Log("¡Victoria! Has limpiado Ciudad Obregón.");
    }

    public void JugarDeNuevo()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MenuPrincipal");
    }
}