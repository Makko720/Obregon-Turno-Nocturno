
using System.Collections.Generic;
using UnityEngine;

public class RegeneradorBasura : MonoBehaviour
{
    [Header("Residuos registrados")]

    // Botellas y latas recolectables.
    private readonly List<BasuraRecolectable> residuos =
        new List<BasuraRecolectable>();

    private readonly List<bool> estadosResiduos =
        new List<bool>();

    // Montones que se limpian con escoba.
    private readonly List<BasuraBarrible> montones =
        new List<BasuraBarrible>();

    private readonly List<bool> estadosMontones =
        new List<bool>();

    private void Awake()
    {
        // Registrar botellas y latas de los parches.
        BasuraRecolectable[] encontrados =
            GetComponentsInChildren<BasuraRecolectable>(true);

        foreach (BasuraRecolectable residuo in encontrados)
        {
            residuos.Add(residuo);
            estadosResiduos.Add(
                residuo.gameObject.activeSelf
            );
        }

        // Registrar los montones barribles.
        BasuraBarrible[] montonesEncontrados =
            GetComponentsInChildren<BasuraBarrible>(true);

        foreach (BasuraBarrible monton in montonesEncontrados)
        {
            montones.Add(monton);
            estadosMontones.Add(
                monton.gameObject.activeSelf
            );
        }

        Debug.Log(
            "Regenerador preparado. " +
            "Botellas y latas: " + residuos.Count +
            " | Montones barribles: " + montones.Count
        );
    }

    public void RegenerarBasura()
    {
        int residuosRegenerados = 0;
        int montonesRegenerados = 0;

        // Restaurar botellas y latas.
        for (int i = 0; i < residuos.Count; i++)
        {
            if (residuos[i] == null)
                continue;

            GameObject objeto = residuos[i].gameObject;

            if (estadosResiduos[i] && !objeto.activeSelf)
                residuosRegenerados++;

            objeto.SetActive(estadosResiduos[i]);
        }

        // Restaurar montones barribles.
        for (int i = 0; i < montones.Count; i++)
        {
            if (montones[i] == null)
                continue;

            GameObject objeto = montones[i].gameObject;

            if (estadosMontones[i] && !objeto.activeSelf)
                montonesRegenerados++;

            objeto.SetActive(estadosMontones[i]);
        }

        Debug.Log(
            "Basura regenerada. " +
            "Residuos: " + residuosRegenerados +
            " | Montones: " + montonesRegenerados
        );
    }

    [ContextMenu("Prueba - Regenerar basura")]
    private void ProbarRegeneracion()
    {
        RegenerarBasura();
    }
}
