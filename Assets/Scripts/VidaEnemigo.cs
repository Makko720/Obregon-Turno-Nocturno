using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Vida del enemigo")]
    [SerializeField] private int vidaMaxima = 100;

    [Header("Animaciones")]
    [SerializeField] private Animator animadorEnemigo;
    [SerializeField] private float tiempoReaccion = 0.9f;
    [SerializeField] private float tiempoDesaparicion = 3f;

    public int VidaActual { get; private set; }
    public int VidaMaxima => vidaMaxima;
    public bool EstaDerrotado => derrotado;

    private bool derrotado = false;

    private NavMeshAgent agente;
    private IAEnemigo iaEnemigo;
    private AtaqueEnemigo ataqueEnemigo;
    private Coroutine rutinaReaccion;

    private void Awake()
    {
        VidaActual = vidaMaxima;

        agente = GetComponent<NavMeshAgent>();
        iaEnemigo = GetComponent<IAEnemigo>();
        ataqueEnemigo = GetComponent<AtaqueEnemigo>();

        if (animadorEnemigo == null)
        {
            animadorEnemigo = GetComponentInChildren<Animator>();
        }
    }

    public void RecibirDanio(int cantidad)
    {
        if (derrotado || cantidad <= 0)
            return;

        VidaActual = Mathf.Max(VidaActual - cantidad, 0);

        Debug.Log(
            gameObject.name +
            " recibio " + cantidad +
            " de daño. Vida restante: " + VidaActual
        );

        if (VidaActual <= 0)
        {
            Morir();
            return;
        }

        if (ataqueEnemigo != null)
        {
            ataqueEnemigo.CancelarCombo();
        }

        if (rutinaReaccion != null)
        {
            StopCoroutine(rutinaReaccion);
        }

        rutinaReaccion = StartCoroutine(ReaccionarAlGolpe());
    }

    private IEnumerator ReaccionarAlGolpe()
    {
        if (iaEnemigo != null)
            iaEnemigo.enabled = false;

        if (ataqueEnemigo != null)
            ataqueEnemigo.enabled = false;

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.ResetPath();
        }

        if (animadorEnemigo != null)
        {
            animadorEnemigo.SetBool("Caminando", false);
            animadorEnemigo.ResetTrigger("Atacar");
            animadorEnemigo.SetTrigger("RecibirGolpe");
        }

        yield return new WaitForSeconds(tiempoReaccion);

        if (derrotado)
            yield break;

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = false;
        }

        if (iaEnemigo != null)
            iaEnemigo.enabled = true;

        if (ataqueEnemigo != null)
            ataqueEnemigo.enabled = true;

        rutinaReaccion = null;
    }

    private void Morir()
    {
        if (derrotado)
            return;

        derrotado = true;

        if (rutinaReaccion != null)
        {
            StopCoroutine(rutinaReaccion);
            rutinaReaccion = null;
        }

        if (ataqueEnemigo != null)
        {
            ataqueEnemigo.CancelarCombo();
            ataqueEnemigo.enabled = false;
        }

        if (iaEnemigo != null)
        {
            iaEnemigo.enabled = false;
        }

        if (agente != null && agente.isOnNavMesh)
        {
            agente.isStopped = true;
            agente.ResetPath();
        }

        if (animadorEnemigo != null)
        {
            animadorEnemigo.SetBool("Caminando", false);
            animadorEnemigo.ResetTrigger("Atacar");
            animadorEnemigo.ResetTrigger("RecibirGolpe");
            animadorEnemigo.SetTrigger("Morir");
        }

        Collider colision = GetComponent<Collider>();

        if (colision != null)
        {
            colision.enabled = false;
        }

        Debug.Log(gameObject.name + " ha sido derrotado.");

        Destroy(gameObject, tiempoDesaparicion);
    }

    [ContextMenu("Prueba - Recibir 25 de daño")]
    private void ProbarDanio()
    {
        RecibirDanio(25);
    }
}