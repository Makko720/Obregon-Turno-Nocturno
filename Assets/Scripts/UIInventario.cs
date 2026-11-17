
using UnityEngine;
using UnityEngine.UI;

public class UIInventario : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InventarioJugador inventario;

    [Header("Capacidad")]
    [SerializeField] private Text textoCapacidad;

    [Header("Botellas")]
    [SerializeField] private Text textoCantidadBotella;
    [SerializeField] private GameObject casillaBotella;

    [Header("Latas")]
    [SerializeField] private Text textoCantidadLata;
    [SerializeField] private GameObject casillaLata;

    [Header("Bolsas de desperdicios")]
    [SerializeField] private Text textoCantidadBolsa;
    [SerializeField] private GameObject casillaBolsaBasura;

    [Header("Escombros")]
    [SerializeField] private Text textoCantidadEscombros;
    [SerializeField] private GameObject casillaEscombros;

    private void OnEnable()
    {
        if (inventario != null)
            inventario.InventarioActualizado += ActualizarInterfaz;

        ActualizarInterfaz();
    }

    private void OnDisable()
    {
        if (inventario != null)
            inventario.InventarioActualizado -= ActualizarInterfaz;
    }

    private void ActualizarInterfaz()
    {
        if (inventario == null)
            return;

        // CAPACIDAD DE LA MOCHILA
        if (textoCapacidad != null)
        {
            textoCapacidad.text =
                inventario.EspaciosOcupados + "/" +
                inventario.CapacidadMaxima;
        }

        // BOTELLAS
        int botellas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Botella
        );

        if (textoCantidadBotella != null)
            textoCantidadBotella.text = "×" + botellas;

        if (casillaBotella != null)
            casillaBotella.SetActive(botellas > 0);

        // LATAS
        int latas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Lata
        );

        if (textoCantidadLata != null)
            textoCantidadLata.text = "×" + latas;

        if (casillaLata != null)
            casillaLata.SetActive(latas > 0);

        // BOLSAS DE DESPERDICIOS
        int bolsas = inventario.ObtenerCantidad(
            TipoObjetoInventario.ResiduosBarrido
        );

        if (textoCantidadBolsa != null)
            textoCantidadBolsa.text = "×" + bolsas;

        if (casillaBolsaBasura != null)
            casillaBolsaBasura.SetActive(bolsas > 0);

        // ESCOMBROS
        int escombros = inventario.ObtenerCantidad(
            TipoObjetoInventario.Escombros
        );

        if (textoCantidadEscombros != null)
            textoCantidadEscombros.text = "×" + escombros;

        if (casillaEscombros != null)
            casillaEscombros.SetActive(escombros > 0);
    }
    
}
