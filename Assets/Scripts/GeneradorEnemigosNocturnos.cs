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

    [Header("Oleadas Noche 2")]
    [SerializeField] private int[] oleadasNoche2 = { 4, 6 };
    [SerializeField] private float pausaEntreOleadas = 3f;

    [Header("Seguridad")]
    [SerializeField] private float distanciaMinimaJugador = 15f;
    [SerializeField] private float radioBusquedaNavMesh = 3f;

    private readonly List<VidaEnemigo> enemigos = new List<VidaEnemigo>();

    private bool combateNocturnoActivo;
    private bool transicionandoOleada;
    private int ultimoConteo = -1;

    private int[] configuracionOleadasActual;
    private int indiceOleadaActual = 0;

    public bool ConfiguracionValida()
    {
        if (sistemaDiaNoche == null || jugador == null || prefabEnemigo == null)
        {
            Debug.LogWarning("Faltan referencias en GeneradorEnemigosNocturnos.");
            return false;
        }

        if (puntosAparicion == null || puntosAparicion.Length == 0)
        {
            Debug.LogWarning("Debes configurar al menos un punto de aparicion.");
            return false;
        }

        for (int i = 0; i < puntosAparicion.Length; i++)
        {
            Transform punto = puntosAparicion[i];

            if (punto == null)
            {
                Debug.LogWarning("Falta configurar el punto " + (i + 1));
                return false;
            }

            if (!NavMesh.SamplePosition(
                punto.position,
                out NavMeshHit lugar,
                radioBusquedaNavMesh,
                NavMesh.AllAreas))
            {
                Debug.LogWarning(punto.name + " no tiene NavMesh cercano.");
                return false;
            }

            float distancia = Vector3.Distance(lugar.position, jugador.transform.position);
            if (distancia < distanciaMinimaJugador)
            {
                Debug.LogWarning(
                    punto.name + " esta demasiado cerca del jugador (" +
                    distancia.ToString("F1") + "m)."
                );
                return false;
            }
        }

        return true;
    }

    public void GenerarOleada(int diaNoche = 1)
    {
        if (combateNocturnoActivo || !ConfiguracionValida())
            return;

        if (diaNoche == 2 && oleadasNoche2 != null && oleadasNoche2.Length > 0)
        {
            configuracionOleadasActual = oleadasNoche2;
        }
        else
        {
            configuracionOleadasActual = new int[] { puntosAparicion.Length };
        }

        indiceOleadaActual = 0;
        combateNocturnoActivo = true;
        transicionandoOleada = false;

        InstanciarOleadaActual();
    }

    private void InstanciarOleadaActual()
    {
        enemigos.Clear();

        int cantidadEnemigos = configuracionOleadasActual[indiceOleadaActual];
        List<Transform> puntosSeguros = ObtenerPuntosSeguros();

        if (puntosSeguros.Count == 0)
        {
            Debug.LogWarning("No se encontraron puntos seguros sobre NavMesh.");
            return;
        }

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            Transform puntoElegido = puntosSeguros[i % puntosSeguros.Count];

            NavMesh.SamplePosition(
                puntoElegido.position,
                out NavMeshHit lugar,
                radioBusquedaNavMesh,
                NavMesh.AllAreas
            );

            if (i >= puntosSeguros.Count)
            {
                Vector2 circulo = Random.insideUnitCircle.normalized * 1.8f;
                Vector3 candidato = lugar.position + new Vector3(circulo.x, 0f, circulo.y);
                if (NavMesh.SamplePosition(candidato, out NavMeshHit lugarOffset, 2f, NavMesh.AllAreas))
                {
                    lugar = lugarOffset;
                }
            }

            VidaEnemigo enemigo = Instantiate(
                prefabEnemigo,
                lugar.position,
                puntoElegido.rotation
            );

            IAEnemigo ia = enemigo.GetComponent<IAEnemigo>();
            AtaqueEnemigo ataque = enemigo.GetComponent<AtaqueEnemigo>();

            if (ia != null)
                ia.AsignarJugador(jugador.transform);

            if (ataque != null)
                ataque.AsignarJugador(jugador);

            enemigos.Add(enemigo);
        }

        ultimoConteo = enemigos.Count;
        int numeroOleada = indiceOleadaActual + 1;
        int totalOleadas = configuracionOleadasActual.Length;

        sistemaDiaNoche.ActualizarProgresoOleada(
            ultimoConteo,
            enemigos.Count,
            numeroOleada,
            totalOleadas
        );

        Debug.Log(
            "Oleada " + numeroOleada + "/" + totalOleadas +
            " iniciada. Enemigos: " + ultimoConteo
        );
    }

    private List<Transform> ObtenerPuntosSeguros()
    {
        List<Transform> seguros = new List<Transform>();
        List<Transform> todosEnNavMesh = new List<Transform>();

        foreach (Transform punto in puntosAparicion)
        {
            if (punto == null)
                continue;

            if (!NavMesh.SamplePosition(
                punto.position,
                out NavMeshHit lugar,
                radioBusquedaNavMesh,
                NavMesh.AllAreas))
                continue;

            todosEnNavMesh.Add(punto);

            float distancia = Vector3.Distance(lugar.position, jugador.transform.position);
            if (distancia >= distanciaMinimaJugador)
            {
                seguros.Add(punto);
            }
        }

        if (seguros.Count > 0)
            return seguros;

        todosEnNavMesh.Sort((a, b) =>
            Vector3.Distance(b.position, jugador.transform.position)
            .CompareTo(Vector3.Distance(a.position, jugador.transform.position))
        );

        return todosEnNavMesh;
    }

    private void Update()
    {
        if (!combateNocturnoActivo || transicionandoOleada)
            return;

        int enemigosVivos = 0;

        foreach (VidaEnemigo enemigo in enemigos)
        {
            if (enemigo != null && !enemigo.EstaDerrotado)
                enemigosVivos++;
        }

        int numeroOleada = indiceOleadaActual + 1;
        int totalOleadas = configuracionOleadasActual.Length;

        if (enemigosVivos != ultimoConteo)
        {
            ultimoConteo = enemigosVivos;

            sistemaDiaNoche.ActualizarProgresoOleada(
                enemigosVivos,
                enemigos.Count,
                numeroOleada,
                totalOleadas
            );
        }

        if (enemigosVivos == 0)
        {
            transicionandoOleada = true;

            if (indiceOleadaActual + 1 < configuracionOleadasActual.Length)
            {
                StartCoroutine(IniciarSiguienteOleada());
            }
            else
            {
                StartCoroutine(FinalizarNoche());
            }
        }
    }

    private IEnumerator IniciarSiguienteOleada()
    {
        yield return new WaitForSeconds(pausaEntreOleadas);

        indiceOleadaActual++;
        transicionandoOleada = false;
        InstanciarOleadaActual();
    }

    private IEnumerator FinalizarNoche()
    {
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
        combateNocturnoActivo = false;

        Debug.Log("¡Todas las oleadas de la noche fueron derrotadas!");
        sistemaDiaNoche.IniciarAmanecer();
    }
}