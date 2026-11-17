
using UnityEngine;
using UnityEngine.UI;

public class ComercianteReciclaje : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private SistemaDinero sistemaDinero;

    [Header("Interfaz de venta")]
    [SerializeField] private Text textoCantidadBotellas;
    [SerializeField] private Text textoCantidadLatas;
    [SerializeField] private Text textoCantidadBolsas;

    [SerializeField] private Button buttonVenderBotella;
    [SerializeField] private Button buttonVenderLata;
    [SerializeField] private Button buttonVenderBolsa;
    [SerializeField] private Text textoCantidadEscombros;
    [SerializeField] private Button buttonVenderEscombros;

    [Header("Precios de compra")]
    [SerializeField] private int precioBotella = 8;
    [SerializeField] private int precioLata = 5;
    [SerializeField] private int precioBolsa = 15;
    [SerializeField] private int precioEscombro = 80;

    private void Awake()
    {
        if (buttonVenderBotella != null)
            buttonVenderBotella.onClick.AddListener(VenderUnaBotella);

        if (buttonVenderLata != null)
            buttonVenderLata.onClick.AddListener(VenderUnaLata);

        if (buttonVenderBolsa != null)
            buttonVenderBolsa.onClick.AddListener(VenderUnaBolsa);
        if (buttonVenderEscombros != null)
        {
            buttonVenderEscombros.onClick.AddListener(
                VenderUnEscombro
            );
        }
    }

    private void OnEnable()
    {
        if (inventario != null)
            inventario.InventarioActualizado += ActualizarInterfazVenta;

        ActualizarInterfazVenta();
    }

    private void OnDisable()
    {
        if (inventario != null)
            inventario.InventarioActualizado -= ActualizarInterfazVenta;
    }

    public void ActualizarInterfazVenta()
    {
        int botellas = 0;
        int latas = 0;
        int bolsas = 0;
        int escombros = 0;

        if (inventario != null)
        {
            botellas = inventario.ObtenerCantidad(
                TipoObjetoInventario.Botella
            );

            latas = inventario.ObtenerCantidad(
                TipoObjetoInventario.Lata
            );

            bolsas = inventario.ObtenerCantidad(
                TipoObjetoInventario.ResiduosBarrido
            );
            escombros = inventario.ObtenerCantidad(
                TipoObjetoInventario.Escombros
            );
        }

        if (textoCantidadBotellas != null)
            textoCantidadBotellas.text =
                "DISPONIBLES: " + botellas;

        if (textoCantidadLatas != null)
            textoCantidadLatas.text =
                "DISPONIBLES: " + latas;

        if (textoCantidadBolsas != null)
            textoCantidadBolsas.text =
                "DISPONIBLES: " + bolsas;

        if (buttonVenderBotella != null)
            buttonVenderBotella.interactable = botellas > 0;

        if (buttonVenderLata != null)
            buttonVenderLata.interactable = latas > 0;

        if (buttonVenderBolsa != null)
            buttonVenderBolsa.interactable = bolsas > 0;
        if (textoCantidadEscombros != null)
        {
            textoCantidadEscombros.text =
                "DISPONIBLES: " + escombros;
        }

        if (buttonVenderEscombros != null)
        {
            buttonVenderEscombros.interactable =
                escombros > 0;
        }
    }

    public void VenderUnaBotella()
    {
        VenderUnidad(
            TipoObjetoInventario.Botella,
            precioBotella,
            "botella"
        );
    }

    public void VenderUnaLata()
    {
        VenderUnidad(
            TipoObjetoInventario.Lata,
            precioLata,
            "lata"
        );
    }

    public void VenderUnaBolsa()
    {
        VenderUnidad(
            TipoObjetoInventario.ResiduosBarrido,
            precioBolsa,
            "bolsa de desperdicios"
        );
    }

    public void VenderUnEscombro()
    {
        VenderUnidad(
            TipoObjetoInventario.Escombros,
            precioEscombro,
            "escombro de calle"
        );
    }

    private void VenderUnidad(
        TipoObjetoInventario tipo,
        int precio,
        string nombre
    )
    {
        if (inventario == null || sistemaDinero == null)
        {
            Debug.LogWarning(
                "Faltan referencias del comerciante."
            );
            return;
        }

        if (inventario.ObtenerCantidad(tipo) <= 0)
        {
            Debug.Log(
                "No tienes " + nombre + " para vender."
            );
            return;
        }

        // Cobrar solo cuando el objeto se retiro.
        if (!inventario.QuitarObjeto(tipo, 1))
            return;

        sistemaDinero.GanarDinero(precio);

        Debug.Log(
            "Vendiste 1 " + nombre + " por $" + precio
        );

        ActualizarInterfazVenta();
    }

    // Vender todos los residuos disponibles.
    [ContextMenu("Prueba - Vender residuos")]
    public void VenderResiduos()
    {
        if (inventario == null || sistemaDinero == null)
        {
            Debug.LogWarning(
                "Faltan referencias del comerciante."
            );
            return;
        }

        int botellas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Botella
        );

        int latas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Lata
        );

        int bolsas = inventario.ObtenerCantidad(
            TipoObjetoInventario.ResiduosBarrido
        );
        int escombros = inventario.ObtenerCantidad(
            TipoObjetoInventario.Escombros
        );

        int totalVenta =
            (botellas * precioBotella) +
            (latas * precioLata) +
            (bolsas * precioBolsa) +
            (escombros * precioEscombro);

        if (totalVenta <= 0)
        {
            Debug.Log("No tienes residuos para vender.");
            return;
        }

        if (botellas > 0)
            inventario.QuitarObjeto(
                TipoObjetoInventario.Botella,
                botellas
            );

        if (latas > 0)
            inventario.QuitarObjeto(
                TipoObjetoInventario.Lata,
                latas
            );

        if (bolsas > 0)
            inventario.QuitarObjeto(
                TipoObjetoInventario.ResiduosBarrido,
                bolsas
            );

        if (escombros > 0)
        {
            inventario.QuitarObjeto(
                TipoObjetoInventario.Escombros,
                escombros
            );
        }

        sistemaDinero.GanarDinero(totalVenta);

        Debug.Log(
            "Venta realizada. Ganaste $" + totalVenta
        );

        ActualizarInterfazVenta();
    }

    public int CalcularTotalVenta()
    {
        if (inventario == null)
            return 0;

        int botellas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Botella
        );

        int latas = inventario.ObtenerCantidad(
            TipoObjetoInventario.Lata
        );

        int bolsas = inventario.ObtenerCantidad(
            TipoObjetoInventario.ResiduosBarrido
        );

        int escombros = inventario.ObtenerCantidad(
            TipoObjetoInventario.Escombros
        );

        return
            (botellas * precioBotella) +
            (latas * precioLata) +
            (bolsas * precioBolsa) +
            (escombros * precioEscombro);
    }
}
