
using UnityEngine;

public class BasuraRecolectable : MonoBehaviour
{
    [Header("Objeto recolectable")]
    [SerializeField]
    private TipoObjetoInventario tipoObjeto =
        TipoObjetoInventario.Botella;

    [SerializeField, Min(1)]
    private int cantidad = 1;

    public void Recolectar(InventarioJugador inventario)
    {
        if (inventario == null)
        {
            Debug.LogWarning(
                "No se encontro el inventario del jugador."
            );
            return;
        }

        // Intentar guardar el objeto en la mochila.
        bool guardado = inventario.AgregarObjeto(
            tipoObjeto,
            cantidad
        );

        // Si la mochila esta llena, no recogerlo.
        if (!guardado)
            return;

        // Comprobar si pertenece a un parche regenerable.
        RegeneradorBasura regenerador =
            GetComponentInParent<RegeneradorBasura>();

        if (regenerador != null)
        {
            // Ocultarlo para recuperarlo al amanecer.
            gameObject.SetActive(false);
        }
        else
        {
            // Otros residuos mantienen su comportamiento.
            Destroy(gameObject);
        }
    }
}
