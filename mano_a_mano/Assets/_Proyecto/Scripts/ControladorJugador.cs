using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorJugador : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 15f;
    public float fuerzaDash = 20f;
    private Vector3 direccionMovimientoActual;

    [Header("Configuración de Rotación (Mouse)")]
    public float sensibilidadMouse = 0.15f;

    [Header("Configuración de Caída y Respawn")]
    public float alturaLimiteCaida = -10f; // Si baja de esto, muere
    private Vector3 posicionInicial;       // Guardamos dónde empezó el nivel

    private Rigidbody rb;
    private Vector2 entradaDireccion;
    private bool quiereDash;
    private float rotacionX = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        rotacionX = transform.eulerAngles.y;

        // Guardamos la posición exacta donde pusiste el cubo en la escena al arrancar
        posicionInicial = transform.position;
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
            transform.rotation = Quaternion.Euler(0f, rotacionX, 0f);
        }

        // 4. CHEQUEAR CAÍDA AL VACÍO
        if (transform.position.y < alturaLimiteCaida)
        {
            ComprobarRespawn();
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
            // Calculamos la dirección basándonos en la orientación del cubo
            direccionMovimientoActual = (transform.forward * entradaDireccion.y + transform.right * entradaDireccion.x).normalized;

            rb.AddForce(direccionMovimientoActual * velocidad, ForceMode.Force);
        }
        else
        {
            // Si no nos movemos, la dirección actual vuelve a ser cero
            direccionMovimientoActual = Vector3.zero;

            // Frenado seco en horizontal
            Vector3 velocidadActual = rb.linearVelocity;
            velocidadActual.x = 0f;
            velocidadActual.z = 0f;
            rb.linearVelocity = velocidadActual;
        }
    }

    void EjecutarDash()
    {
        // ¡EL AJUSTE CLAVE!: Si nos estamos moviendo, el dash va hacia esa dirección. 
        // Si estamos quietos, por defecto sale hacia adelante (transform.forward).
        Vector3 direccionDash = direccionMovimientoActual != Vector3.zero ? direccionMovimientoActual : transform.forward;

        Vector3 impulso = direccionDash * fuerzaDash;
        rb.AddForce(impulso, ForceMode.VelocityChange);

        quiereDash = false;
    }

    void ComprobarRespawn()
    {
        // EL TRUCO FÍSICO: Para teletransportar un Rigidbody sin que tire tirones o conserve inercia
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Lo movemos a la posición inicial
        transform.position = posicionInicial;
    }
}