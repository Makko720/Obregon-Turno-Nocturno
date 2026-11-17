
using UnityEngine;
using UnityEngine.UI;

public class ControladorTienda : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject contenidoHerramientas;
    [SerializeField] private GameObject contenidoMejoras;
    [SerializeField] private GameObject contenidoVender;

    [Header("Botones")]
    [SerializeField] private Button buttonHerramientas;
    [SerializeField] private Button buttonMejoras;
    [SerializeField] private Button buttonVender;

    private readonly Color colorActivo =
        new Color32(197, 231, 182, 255);

    private readonly Color colorInactivo =
        new Color32(45, 62, 71, 255);

    private readonly Color textoInactivo =
        new Color32(240, 242, 239, 255);

    private void Awake()
    {
        if (buttonHerramientas != null)
            buttonHerramientas.onClick.AddListener(MostrarHerramientas);

        if (buttonMejoras != null)
            buttonMejoras.onClick.AddListener(MostrarMejoras);

        if (buttonVender != null)
            buttonVender.onClick.AddListener(MostrarVender);
    }

    private void OnEnable()
    {
        MostrarHerramientas();
    }

    public void MostrarHerramientas()
    {
        CambiarPestana(0);
    }

    public void MostrarMejoras()
    {
        CambiarPestana(1);
    }

    public void MostrarVender()
    {
        CambiarPestana(2);
    }

    private void CambiarPestana(int pestana)
    {
        if (contenidoHerramientas != null)
            contenidoHerramientas.SetActive(pestana == 0);

        if (contenidoMejoras != null)
            contenidoMejoras.SetActive(pestana == 1);

        if (contenidoVender != null)
            contenidoVender.SetActive(pestana == 2);

        ActualizarBoton(buttonHerramientas, pestana == 0);
        ActualizarBoton(buttonMejoras, pestana == 1);
        ActualizarBoton(buttonVender, pestana == 2);
    }

    private void ActualizarBoton(Button boton, bool activo)
    {
        if (boton == null)
            return;

        // Cambiar el fondo.
        if (boton.image != null)
            boton.image.color =
                activo ? colorActivo : colorInactivo;

        // Cambiar también el texto.
        Text texto = boton.GetComponentInChildren<Text>(true);

        if (texto != null)
            texto.color =
                activo ? colorInactivo : textoInactivo;
    }
}
