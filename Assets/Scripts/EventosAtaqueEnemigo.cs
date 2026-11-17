
using UnityEngine;

public class EventosAtaqueEnemigo : MonoBehaviour
{
    private AtaqueEnemigo ataqueEnemigo;

    private void Awake()
    {
        // Buscar el sistema de ataque en el objeto padre.
        ataqueEnemigo = GetComponentInParent<AtaqueEnemigo>();

        if (ataqueEnemigo == null)
        {
            Debug.LogWarning(
                "No se encontro AtaqueEnemigo en los padres de " +
                gameObject.name
            );
        }
    }

    // Este metodo es llamado por los Animation Events.
    public void ImpactoEspada(int danio)
    {
        if (ataqueEnemigo == null)
            return;

        // Comunicar el impacto al sistema de combate.
        ataqueEnemigo.AplicarImpacto(danio);
    }
}
