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
        public Text textoNivel;
        public Image[] segmentosProgreso;
    }

    [Header("Referencias de sistemas")]
    [SerializeField] private SistemaDinero sistemaDinero;
    [SerializeField] private InventarioJugador inventarioJugador;
    [SerializeField] private JugadorFPS jugadorFPS;
    [SerializeField] private VidaJugador vidaJugador;
    [SerializeField] private SistemaDiaNoche sistemaDiaNoche;

    [Header("1. Mochila reforzada (3 niveles: 5 -> 8 -> 11 -> 15)")]
    [SerializeField] private TarjetaMejoraUI uiMochila;
    [SerializeField] private int[] preciosMochila = { 80, 160, 280 };
    private readonly int[] capacidadesMochila = { 5, 8, 11, 15 };
    private int nivelMochila = 0;

    [Header("2. Tenis de corredor (2 niveles: 1.0x -> 1.1x -> 1.2x)")]
    [SerializeField] private TarjetaMejoraUI uiTenis;
    [SerializeField] private int[] preciosTenis = { 100, 220 };
    private readonly float[] multiplicadoresVelocidad = { 1f, 1.30f, 1.60f };
    private int nivelTenis = 0;

    [Header("3. Brazalete de vitalidad (5 niveles: 100 -> 175 HP)")]
    [SerializeField] private TarjetaMejoraUI uiBrazalete;
    [SerializeField] private int[] preciosBrazalete = { 75, 140, 220, 320, 450 };
    private readonly int[] vidasMaximas = { 100, 125, 150, 175, 200, 225 };
    private int nivelBrazalete = 0;

    [Header("4. Collar de estamina (3 niveles: 100 -> 160)")]
    [SerializeField] private TarjetaMejoraUI uiCollar;
    [SerializeField] private int[] preciosCollar = { 90, 180, 300 };
    private readonly float[] estaminasMaximas = { 100f, 135f, 170f, 205f };
    private int nivelCollar = 0;

    [Header("Colores de segmentos")]
    [SerializeField] private Color colorSegmentoActivo = new Color32(197, 231, 182, 255);
    [SerializeField] private Color colorSegmentoInactivo = new Color32(45, 62, 71, 255);

    private int ultimoDineroRevisado = -1;
    private int ultimoDiaRevisado = -1;

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

        int diaActual = sistemaDiaNoche != null ? sistemaDiaNoche.DiaActual : 1;

        if (ultimoDineroRevisado != sistemaDinero.DineroActual || ultimoDiaRevisado != diaActual)
        {
            ultimoDineroRevisado = sistemaDinero.DineroActual;
            ultimoDiaRevisado = diaActual;
            ActualizarTodaLaInterfaz();
        }
    }

    private bool EstaBloqueadoPorDia1(int nivelActual)
    {
        return sistemaDiaNoche != null && sistemaDiaNoche.DiaActual == 1 && nivelActual >= 1;
    }

    public void ComprarMejoraMochila()
    {
        int nivelMaximo = preciosMochila.Length;
        if (nivelMochila >= nivelMaximo || EstaBloqueadoPorDia1(nivelMochila) ||
            sistemaDinero == null || inventarioJugador == null)
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
        if (nivelTenis >= nivelMaximo || EstaBloqueadoPorDia1(nivelTenis) ||
            sistemaDinero == null || jugadorFPS == null)
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
        if (nivelBrazalete >= nivelMaximo || EstaBloqueadoPorDia1(nivelBrazalete) ||
            sistemaDinero == null || vidaJugador == null)
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
        if (nivelCollar >= nivelMaximo || EstaBloqueadoPorDia1(nivelCollar) ||
            sistemaDinero == null || jugadorFPS == null)
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
        bool bloqueadoDia1 = !estaAlMaximo && EstaBloqueadoPorDia1(nivelActual);
        int costoSiguiente = estaAlMaximo ? 0 : precios[nivelActual];
        bool tieneDinero = sistemaDinero != null && sistemaDinero.DineroActual >= costoSiguiente;

        if (tarjeta.textoNivel != null)
        {
            tarjeta.textoNivel.text = nivelActual + "/" + nivelMaximo;
        }

        if (tarjeta.textoPrecio != null)
        {
            if (estaAlMaximo)
                tarjeta.textoPrecio.text = "NIVEL MÁXIMO";
            else if (bloqueadoDia1)
                tarjeta.textoPrecio.text = "DISP. EN DÍA 2";
            else
                tarjeta.textoPrecio.text = "PRECIO: $" + costoSiguiente;
        }

        if (tarjeta.textoBoton != null)
        {
            if (estaAlMaximo)
                tarjeta.textoBoton.text = "MÁXIMO";
            else if (bloqueadoDia1)
                tarjeta.textoBoton.text = "DÍA 2";
            else
                tarjeta.textoBoton.text = "MEJORAR";
        }

        if (tarjeta.botonMejorar != null)
        {
            tarjeta.botonMejorar.interactable = !estaAlMaximo && !bloqueadoDia1 && tieneDinero;
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