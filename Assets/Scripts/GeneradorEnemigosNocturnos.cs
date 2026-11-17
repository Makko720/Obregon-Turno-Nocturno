
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneradorEnemigosNocturnos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SistemaDiaNoche sistemaDiaNoche;
    [SerializeField] private VidaJugador jugador;
    [SerializeField] private VidaEnemigo prefabEnemigo;

    [Header("Puntos de aparicion")]
    [SerializeField] private Transform[] puntosAparicion = new Transform[5];

    [Header("Seguridad")]
    [SerializeField] private float distanciaMinimaJugador = 15f;
    [SerializeField] private float radioBusquedaNavMesh = 3f;

    private readonly List<VidaEnemigo> enemigos =
        new List<VidaEnemigo>();

    private bool oleadaActiva;
    private bool finalizandoOleada;
    private int ultimoConteo = -1;

    public bool ConfiguracionValida()
    {
        if (sistemaDiaNoche == null ||
            jugador == null ||
            prefabEnemigo == null)
        {
            Debug.LogWarning(
                "Faltan referencias en GeneradorEnemigosNocturnos."
            );
            return false;
        }

        if (puntosAparicion == null ||
            puntosAparicion.Length != 5)
        {
            Debug.LogWarning(
                "Debes configurar exactamente 5 puntos de aparicion."
            );
            return false;
        }

        for (int i = 0; i < puntosAparicion.Length; i++)
        {
            Transform punto = puntosAparicion[i];

            if (punto == null)
            {
                Debug.LogWarning(
                    "Falta configurar el punto " + (i + 1)
                );
                return false;
            }

            if (!NavMesh.SamplePosition(
                punto.position,
                out NavMeshHit lugar,
                radioBusquedaNavMesh,
                NavMesh.AllAreas))
            {
                Debug.LogWarning(
                    punto.name + " no tiene NavMesh cercano."
                );
                return false;
            }

            float distancia = Vector3.Distance(
                lugar.position,
                jugador.transform.position
            );

            if (distancia < distanciaMinimaJugador)
            {
                Debug.LogWarning(
                    punto.name +
                    " esta demasiado cerca del jugador. " +
                    "Alejalo hasta al menos " +
                    distanciaMinimaJugador + " metros."
                );
                return false;
            }
        }

        return true;
    }

    public void GenerarOleada()
    {
        if (oleadaActiva || !ConfiguracionValida())
            return;

        enemigos.Clear();
        finalizandoOleada = false;

        foreach (Transform punto in puntosAparicion)
        {
            NavMesh.SamplePosition(
                punto.position,
                out NavMeshHit lugar,
                radioBusquedaNavMesh,
                NavMesh.AllAreas
            );

            VidaEnemigo enemigo = Instantiate(
                prefabEnemigo,
                lugar.position,
                punto.rotation
            );

            IAEnemigo ia = enemigo.GetComponent<IAEnemigo>();
            AtaqueEnemigo ataque =
                enemigo.GetComponent<AtaqueEnemigo>();

            if (ia != null)
                ia.AsignarJugador(jugador.transform);

            if (ataque != null)
                ataque.AsignarJugador(jugador);

            enemigos.Add(enemigo);
        }

        oleadaActiva = true;
        ultimoConteo = enemigos.Count;
        sistemaDiaNoche.ActualizarProgresoOleada(
            ultimoConteo,
            enemigos.Count
        );

        Debug.Log(
            "¡Comenzo la oleada! Enemigos restantes: " +
            ultimoConteo
        );
    }

    private void Update()
    {
        if (!oleadaActiva || finalizandoOleada)
            return;

        int enemigosVivos = 0;

        foreach (VidaEnemigo enemigo in enemigos)
        {
            if (enemigo != null && !enemigo.EstaDerrotado)
                enemigosVivos++;
        }

        if (enemigosVivos != ultimoConteo)
        {
            ultimoConteo = enemigosVivos;

            Debug.Log(
                "Enemigos restantes: " + enemigosVivos
            );

            sistemaDiaNoche.ActualizarProgresoOleada(
                enemigosVivos,
                enemigos.Count
            );
        }

        if (enemigosVivos == 0)
        {
            finalizandoOleada = true;
            StartCoroutine(FinalizarOleada());
        }
    }

    private IEnumerator FinalizarOleada()
    {
        Debug.Log(
            "¡Oleada completada! Esperando animaciones de muerte."
        );

        // Esperar a que desaparezcan los cuerpos.
        bool quedanCuerpos = true;

        while (quedanCuerpos)
        {
            quedanCuerpos = false;

            foreach (VidaEnemigo enemigo in enemigos)
            {
                if (enemigo != null)
                {
                    quedanCuerpos = true;
                    break;
                }
            }

            if (quedanCuerpos)
                yield return null;
        }

        enemigos.Clear();
        oleadaActiva = false;

        Debug.Log("¡Todos los enemigos fueron derrotados!");

        sistemaDiaNoche.IniciarAmanecer();
    }
}
