using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JugadorFPS : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 4f;
    public float velocidadCorrer = 7f;

    [Header("Salto y Gravedad")]
    [SerializeField] private float alturaSalto = 1.25f;
    [SerializeField] private float gravedad = -22f;

    [Header("Estamina")]
    [SerializeField] private float estaminaMaxima = 100f;
    [SerializeField] private float consumoCorrerPorSegundo = 22f;
    [SerializeField] private float costoEstaminaSalto = 8f;
    [SerializeField] private float regeneracionPorSegundo = 16f;
    [SerializeField] private float retrasoRegeneracion = 0.8f;
    [SerializeField] private float estaminaMinimaParaCorrer = 15f;

    [Header("Camara y Referencias")]
    public Transform camaraJugador;
    public float sensibilidadMouse = 0.12f;
    [SerializeField] private VidaJugador vidaJugador;

    // Propiedades publicas para la interfaz (HUD) y mejoras
    public float EstaminaActual { get; private set; }
    public float EstaminaMaxima => estaminaMaxima;
    public bool Agotado { get; private set; }

    private CharacterController controlador;
    private float rotacionVertical = 0f;
    private float velocidadVertical = 0f;
    private float multiplicadorVelocidad = 1f;
    private float tiempoUltimaAccionEstamina = -10f;

    private void Awake()
    {
        if (vidaJugador == null)
            vidaJugador = GetComponent<VidaJugador>();

        EstaminaActual = estaminaMaxima;
    }

    private void Start()
    {
        controlador = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Bloquear movimiento y consumo de estamina en pausa, dialogos o derrota
        if (MenuPausa.EstaPausado || SistemaDialogos.DialogoActivo)
            return;

        if (vidaJugador != null && vidaJugador.EstaDerrotado)
            return;

        Keyboard teclado = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (teclado == null)
            return;

        bool enSuelo = controlador.isGrounded;

        if (enSuelo && velocidadVertical < 0f)
        {
            // Mantiene al personaje pegado al suelo al bajar banquetas
            velocidadVertical = -2f;
        }

        // Movimiento WASD
        Vector2 entrada = Vector2.zero;
        if (teclado.wKey.isPressed) entrada.y += 1f;
        if (teclado.sKey.isPressed) entrada.y -= 1f;
        if (teclado.dKey.isPressed) entrada.x += 1f;
        if (teclado.aKey.isPressed) entrada.x -= 1f;

        entrada = Vector2.ClampMagnitude(entrada, 1f);
        bool intentandoMoverse = entrada.sqrMagnitude > 0.01f;

        // Carrera y gestion de estamina
        bool quiereCorrer = teclado.leftShiftKey.isPressed && intentandoMoverse;
        bool puedeCorrer = !Agotado && EstaminaActual > 0.1f;
        bool estaCorriendo = quiereCorrer && puedeCorrer;

        if (estaCorriendo)
        {
            ConsumirEstamina(consumoCorrerPorSegundo * Time.deltaTime, true);
        }
        else
        {
            RegenerarEstamina(Time.deltaTime);
        }

        float velocidadBase = estaCorriendo ? velocidadCorrer : velocidadCaminar;
        float velocidadFinal = velocidadBase * multiplicadorVelocidad;

        Vector3 movimientoHorizontal =
            (transform.right * entrada.x + transform.forward * entrada.y) * velocidadFinal;

        // Salto con barra espaciadora (solo cuando esta en el suelo)
        if (enSuelo && teclado.spaceKey.wasPressedThisFrame)
        {
            velocidadVertical = Mathf.Sqrt(alturaSalto * -2f * gravedad);
            ConsumirEstamina(costoEstaminaSalto, false);
        }

        // Aplicar gravedad y mover
        velocidadVertical += gravedad * Time.deltaTime;
        Vector3 movimientoTotal = movimientoHorizontal + Vector3.up * velocidadVertical;
        controlador.Move(movimientoTotal * Time.deltaTime);

        // Movimiento de la camara
        if (mouse != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 movimientoMouse = mouse.delta.ReadValue() * sensibilidadMouse;

            transform.Rotate(Vector3.up * movimientoMouse.x);

            rotacionVertical -= movimientoMouse.y;
            rotacionVertical = Mathf.Clamp(rotacionVertical, -85f, 85f);

            if (camaraJugador != null)
            {
                camaraJugador.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
            }
        }
    }

    private void ConsumirEstamina(float cantidad, bool puedeAgotarAlCorrer)
    {
        if (cantidad <= 0f)
            return;

        EstaminaActual = Mathf.Max(0f, EstaminaActual - cantidad);
        tiempoUltimaAccionEstamina = Time.time;

        if (puedeAgotarAlCorrer && EstaminaActual <= 0f)
        {
            Agotado = true;
        }
    }

    private void RegenerarEstamina(float deltaTime)
    {
        if (Time.time - tiempoUltimaAccionEstamina < retrasoRegeneracion)
            return;

        if (EstaminaActual < estaminaMaxima)
        {
            EstaminaActual = Mathf.Min(
                estaminaMaxima,
                EstaminaActual + regeneracionPorSegundo * deltaTime
            );

            if (Agotado && EstaminaActual >= estaminaMinimaParaCorrer)
            {
                Agotado = false;
            }
        }
    }

    // Mejora 1 de movimiento: Tenis de corredor (Niveles 1 y 2)
    public void EstablecerMultiplicadorVelocidad(float nuevoMultiplicador)
    {
        multiplicadorVelocidad = Mathf.Max(0.1f, nuevoMultiplicador);
    }

    // Mejora 2 de movimiento: Collar de estamina (Niveles 1, 2 y 3)
    public void EstablecerEstaminaMaxima(float nuevaMaxima, bool rellenar = true)
    {
        estaminaMaxima = Mathf.Max(1f, nuevaMaxima);

        if (rellenar)
            EstaminaActual = estaminaMaxima;
        else
            EstaminaActual = Mathf.Min(EstaminaActual, estaminaMaxima);

        Agotado = false;
    }
}