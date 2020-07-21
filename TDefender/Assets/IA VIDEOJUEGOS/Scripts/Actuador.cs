using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuador : MonoBehaviour
{

    // Start is called before the first frame update

    [SerializeField]
    GameObject target = null; //Objetivo

    [SerializeField]
    Camera cam = null;

    [SerializeField]
    float maxAceleracion = 0; // Máxima aceleracion
    [SerializeField]
    float maxVelocidad = 0; // Máxima velocidad
    [SerializeField]
    float maxVelocidadAngular = 0; // Máxima velocidad angular
    [SerializeField]
    float maxAceleracionAngular = 0; // Máxima aceleracion angular

    Vector3 velocidad = Vector3.zero; // Velocidad lineal
    public float orientacion = 0;

    bool seleccionado = false; // Para seleccionar unidades y tomar el control manual de ellas
    GameObject esfera; // Esfera de seleccionado

    Animator animator = null; // Posible animator, que tendría un hijo, al que habrá que notificarle si estoy en movimiento o no.
    Vector3 posicionAnterior = Vector3.zero;
    bool acabaDeSeleccionarse = false;

    public float dificultadTerreno = 1.0f;

    private void Start()
    {
        esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Establecemos tamaño
        esfera.SetActive(false);
        esfera.hideFlags = HideFlags.HideInHierarchy;

        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        esfera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        if (seleccionado){ // Se toma el control manual, ya sea para dar una intrucción de target o manejarlo con wasd qe espacio
            // wasd para el movimiento lineal
            Vector3 newDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            // Avanzar de acuerdo a la velocidad establecida
            transform.position += newDirection * maxVelocidad * Time.deltaTime;
            

            // q y e para las rotaciones
            if (Input.GetKey("q"))
            {
                transform.Rotate(Vector3.up * 45 * maxVelocidad * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("e"))
            {
                transform.Rotate(Vector3.up * 45 * -maxVelocidad * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("space"))
            {
                animator.ResetTrigger("Attack1Trigger");
                animator.SetTrigger("Attack1Trigger"); // Ataca el personaje
            }

            if (Input.GetMouseButtonDown(1)) // Si pulsamos click derecho, se toma como nuevo target
            {
                // Tiramos un rayo desde la cámara
                Ray rayo = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // Calculamos con lo que ha colisionado
                if (Physics.Raycast(rayo, out hit))
                {
                    Vector3 pos = hit.point;
                    GameObject ob = hit.collider.gameObject;
                    GameObject pseudoObjetivo = new GameObject();
                    pseudoObjetivo.name = "Nuevo Objetivo";
                    pseudoObjetivo.SetActive(false);
                    pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy;
                    pseudoObjetivo.transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    target = pseudoObjetivo;
                }

                // Una vez dictaminado su nuevo destino se desactiva el control manual

                seleccionado = false;
                esfera.SetActive(false);
            }
            return;
        }


        // Cambiar posicion y rotacion en cada momento
        SteeringAcelerado.MovimientoAcelerado movimientoActual = GetComponent<SteeringAcelerado>().getSteering(target, this.gameObject, maxVelocidad, maxAceleracion, maxVelocidadAngular, maxAceleracionAngular, orientacion, velocidad);

        // Modificación de nuestros parámetros actuales

        if (movimientoActual.steeringLineal == Vector3.zero) //Si la velocidad es cero, bajamos la incercia a la mitad
        {
            velocidad = velocidad - velocidad * Time.deltaTime; // Restamos le velocidad que llevamos
        }
        else
        {
            velocidad += movimientoActual.steeringLineal * Time.deltaTime; // Modificamos la velocidad con los nuevos parámetros
            if (velocidad.magnitude > maxVelocidad)
                velocidad = velocidad.normalized * maxVelocidad; // Lo ponemos como máximo a la velocidad máxima

        }

        orientacion += movimientoActual.steeringAngular * Time.deltaTime; // Modificamos la rotación con los nuevos parámetros
        
        // Modificaciones aplicadas al personaje
        transform.position += velocidad * dificultadTerreno * Time.deltaTime; // Modificamos la posición con la velocidad
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up,orientacion); // Rotamos lo que nos diga el steering angular

    }


    private void Update()
    { 
        // Si tiene un hijo que tiene un animator, será el de la animación de movimiento que queremos activar
        if (animator == null) return; // Si no hay animator no hace nada
        Vector3 diferencia = transform.position - posicionAnterior; 
        posicionAnterior = transform.position;

        if (diferencia.magnitude != 0) { 
            // EL valor 0 ocurre y suele ser por ralentización del pc o por toma manual del personaje
            // normalmente el cambio será de valores muy pequeños, por tanto si es 0 dejamos la animación que hubies
            if (diferencia.magnitude < Time.deltaTime) animator.SetBool("Moving", false);
            else animator.SetBool("Moving", true); // Para no machacar constantemente que nos estamos moviendo
        }

        if (acabaDeSeleccionarse) // El 0 de la diferencia dado por el control manual se tendrá en cuenta a parte
        {
            animator.SetBool("Moving", false);
            acabaDeSeleccionarse = false;
        }






    }

    void OnMouseDown()
    {
        // Seleccionar objeto
        if (seleccionado)
        {
            seleccionado = false;
            esfera.SetActive(false);
            
        }
        else
        {
            seleccionado = true;
            esfera.SetActive(true);
            acabaDeSeleccionarse = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Puente"))
            dificultadTerreno = 0.8f;

        else if (collision.gameObject.CompareTag("Tierra"))
            dificultadTerreno = 0.5f;

        else dificultadTerreno = 1f;
    }



}
