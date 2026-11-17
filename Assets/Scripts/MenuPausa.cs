
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class MenuPausa : MonoBehaviour
{
    public static bool EstaPausado { get; private set; }

    [Header("Menu de pausa")]
    [SerializeField] private GameObject panelPausa;
    [SerializeField] private GameObject panelInicio;
    [SerializeField] private GameObject panelEstadisticas;
    [SerializeField] private GameObject panelOpciones;

    [Header("Mezclador de audio")]
    [SerializeField] private AudioMixer mezcladorAudio;

    [Header("Sliders")]
    [SerializeField] private Slider sliderGeneral;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;

    private void Awake()
    {
        Time.timeScale = 1f;
        EstaPausado = false;
        panelPausa.SetActive(false);
    }

    private void Start()
    {
        ConfigurarAudio();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (EstaPausado)
                VolverAlJuego();
            else
                AbrirPausa();
        }
    }

    public void AbrirPausa()
    {
        EstaPausado = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        panelPausa.SetActive(true);
        MostrarInicio();
    }

    public void VolverAlJuego()
    {
        panelPausa.SetActive(false);

        Time.timeScale = 1f;
        EstaPausado = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MostrarInicio()
    {
        panelInicio.SetActive(true);
        panelEstadisticas.SetActive(false);
        panelOpciones.SetActive(false);
    }

    public void MostrarEstadisticas()
    {
        panelInicio.SetActive(false);
        panelEstadisticas.SetActive(true);
        panelOpciones.SetActive(false);
    }

    public void MostrarOpciones()
    {
        panelInicio.SetActive(false);
        panelEstadisticas.SetActive(false);
        panelOpciones.SetActive(true);
    }

    private void ConfigurarAudio()
    {
        float general =
            PlayerPrefs.GetFloat("VolGeneral", 1f);
        float musica =
            PlayerPrefs.GetFloat("VolMusica", 1f);
        float efectos =
            PlayerPrefs.GetFloat("VolEfectos", 1f);

        sliderGeneral.SetValueWithoutNotify(general);
        sliderMusica.SetValueWithoutNotify(musica);
        sliderEfectos.SetValueWithoutNotify(efectos);

        AplicarVolumen("VolGeneral", general);
        AplicarVolumen("VolMusica", musica);
        AplicarVolumen("VolEfectos", efectos);

        sliderGeneral.onValueChanged.AddListener(
            valor => CambiarVolumen("VolGeneral", valor));

        sliderMusica.onValueChanged.AddListener(
            valor => CambiarVolumen("VolMusica", valor));

        sliderEfectos.onValueChanged.AddListener(
            valor => CambiarVolumen("VolEfectos", valor));
    }

    private void CambiarVolumen(string parametro, float valor)
    {
        AplicarVolumen(parametro, valor);

        PlayerPrefs.SetFloat(parametro, valor);
        PlayerPrefs.Save();
    }

    private void AplicarVolumen(string parametro, float valor)
    {
        float decibelios = valor <= 0.0001f
            ? -80f
            : Mathf.Log10(valor) * 20f;

        if (!mezcladorAudio.SetFloat(parametro, decibelios))
        {
            Debug.LogWarning(
                "No se encontro el parametro: " + parametro);
        }
    }

    public void SalirDelJuego()
    {
        Time.timeScale = 1f;
        EstaPausado = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (EstaPausado)
            Time.timeScale = 1f;

        EstaPausado = false;
    }
}
