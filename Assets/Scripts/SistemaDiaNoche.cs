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

    public void IniciarNoche()
    {
        if (EsDeNoche || transicionEnCurso)
            return;

        if (DiaActual != 1 && DiaActual != 2)
        {
            Debug.Log(
                "La siguiente noche aun no esta desbloqueada."
            );
            return;
        }

        if (luzSol == null || camaraJugador == null ||
            generadorEnemigos == null)
        {
            Debug.LogWarning(
                "Faltan referencias en SistemaDiaNoche."
            );
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

            yield return null;
        }

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

            yield return null;
        }

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
}