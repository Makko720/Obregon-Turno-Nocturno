
using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VidaJugador jugador;
    [SerializeField] private Animator animadorEnemigo;

    [Header("Configuracion del ataque")]
    [SerializeField] private int danioAtaque = 15;
    [SerializeField] private float distanciaAtaque = 1.7f;
    [SerializeField] private float tiempoEntreAtaques = 3f;
    [SerializeField] private float duracionCombo = 2.7f;

    private float siguienteAtaque;
    private float finCombo;

    private bool comboActivo;
    private int impactosRegistrados;
    private int danioAcumulado;

    private void Update()
    {
        // Finalizar el combo cuando termine su duracion.
        if (comboActivo && Time.time >= finCombo)
        {
            comboActivo = false;
        }

        if (jugador == null || animadorEnemigo == null)
            return;

        // No atacar a un jugador derrotado.
        if (jugador.EstaDerrotado)
            return;

        // No iniciar otro ataque si el combo sigue activo.
        if (comboActivo)
            return;

        // Respetar el intervalo entre combos.
        if (Time.time < siguienteAtaque)
            return;

        float distancia = Vector3.Distance(
            transform.position,
            jugador.transform.position
        );

        // Solo atacar cuando el jugador esta cerca.
        if (distancia > distanciaAtaque)
            return;

        // Preparar un nuevo combo.
        comboActivo = true;
        impactosRegistrados = 0;
        danioAcumulado = 0;

        finCombo = Time.time + duracionCombo;

        siguienteAtaque = Time.time + Mathf.Max(
            tiempoEntreAtaques,
            duracionCombo + 0.1f
        );

        // Iniciar la animacion, sin aplicar daño inmediato.
        animadorEnemigo.SetTrigger("Atacar");
    }

    // Este metodo sera llamado por los Animation Events.
    public void AplicarImpacto(int danio)
    {
        if (!comboActivo || jugador == null)
            return;

        if (jugador.EstaDerrotado)
            return;

        // El combo tiene un maximo de dos impactos.
        if (impactosRegistrados >= 2)
            return;

        // Registrar el impacto aunque el jugador lo esquive.
        impactosRegistrados++;

        float distancia = Vector3.Distance(
            transform.position,
            jugador.transform.position
        );

        // Si el jugador se alejo, el golpe no hace daño.
        if (distancia > distanciaAtaque)
        {
            Debug.Log(
                gameObject.name + " fallo un espadazo."
            );
            return;
        }

        // Evitar superar los 15 HP por combo.
        int danioRestante = danioAtaque - danioAcumulado;

        int danioFinal = Mathf.Clamp(
            danio,
            0,
            danioRestante
        );

        if (danioFinal <= 0)
            return;

        danioAcumulado += danioFinal;

        // Aplicar daño en el instante del impacto.
        jugador.RecibirDanio(danioFinal);

        Debug.Log(
            gameObject.name +
            " impacto al jugador e hizo " +
            danioFinal + " de daño."
        );
    }

    public void CancelarCombo()
    {
        comboActivo = false;
        impactosRegistrados = 0;
        danioAcumulado = 0;

        if (animadorEnemigo != null)
        {
            animadorEnemigo.ResetTrigger("Atacar");
        }

        // Evitar un nuevo ataque inmediatamente despues del golpe.
        siguienteAtaque = Mathf.Max(
            siguienteAtaque,
            Time.time + tiempoEntreAtaques
        );
    }
    public void AsignarJugador(VidaJugador nuevoJugador)
    {
        jugador = nuevoJugador;
    }
}
