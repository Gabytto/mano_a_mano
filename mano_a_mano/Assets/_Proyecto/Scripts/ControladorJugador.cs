using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 15f;
    public float fuerzaDash = 20f;

    [Header("Configuración de Rotación (Mouse)")]
    public float sensibilidadMouse = 0.15f;

    private Rigidbody rb;
    private Vector2 entradaDireccion;
    private bool quiereDash;
    private float rotacionX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Bloqueamos el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializamos la rotación con la que ya tenga el objeto
        rotacionX = transform.eulerAngles.y;
    }

    void Update()
    {
        // 1. Leer movimiento WASD / Flechas
        Vector2 teclado = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) teclado.y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) teclado.y = -1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) teclado.x = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) teclado.x = 1f;
        }
        entradaDireccion = teclado.normalized;

        // 2. Detectar el Dash (Espacio)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && entradaDireccion != Vector2.zero)
        {
            quiereDash = true;
        }

        // 3. Rotar el CUBO con el movimiento horizontal del mouse
        if (Mouse.current != null)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            rotacionX += mouseX * sensibilidadMouse;

            // Aplicamos la rotación directamente al transform del jugador
            transform.rotation = Quaternion.Euler(0f, rotacionX, 0f);
        }
    }

    void FixedUpdate()
    {
        MoverJugador();

        if (quiereDash)
        {
            EjecutarDash();
        }
    }

    void MoverJugador()
    {
        if (entradaDireccion.magnitude > 0.1f)
        {
            // Ahora el movimiento es relativo hacia donde APUNTA EL CUBO (su frente local)
            Vector3 direccionFinal = (transform.forward * entradaDireccion.y + transform.right * entradaDireccion.x).normalized;

            rb.AddForce(direccionFinal * velocidad, ForceMode.Force);
        }
    }

    void EjecutarDash()
    {
        // El dash sale disparado hacia el frente actual del jugador
        Vector3 impulso = transform.forward * fuerzaDash;
        rb.AddForce(impulso, ForceMode.VelocityChange);
        quiereDash = false;
    }
}