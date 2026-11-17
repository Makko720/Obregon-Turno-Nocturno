
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AtaqueEscoba : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform escobaEquipada;
    [SerializeField] private Transform palaEquipada;
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private InventarioJugador inventarioJugador;

    [Header("Combate")]
    [SerializeField] private int danioEscoba = 25;
    [SerializeField] private int danioPala = 40;
    [SerializeField] private float alcanceGolpe = 3f;

    [Header("Animacion de la escoba")]
    [SerializeField] private float duracionGolpe = 0.12f;
    [SerializeField] private float duracionRegreso = 0.20f;
    [SerializeField] private float esperaEntreAtaques = 0.15f;

    [SerializeField] private Vector3 rotacionAtaque =
        new Vector3(-15f, 0f, -65f);

    [Header("Animacion de la pala")]
    [SerializeField] private float duracionGolpePala = 0.25f;
    [SerializeField] private float duracionRegresoPala = 0.30f;
    [SerializeField] private float esperaEntreAtaquesPala = 0.25f;

    [SerializeField] private Vector3 rotacionAtaquePala =
        new Vector3(-30f, 0f, -85f);

    private bool atacando = false;

    private void Update()
    {
        if (jugadorFPS == null || Mouse.current == null)
            return;

        if (MenuPausa.EstaPausado ||
            SistemaDialogos.DialogoActivo ||
            !jugadorFPS.enabled ||
            atacando)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // ESCOBA
        if (escobaEquipada != null &&
            escobaEquipada.gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimarAtaque(
                escobaEquipada,
                danioEscoba,
                duracionGolpe,
                duracionRegreso,
                esperaEntreAtaques,
                rotacionAtaque,
                true
            ));

            return;
        }

        // PALA
        if (palaEquipada != null &&
            palaEquipada.gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimarAtaque(
                palaEquipada,
                danioPala,
                duracionGolpePala,
                duracionRegresoPala,
                esperaEntreAtaquesPala,
                rotacionAtaquePala,
                false
            ));
        }
    }

    private IEnumerator AnimarAtaque(
        Transform herramienta,
        int danio,
        float tiempoGolpe,
        float tiempoRegreso,
        float espera,
        Vector3 rotacion,
        bool esEscoba
    )
    {
        atacando = true;

        Quaternion reposo = herramienta.localRotation;

        Quaternion golpe =
            reposo * Quaternion.Euler(rotacion);

        yield return GirarHerramienta(
            herramienta,
            reposo,
            golpe,
            tiempoGolpe
        );

        if (herramienta.gameObject.activeInHierarchy &&
            !MenuPausa.EstaPausado &&
            !SistemaDialogos.DialogoActivo &&
            jugadorFPS != null &&
            jugadorFPS.enabled)
        {
            DetectarImpacto(danio, esEscoba);
        }

        yield return GirarHerramienta(
            herramienta,
            golpe,
            reposo,
            tiempoRegreso
        );

        herramienta.localRotation = reposo;

        yield return new WaitForSeconds(espera);

        atacando = false;
    }

    private IEnumerator GirarHerramienta(
        Transform herramienta,
        Quaternion inicio,
        Quaternion destino,
        float duracion
    )
    {
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;

            float progreso = Mathf.Clamp01(
                tiempo / Mathf.Max(0.01f, duracion)
            );

            herramienta.localRotation =
                Quaternion.Slerp(inicio, destino, progreso);

            yield return null;
        }

        herramienta.localRotation = destino;
    }

    private void DetectarImpacto(int danio, bool esEscoba)
    {
        if (camaraJugador == null)
        {
            Debug.LogWarning(
                "Falta asignar la camara del jugador."
            );
            return;
        }

        Ray rayo = new Ray(
            camaraJugador.transform.position,
            camaraJugador.transform.forward
        );

        if (!Physics.Raycast(
            rayo,
            out RaycastHit impacto,
            alcanceGolpe,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
            return;

        // PRIMERO: comprobar si golpeamos basura barrible.
        BasuraBarrible basura =
            impacto.collider.GetComponentInParent<BasuraBarrible>();

        if (basura != null)
        {
            // Solo la escoba puede limpiar este residuo.
            if (esEscoba)
            {
                basura.RecibirBarrido(inventarioJugador);
            }
            else
            {
                Debug.Log(
                    "Necesitas la escoba para limpiar esta basura."
                );
            }

            return;
        }

        // SEGUNDO: comprobar si golpeamos un enemigo.
        VidaEnemigo enemigo =
            impacto.collider.GetComponentInParent<VidaEnemigo>();

        if (enemigo != null)
        {
            enemigo.RecibirDanio(danio);
        }
    }
}
