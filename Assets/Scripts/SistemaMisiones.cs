
using UnityEngine;
using UnityEngine.UI;

public enum EtapaMision
{
    HablarComerciante,
    RecolectarResiduos,
    VolverComerciante,
    ComprarEscoba,
    TutorialCompletado,
    EliminarOrcosNoche1,
    PrimeraNocheCompletada,
    ComprarPalaDia2,
    PalaCompradaDia2,
    RepararBachesDia2,
    BachesDia2Completados
}

public class SistemaMisiones : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Text textoMision;
    [SerializeField] private InventarioJugador inventario;

    [Header("Configuracion")]
    [SerializeField] private int residuosNecesarios = 5;

    [Header("Reparaciones del Dia 2")]
    [SerializeField] private int bachesNecesarios = 3;

    private int bachesReparados = 0;
    public EtapaMision EtapaActual { get; private set; } =
        EtapaMision.HablarComerciante;

    private int enemigosTotalesNoche;
    private int enemigosDerrotadosNoche;

    private void OnEnable()
    {
        if (inventario != null)
            inventario.InventarioActualizado += RevisarRecoleccion;

        ActualizarHUD();
    }

    private void OnDisable()
    {
        if (inventario != null)
            inventario.InventarioActualizado -= RevisarRecoleccion;
    }

    private int ObtenerResiduos()
    {
        if (inventario == null)
            return 0;

        return inventario.ObtenerCantidad(
            TipoObjetoInventario.Botella
        ) + inventario.ObtenerCantidad(
            TipoObjetoInventario.Lata
        );
    }

    public void FinalizarPrimerDialogo()
    {
        if (EtapaActual != EtapaMision.HablarComerciante)
            return;

        EtapaActual = EtapaMision.RecolectarResiduos;
        RevisarRecoleccion();
    }

    private void RevisarRecoleccion()
    {
        if (EtapaActual == EtapaMision.RecolectarResiduos &&
            ObtenerResiduos() >= residuosNecesarios)
        {
            EtapaActual = EtapaMision.VolverComerciante;
        }

        ActualizarHUD();
    }

    public void FinalizarTutorialComerciante()
    {
        if (EtapaActual != EtapaMision.VolverComerciante)
            return;

        EtapaActual = EtapaMision.ComprarEscoba;
        ActualizarHUD();
    }

    public void RegistrarCompraEscoba()
    {
        if (EtapaActual != EtapaMision.ComprarEscoba)
            return;

        EtapaActual = EtapaMision.TutorialCompletado;
        ActualizarHUD();
    }

    // NUEVO: actualizar la mision durante la primera noche.
    public void ActualizarProgresoOleada(
        int enemigosRestantes,
        int totalEnemigos
    )
    {
        if (EtapaActual != EtapaMision.TutorialCompletado &&
            EtapaActual != EtapaMision.EliminarOrcosNoche1 &&
            EtapaActual != EtapaMision.PrimeraNocheCompletada)
        {
            return;
        }

        enemigosTotalesNoche = Mathf.Max(0, totalEnemigos);

        enemigosDerrotadosNoche = Mathf.Clamp(
            enemigosTotalesNoche - enemigosRestantes,
            0,
            enemigosTotalesNoche
        );

        if (enemigosTotalesNoche > 0 &&
            enemigosRestantes <= 0)
        {
            EtapaActual = EtapaMision.PrimeraNocheCompletada;
        }
        else
        {
            EtapaActual = EtapaMision.EliminarOrcosNoche1;
        }

        ActualizarHUD();
    }

    // NUEVO: pasar al objetivo del Dia 2.
    public void RegistrarAmanecer(int nuevoDia)
    {
        if (nuevoDia == 2 &&
            EtapaActual == EtapaMision.PrimeraNocheCompletada)
        {
            EtapaActual = EtapaMision.ComprarPalaDia2;
            ActualizarHUD();
        }
    }

    private void ActualizarHUD()
    {
        if (textoMision == null)
            return;

        switch (EtapaActual)
        {
            case EtapaMision.HablarComerciante:
                textoMision.text =
                    "Habla con el comerciante de la gasolinera.";
                break;

            case EtapaMision.RecolectarResiduos:
                textoMision.text =
                    "Recoge basura de las calles: " +
                    Mathf.Min(ObtenerResiduos(), residuosNecesarios) +
                    "/" + residuosNecesarios;
                break;

            case EtapaMision.VolverComerciante:
                textoMision.text =
                    "Vuelve con el comerciante de la gasolinera.";
                break;

            case EtapaMision.ComprarEscoba:
                textoMision.text =
                    "Reúne dinero y compra una escoba al comerciante.";
                break;

            case EtapaMision.TutorialCompletado:
                textoMision.text =
                    "Puedes ir a dormir o seguir recogiendo basura.";
                break;

            case EtapaMision.EliminarOrcosNoche1:
                textoMision.text =
                    "NOCHE 1: Derrota a los orcos: " +
                    enemigosDerrotadosNoche +
                    "/" + enemigosTotalesNoche;
                break;

            case EtapaMision.PrimeraNocheCompletada:
                textoMision.text =
                    "¡Sobreviviste a la primera noche! Espera el amanecer.";
                break;

            case EtapaMision.ComprarPalaDia2:
                textoMision.text =
                    "DÍA 2: Reúne dinero y compra la pala al comerciante.";
                break;
            case EtapaMision.PalaCompradaDia2:
                textoMision.text =
                    "DIA 2: Pala comprada. Preparate para la segunda noche.";
                break;
            case EtapaMision.RepararBachesDia2:
                textoMision.text =
                    "DIA 2: Repara los baches: " +
                    bachesReparados + "/" + bachesNecesarios;
                break;

            case EtapaMision.BachesDia2Completados:
                textoMision.text =
                    "¡Reparaciones completas! Preparate para la segunda noche.";
                break;
        }
    }

    public void RegistrarCompraPala()
    {
        if (EtapaActual != EtapaMision.ComprarPalaDia2)
            return;

        // Al comprar la pala, comienza la mision de reparacion.
        bachesReparados = 0;
        EtapaActual = EtapaMision.RepararBachesDia2;

        ActualizarHUD();

        Debug.Log(
            "Nueva mision: reparar " +
            bachesNecesarios + " baches."
        );
    }

    public void RegistrarBacheReparado()
    {
        if (EtapaActual != EtapaMision.RepararBachesDia2)
            return;

        bachesReparados = Mathf.Min(
            bachesReparados + 1,
            bachesNecesarios
        );

        if (bachesReparados >= bachesNecesarios)
        {
            EtapaActual = EtapaMision.BachesDia2Completados;
        }

        ActualizarHUD();
    }


}
