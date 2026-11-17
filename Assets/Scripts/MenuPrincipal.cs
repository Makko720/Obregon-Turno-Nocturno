
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Ventana de opciones")]
    [SerializeField] private GameObject panelOpciones;

    [Header("Audio")]
    [SerializeField] private AudioMixer mezcladorAudio;

    [Header("Sliders")]
    [SerializeField] private Slider sliderGeneral;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;

    private void Start()
    {
        // La ventana inicia oculta.
        panelOpciones.SetActive(false);

        // Recuperar volumen guardado.
        float general = PlayerPrefs.GetFloat("VolGeneral", 1f);
        float musica = PlayerPrefs.GetFloat("VolMusica", 1f);
        float efectos = PlayerPrefs.GetFloat("VolEfectos", 1f);

        // Establecer valores sin disparar eventos.
        sliderGeneral.SetValueWithoutNotify(general);
        sliderMusica.SetValueWithoutNotify(musica);
        sliderEfectos.SetValueWithoutNotify(efectos);

        // Aplicar los volúmenes al mezclador.
        AplicarVolumen("VolGeneral", general);
        AplicarVolumen("VolMusica", musica);
        AplicarVolumen("VolEfectos", efectos);

        // Escuchar los cambios de los sliders.
        sliderGeneral.onValueChanged.AddListener(CambiarGeneral);
        sliderMusica.onValueChanged.AddListener(CambiarMusica);
        sliderEfectos.onValueChanged.AddListener(CambiarEfectos);
    }

    public void AbrirOpciones()
    {
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
    }

    private void CambiarGeneral(float valor)
    {
        GuardarVolumen("VolGeneral", valor);
    }

    private void CambiarMusica(float valor)
    {
        GuardarVolumen("VolMusica", valor);
    }

    private void CambiarEfectos(float valor)
    {
        GuardarVolumen("VolEfectos", valor);
    }

    private void GuardarVolumen(string parametro, float valor)
    {
        AplicarVolumen(parametro, valor);

        PlayerPrefs.SetFloat(parametro, valor);
        PlayerPrefs.Save();
    }

    private void AplicarVolumen(string parametro, float valor)
    {
        // Convertir de 0-1 a decibelios.
        float decibelios = valor <= 0.0001f
            ? -80f
            : Mathf.Log10(valor) * 20f;

        if (!mezcladorAudio.SetFloat(parametro, decibelios))
        {
            Debug.LogWarning(
                "No se encontró el parámetro: " + parametro
            );
        }
    }
    
    public void IniciarPartida()
    {
        Debug.Log("Iniciando Obregón: Turno Nocturno...");
        SceneManager.LoadScene("SectorInicialCiudad");
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del videojuego...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
