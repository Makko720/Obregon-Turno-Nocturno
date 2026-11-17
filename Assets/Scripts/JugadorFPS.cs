
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class JugadorFPS : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadCaminar = 4f;
    public float velocidadCorrer = 7f;

    [Header("Camara")]
    public Transform camaraJugador;
    public float sensibilidadMouse = 0.12f;

    private CharacterController controlador;
    private float rotacionVertical = 0f;

    void Start()
    {
        controlador = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (MenuPausa.EstaPausado) return;
        Keyboard teclado = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (teclado == null) return;

        // Movimiento WASD
        Vector2 entrada = Vector2.zero;

        if (teclado.wKey.isPressed) entrada.y += 1;
        if (teclado.sKey.isPressed) entrada.y -= 1;
        if (teclado.dKey.isPressed) entrada.x += 1;
        if (teclado.aKey.isPressed) entrada.x -= 1;

        entrada = Vector2.ClampMagnitude(entrada, 1f);

        Vector3 direccion =
            transform.right * entrada.x +
            transform.forward * entrada.y;

        float velocidad = teclado.leftShiftKey.isPressed
            ? velocidadCorrer
            : velocidadCaminar;

        controlador.SimpleMove(direccion * velocidad);

        // Movimiento de la camara
        if (mouse != null &&
            Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 movimientoMouse =
                mouse.delta.ReadValue() * sensibilidadMouse;

            transform.Rotate(Vector3.up * movimientoMouse.x);

            rotacionVertical -= movimientoMouse.y;
            rotacionVertical = Mathf.Clamp(
                rotacionVertical, -85f, 85f
            );

            camaraJugador.localRotation =
                Quaternion.Euler(rotacionVertical, 0f, 0f);
        }
    }
}
