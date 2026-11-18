using System;
using UnityEngine;
using UnityEngine.UI;

public class SistemaMejoras : MonoBehaviour
{
    [Serializable]
    public class TarjetaMejoraUI
    {
        public Button botonMejorar;
        public Text textoPrecio;
        public Text textoBoton;
        public Image[] segmentosProgreso;
    }

    [Header("Referencias de sistemas")]
    [SerializeField] private SistemaDinero sistemaDinero;
    [SerializeField] private InventarioJugador inventarioJugador;
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private VidaJugador vidaJugador;

    [Header("1. Mochila reforzada (3 niveles: 5 -> 8 -> 11 -> 15)")]
    [SerializeField] private TarjetaMejoraUI uiMochila;
    [SerializeField] private int[] preciosMochila = { 80, 160, 280 };
    private readonly int[] capacidadesMochila = { 5, 8, 11, 15 };
    private int nivelMochila = 0;

    [Header("2. Tenis de corredor (2 niveles: 1.0x -> 1.1x -> 1.2x)")]
    [SerializeField] private TarjetaMejoraUI uiTenis;
    [SerializeField] private int[] preciosTenis = { 100, 220 };
    private readonly float[] multiplicadoresVelocidad = { 1f, 1.10f, 1.20f };
    private int nivelTenis = 0;

    [Header("3. Brazalete de vitalidad (5 niveles: 100 -> 175 HP)")]
    [SerializeField] private TarjetaMejoraUI uiBrazalete;
    [SerializeField] private int[] preciosBrazalete = { 75, 140, 220, 320, 450 };
    private readonly int[] vidasMaximas = { 100, 115, 130, 145, 160, 175 };
    private int nivelBrazalete = 0;

    [Header("4. Collar de estamina (3 niveles: 100 -> 160)")]
    [SerializeField] private TarjetaMejoraUI uiCollar;
    [SerializeField] private int[] preciosCollar = { 90, 180, 300 };
    private readonly float[] estaminasMaximas = { 100f, 120f, 140f, 160f };
    private int nivelCollar = 0;

    [Header("Colores de segmentos")]
    [SerializeField] private Color colorSegmentoActivo = new Color32(197, 231, 182, 255);
    [SerializeField] private Color colorSegmentoInactivo = new Color32(45, 62, 71, 255);

    private int ultimoDineroRevisado = -1;

    private void Awake()
    {
        if (uiMochila.botonMejorar != null)
            uiMochila.botonMejorar.onClick.AddListener(ComprarMejoraMochila);

        if (uiTenis.botonMejorar != null)
            uiTenis.botonMejorar.onClick.AddListener(ComprarMejoraTenis);

        if (uiBrazalete.botonMejorar != null)
            uiBrazalete.botonMejorar.onClick.AddListener(ComprarMejoraBrazalete);

        if (uiCollar.botonMejorar != null)
            uiCollar.botonMejorar.onClick.AddListener(ComprarMejoraCollar);
    }

    private void OnEnable()
    {
        ActualizarTodaLaInterfaz();
    }

    private void Update()
    {
        if (sistemaDinero == null)
            return;

        if (ultimoDineroRevisado != sistemaDinero.DineroActual)
        {
            ultimoDineroRevisado = sistemaDinero.DineroActual;
            ActualizarTodaLaInterfaz();
        }
    }

    public void ComprarMejoraMochila()
    {
        int nivelMaximo = preciosMochila.Length;
        if (nivelMochila >= nivelMaximo || sistemaDinero == null || inventarioJugador == null)
            return;

        int costo = preciosMochila[nivelMochila];
        if (!sistemaDinero.GastarDinero(costo))
            return;

        nivelMochila++;
        inventarioJugador.EstablecerCapacidadMaxima(capacidadesMochila[nivelMochila]);
        ActualizarTodaLaInterfaz();
    }

    public void ComprarMejoraTenis()
    {
        int nivelMaximo = preciosTenis.Length;
        if (nivelTenis >= nivelMaximo || sistemaDinero == null || jugadorFPS == null)
            return;

        int costo = preciosTenis[nivelTenis];
        if (!sistemaDinero.GastarDinero(costo))
            return;

        nivelTenis++;
        jugadorFPS.EstablecerMultiplicadorVelocidad(multiplicadoresVelocidad[nivelTenis]);
        ActualizarTodaLaInterfaz();
    }

    public void ComprarMejoraBrazalete()
    {
        int nivelMaximo = preciosBrazalete.Length;
        if (nivelBrazalete >= nivelMaximo || sistemaDinero == null || vidaJugador == null)
            return;

        int costo = preciosBrazalete[nivelBrazalete];
        if (!sistemaDinero.GastarDinero(costo))
            return;

        nivelBrazalete++;
        vidaJugador.EstablecerVidaMaxima(vidasMaximas[nivelBrazalete], true);
        ActualizarTodaLaInterfaz();
    }

    public void ComprarMejoraCollar()
    {
        int nivelMaximo = preciosCollar.Length;
        if (nivelCollar >= nivelMaximo || sistemaDinero == null || jugadorFPS == null)
            return;

        int costo = preciosCollar[nivelCollar];
        if (!sistemaDinero.GastarDinero(costo))
            return;

        nivelCollar++;
        jugadorFPS.EstablecerEstaminaMaxima(estaminasMaximas[nivelCollar], true);
        ActualizarTodaLaInterfaz();
    }

    public void ActualizarTodaLaInterfaz()
    {
        ActualizarTarjeta(uiMochila, nivelMochila, preciosMochila);
        ActualizarTarjeta(uiTenis, nivelTenis, preciosTenis);
        ActualizarTarjeta(uiBrazalete, nivelBrazalete, preciosBrazalete);
        ActualizarTarjeta(uiCollar, nivelCollar, preciosCollar);
    }

    private void ActualizarTarjeta(TarjetaMejoraUI tarjeta, int nivelActual, int[] precios)
    {
        if (tarjeta == null)
            return;

        int nivelMaximo = precios.Length;
        bool estaAlMaximo = nivelActual >= nivelMaximo;
        int costoSiguiente = estaAlMaximo ? 0 : precios[nivelActual];
        bool tieneDinero = sistemaDinero != null && sistemaDinero.DineroActual >= costoSiguiente;

        if (tarjeta.textoPrecio != null)
        {
            tarjeta.textoPrecio.text = estaAlMaximo
                ? "NIVEL MÁXIMO"
                : "PRECIO: $" + costoSiguiente;
        }

        if (tarjeta.textoBoton != null)
        {
            tarjeta.textoBoton.text = estaAlMaximo ? "MÁXIMO" : "MEJORAR";
        }

        if (tarjeta.botonMejorar != null)
        {
            tarjeta.botonMejorar.interactable = !estaAlMaximo && tieneDinero;
        }

        if (tarjeta.segmentosProgreso != null)
        {
            for (int i = 0; i < tarjeta.segmentosProgreso.Length; i++)
            {
                if (tarjeta.segmentosProgreso[i] == null)
                    continue;

                tarjeta.segmentosProgreso[i].color = i < nivelActual
                    ? colorSegmentoActivo
                    : colorSegmentoInactivo;
            }
        }
    }
}