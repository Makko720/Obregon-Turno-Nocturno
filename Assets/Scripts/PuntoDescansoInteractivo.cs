
using UnityEngine;

public class PuntoDescansoInteractivo : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SistemaMisiones sistemaMisiones;
    [SerializeField] private SistemaDiaNoche sistemaDiaNoche;

    // Comprobar si el jugador puede descansar.
    public bool PuedeDormir()
    {
        if (sistemaMisiones == null || sistemaDiaNoche == null)
            return false;

        // No se puede dormir durante la noche.
        if (sistemaDiaNoche.EsDeNoche)
            return false;

        int diaActual = sistemaDiaNoche.DiaActual;

        // DIA 1: necesita haber comprado la escoba.
        if (diaActual == 1)
        {
            return sistemaMisiones.EtapaActual ==
                EtapaMision.TutorialCompletado;
        }

        // DIA 2: necesita haber comprado la pala.
        if (diaActual == 2)
        {
            return sistemaMisiones.EtapaActual ==
                EtapaMision.PalaCompradaDia2;
        }

        // Otros dias se configuraran posteriormente.
        return false;
    }

    // Se llama cuando el jugador pulsa E.
    public void IntentarDormir()
    {
        if (sistemaDiaNoche == null || sistemaMisiones == null)
        {
            Debug.LogWarning(
                "Faltan referencias en PuntoDescansoInteractivo."
            );
            return;
        }

        if (sistemaDiaNoche.EsDeNoche)
        {
            Debug.Log("No puedes dormir durante la noche.");
            return;
        }

        if (!PuedeDormir())
        {
            if (sistemaDiaNoche.DiaActual == 1)
            {
                Debug.Log(
                    "Debes comprar la escoba antes de dormir."
                );
            }
            else if (sistemaDiaNoche.DiaActual == 2)
            {
                Debug.Log(
                    "Debes comprar la pala antes de dormir."
                );
            }
            else
            {
                Debug.Log(
                    "Todavia no puedes descansar en este dia."
                );
            }

            return;
        }

        Debug.Log(
            "El jugador se prepara para dormir. Dia " +
            sistemaDiaNoche.DiaActual
        );

        // Solicitar el inicio de la noche correspondiente.
        sistemaDiaNoche.IniciarNoche();
    }
}
