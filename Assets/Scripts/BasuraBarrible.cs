
using UnityEngine;

public class BasuraBarrible : MonoBehaviour
{
    [Header("Configuracion de limpieza")]
    [SerializeField, Min(1)]
    private int barridosNecesarios = 2;

    private int barridosActuales = 0;

    private void OnEnable()
    {
        barridosActuales = 0;
    }

    public void RecibirBarrido(InventarioJugador inventario)
    {
        if (inventario == null)
        {
            Debug.LogWarning(
                "No se encontro el inventario del jugador."
            );
            return;
        }

        int siguienteBarrido = barridosActuales + 1;

        // Todavia faltan barridos para limpiar.
        if (siguienteBarrido < barridosNecesarios)
        {
            barridosActuales = siguienteBarrido;

            Debug.Log(
                gameObject.name +
                " | Barridos: " +
                barridosActuales + "/" + barridosNecesarios
            );

            return;
        }

        // El ultimo barrido intenta entregar la bolsa.
        bool guardado = inventario.AgregarObjeto(
            TipoObjetoInventario.ResiduosBarrido,
            1
        );

        // Si no hay espacio, no desaparece la basura.
        if (!guardado)
        {
            Debug.Log(
                "Mochila llena. No puedes terminar de limpiar."
            );
            return;
        }

        Debug.Log(
            "¡Basura limpiada! Obtuviste una bolsa de desperdicios."
        );

        // Ocultar el monton para poder regenerarlo despues.
        gameObject.SetActive(false);
    }
}
