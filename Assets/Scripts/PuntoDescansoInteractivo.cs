using UnityEngine;

public class PuntoDescansoInteractivo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SistemaMisiones sistemaMisiones;
    [SerializeField] private SistemaDiaNoche sistemaDiaNoche;

    public bool PuedeDormir()
    {
        if (sistemaMisiones == null || sistemaDiaNoche == null)
            return false;

        if (sistemaDiaNoche.EsDeNoche)
            return false;

        int diaActual = sistemaDiaNoche.DiaActual;

        if (diaActual == 1)
            return sistemaMisiones.EtapaActual == EtapaMision.TutorialCompletado;

        if (diaActual == 2)
            return sistemaMisiones.EtapaActual == EtapaMision.BachesDia2Completados;

        if (diaActual == 3)
            return sistemaMisiones.EtapaActual == EtapaMision.PortalActivadoDia3;

        return false;
    }

    public string ObtenerTextoInteraccion()
    {
        if (sistemaDiaNoche == null || sistemaMisiones == null)
            return "";

        if (sistemaDiaNoche.EsDeNoche)
            return "";

        if (PuedeDormir())
            return "[E] Ir a dormir";

        if (sistemaDiaNoche.DiaActual == 1)
            return "Necesitas una escoba";

        if (sistemaDiaNoche.DiaActual == 2)
            return "Completa las reparaciones del día";

        if (sistemaDiaNoche.DiaActual == 3)
            return "Activa primero el Portal de Basura";

        return "Aún no puedes descansar";
    }

    public void IntentarDormir()
    {
        if (sistemaDiaNoche == null || sistemaMisiones == null)
            return;

        if (sistemaDiaNoche.EsDeNoche || !PuedeDormir())
            return;

        Debug.Log("El jugador se prepara para dormir. Dia " + sistemaDiaNoche.DiaActual);
        sistemaDiaNoche.IniciarNoche();
    }
}