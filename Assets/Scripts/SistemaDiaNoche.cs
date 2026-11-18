using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SistemaDiaNoche : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Light luzSol;
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private GeneradorEnemigosNocturnos generadorEnemigos;
    [SerializeField] private SistemaMisiones sistemaMisiones;
    [SerializeField] private RegeneradorBasura regeneradorBasura;
    [SerializeField] private JugadorFPS jugadorFPS;

    [Header("Audio Ambiente")]
    [SerializeField] private AudioSource audioDia;
    [SerializeField] private AudioSource audioNoche;
    [SerializeField, Range(0f, 1f)] private float volumenMaximoAudio = 0.5f;

    [Header("Transicion")]
    [SerializeField] private float duracionTransicion = 4f;

    [Header("Iluminacion nocturna")]
    [SerializeField] private float intensidadSolNoche = 0.08f;

    [SerializeField] private Color ambienteNoche =
        new Color(0.12f, 0.15f, 0.23f);

    [Header("Colores del cielo")]
    [SerializeField] private Color cieloDia =
        new Color(0.42f, 0.72f, 0.92f);

    [SerializeField] private Color cieloNoche =
        new Color(0.02f, 0.04f, 0.11f);

    public int DiaActual { get; private set; } = 1;
    public bool EsDeNoche { get; private set; }

    private bool transicionEnCurso;

    private float intensidadSolDia;
    private Color ambienteDia;
    private AmbientMode modoAmbienteDia;
    private CameraClearFlags modoCamaraDia;
    private Color fondoCamaraDia;

    private void Awake()
    {
        if (luzSol != null)
            intensidadSolDia = luzSol.intensity;

        ambienteDia = RenderSettings.ambientLight;
        modoAmbienteDia = RenderSettings.ambientMode;

        if (camaraJugador != null)
        {
            modoCamaraDia = camaraJugador.clearFlags;
            fondoCamaraDia = camaraJugador.backgroundColor;
        }
    }

    private void Start()
    {
        if (audioDia != null)
        {
            audioDia.loop = true;
            audioDia.volume = volumenMaximoAudio;
            if (!audioDia.isPlaying)
                audioDia.Play();
        }

        if (audioNoche != null)
        {
            audioNoche.loop = true;
            audioNoche.volume = 0f;
            audioNoche.Stop();
        }
    }

    public void IniciarNoche()
    {
        if (EsDeNoche || transicionEnCurso)
            return;

        if (DiaActual < 1 || DiaActual > 3)
        {
            Debug.Log("La siguiente noche aun no esta desbloqueada.");
            return;
        }

        if (luzSol == null || camaraJugador == null || generadorEnemigos == null)
        {
            Debug.LogWarning("Faltan referencias en SistemaDiaNoche.");
            return;
        }

        if (!generadorEnemigos.ConfiguracionValida())
            return;

        StartCoroutine(TransicionANoche());
    }

    private IEnumerator TransicionANoche()
    {
        transicionEnCurso = true;

        if (jugadorFPS != null)
            jugadorFPS.enabled = false;

        if (audioNoche != null && !audioNoche.isPlaying)
        {
            audioNoche.volume = 0f;
            audioNoche.Play();
        }

        float volumenInicialDia = audioDia != null ? audioDia.volume : 0f;
        float intensidadInicial = luzSol.intensity;
        Color ambienteInicial = RenderSettings.ambientLight;

        camaraJugador.clearFlags = CameraClearFlags.SolidColor;
        camaraJugador.backgroundColor = cieloDia;

        RenderSettings.ambientMode = AmbientMode.Flat;

        float tiempo = 0f;
        float duracion = Mathf.Max(0.01f, duracionTransicion);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / duracion);

            luzSol.intensity = Mathf.Lerp(
                intensidadInicial,
                intensidadSolNoche,
                progreso
            );

            RenderSettings.ambientLight = Color.Lerp(
                ambienteInicial,
                ambienteNoche,
                progreso
            );

            camaraJugador.backgroundColor = Color.Lerp(
                cieloDia,
                cieloNoche,
                progreso
            );

            if (audioDia != null)
                audioDia.volume = Mathf.Lerp(volumenInicialDia, 0f, progreso);

            if (audioNoche != null)
                audioNoche.volume = Mathf.Lerp(0f, volumenMaximoAudio, progreso);

            yield return null;
        }

        if (audioDia != null)
        {
            audioDia.volume = 0f;
            audioDia.Stop();
        }

        if (audioNoche != null)
            audioNoche.volume = volumenMaximoAudio;

        EsDeNoche = true;
        transicionEnCurso = false;

        if (jugadorFPS != null)
            jugadorFPS.enabled = true;

        Debug.Log("Ha comenzado la Noche " + DiaActual);

        generadorEnemigos.GenerarOleada(DiaActual);
    }

    public void ActualizarProgresoOleada(
        int restantes,
        int total,
        int oleadaActual = 1,
        int totalOleadas = 1
    )
    {
        if (!EsDeNoche)
            return;

        if (sistemaMisiones != null)
        {
            sistemaMisiones.ActualizarProgresoOleada(
                DiaActual,
                restantes,
                total,
                oleadaActual,
                totalOleadas
            );
        }
    }

    public void IniciarAmanecer()
    {
        if (!EsDeNoche || transicionEnCurso)
            return;

        StartCoroutine(TransicionADia());
    }

    private IEnumerator TransicionADia()
    {
        transicionEnCurso = true;

        if (audioDia != null && !audioDia.isPlaying)
        {
            audioDia.volume = 0f;
            audioDia.Play();
        }

        float volumenInicialNoche = audioNoche != null ? audioNoche.volume : 0f;
        float intensidadInicial = luzSol.intensity;
        Color ambienteInicial = RenderSettings.ambientLight;
        Color cieloInicial = camaraJugador.backgroundColor;

        float tiempo = 0f;
        float duracion = Mathf.Max(0.01f, duracionTransicion);

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(tiempo / duracion);

            luzSol.intensity = Mathf.Lerp(
                intensidadInicial,
                intensidadSolDia,
                progreso
            );

            RenderSettings.ambientLight = Color.Lerp(
                ambienteInicial,
                ambienteDia,
                progreso
            );

            camaraJugador.backgroundColor = Color.Lerp(
                cieloInicial,
                cieloDia,
                progreso
            );

            if (audioNoche != null)
                audioNoche.volume = Mathf.Lerp(volumenInicialNoche, 0f, progreso);

            if (audioDia != null)
                audioDia.volume = Mathf.Lerp(0f, volumenMaximoAudio, progreso);

            yield return null;
        }

        if (audioNoche != null)
        {
            audioNoche.volume = 0f;
            audioNoche.Stop();
        }

        if (audioDia != null)
            audioDia.volume = volumenMaximoAudio;

        luzSol.intensity = intensidadSolDia;
        RenderSettings.ambientLight = ambienteDia;
        RenderSettings.ambientMode = modoAmbienteDia;

        camaraJugador.clearFlags = modoCamaraDia;
        camaraJugador.backgroundColor = fondoCamaraDia;

        EsDeNoche = false;
        DiaActual++;
        transicionEnCurso = false;

        if (regeneradorBasura != null)
        {
            regeneradorBasura.RegenerarBasura();
        }
        
        if (sistemaMisiones != null)
        {
            sistemaMisiones.RegistrarAmanecer(DiaActual);
        }

        Debug.Log("¡Ha amanecido! Comienza el Dia " + DiaActual);
    }

    [ContextMenu("Prueba - Saltar al Dia 3 (Dormir directo para Jefe)")]
    private void PruebaSaltarDia3DormirDirecto()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Ejecuta esta prueba en Play Mode.");
            return;
        }

        EsDeNoche = false;
        transicionEnCurso = false;
        DiaActual = 3;

        if (sistemaMisiones != null)
            sistemaMisiones.ForzarPruebaDia3(true);

        Debug.Log("PRUEBA: Saltado al Dia 3. Ve a la tienda de campaña y presiona [E] para dormir.");
    }

    [ContextMenu("Prueba - Saltar al Dia 3 (Probar Portal desde cero)")]
    private void PruebaSaltarDia3ConPortal()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Ejecuta esta prueba en Play Mode.");
            return;
        }

        EsDeNoche = false;
        transicionEnCurso = false;
        DiaActual = 3;

        if (regeneradorBasura != null)
            regeneradorBasura.RegenerarBasura();

        if (sistemaMisiones != null)
            sistemaMisiones.ForzarPruebaDia3(false);

        Debug.Log("PRUEBA: Saltado al Dia 3. Recoge 6 residuos y activa el Portal de Basura.");
    }
}