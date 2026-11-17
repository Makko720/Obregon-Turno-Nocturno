
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class IAEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform jugador;
    [SerializeField] private Animator animadorEnemigo;

    [Header("Deteccion")]
    [SerializeField] private float distanciaDeteccion = 12f;

    private NavMeshAgent agente;
    private bool avisoNavegacion = false;

    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (jugador == null || agente == null)
        {
            ActualizarAnimacion(false);
            return;
        }

        // Comprobar si estamos sobre el NavMesh.
        if (!agente.isOnNavMesh)
        {
            ActualizarAnimacion(false);

            if (!avisoNavegacion)
            {
                Debug.LogWarning(
                    gameObject.name +
                    " no esta sobre el NavMesh."
                );

                avisoNavegacion = true;
            }

            return;
        }

        float distanciaJugador = Vector3.Distance(
            transform.position,
            jugador.position
        );

        // El jugador esta fuera del rango.
        if (distanciaJugador > distanciaDeteccion)
        {
            if (agente.hasPath)
                agente.ResetPath();

            ActualizarAnimacion(false);
            return;
        }

        // El jugador esta dentro del rango.
        if (distanciaJugador > agente.stoppingDistance)
        {
            agente.SetDestination(jugador.position);

            // Animar solo cuando realmente se mueve.
            bool estaCaminando =
                agente.velocity.sqrMagnitude > 0.01f;

            ActualizarAnimacion(estaCaminando);
        }
        else
        {
            // Detenernos al alcanzar al jugador.
            if (agente.hasPath)
                agente.ResetPath();

            ActualizarAnimacion(false);
        }
    }

    public void AsignarJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
    }
    private void ActualizarAnimacion(bool caminando)
    {
        if (animadorEnemigo == null)
            return;

        animadorEnemigo.SetBool(
            "Caminando",
            caminando
        );
    }
}
