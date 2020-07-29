using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class movimientoPersonaje : MonoBehaviour
{
    [SerializeField]
    ClaseInput2 input;

    [SerializeField]
    Camera cam;

    [SerializeField]
    GameObject arma;

    [SerializeField]
    GameObject disparo;

    [SerializeField]
    GameObject spawn; //Empty que contenga las coordenadas del spawn

    public Text ammoDisplay;
    public Text goldDisplay;
    static public int oro;
    public Slider barraVida;

    [SerializeField]
    float v, vg, jumpv, t_recarga;

    public float hor, ver, avanzar, dis, sal, balas, cargador;
    float[] inputs;

    private float t;

    CapsuleCollider m_Collider; // Referencia al collider del Game Object
    Rigidbody m_rigidBody;
    bool m_isGrounded; // Boolean para ser seguro que tocamos el suelo

    Vector3 dir;
    Ray rayo;
    RaycastHit hit;
    Vector3 offset; //distancia camara-jugador
    static public int vida = 100;
    Animator animator;


    void cogerInput()
    {
        inputs = input.getInput();
        hor = inputs[0];
        ver = inputs[1];
        avanzar = inputs[2];
        dis = inputs[3];
        sal = inputs[4];

        // Para las animaciones
        if (inputs[0] != 0  || inputs[2] != 0) animator.SetBool("caminando", true);
        else animator.SetBool("caminando", false);
    }

    void Start()
    {
        // Para las animaciones de movimiento
        animator = GetComponent<Animator>();
        animator.SetBool("caminando", false);

        vida = 100;
        oro = 0;
        offset = cam.transform.position - transform.position; // para camara dungeon crawler

        m_Collider = GetComponent<CapsuleCollider>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_isGrounded = true; // Si empezamos tocando al suelo
        t = Time.time;
        Physics.IgnoreCollision(this.GetComponent<Collider>(), disparo.GetComponent<Collider>());
        ammoDisplay.text = balas.ToString() + "/" + cargador.ToString();
        goldDisplay.text = "0";
        barraVida.value = vida;
        this.transform.position = spawn.transform.position;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && m_isGrounded == true)
            Jump();

        cogerInput();
        transform.Translate(Vector3.right *  hor * v * Time.deltaTime, Space.World);
        transform.Translate(Vector3.forward * avanzar * v * Time.deltaTime, Space.World);

        girar();
        if (balas > 0) { disparar(); ammoDisplay.text = balas.ToString() + "/" + cargador.ToString(); }
        else { recargar(); ammoDisplay.text = "Recargando..."; }

        goldDisplay.text = oro.ToString(); //Actualizamos el dinero del personaje
   
        barraVida.value = vida; //actualizar la barra de vida
        if(vida <= 0) //Si el personaje muere
        {

            //TODO poner segundos de invencibilidad y parpadeo del personaje

            Destroy(this);
            



        }
    }

    void LateUpdate() //Late update para actualizadr la camara
    {

        Vector3 Q =  transform.position + offset;

        cam.transform.position = Vector3.Lerp(cam.transform.position, Q, 0.025f);



    }

    private void disparar()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject ob = Instantiate(disparo);
            ob.transform.position = new Vector3(arma.transform.position.x,arma.transform.position.y, arma.transform.position.z);
            ob.transform.forward = transform.forward;
           Rigidbody ob_rigidb = ob.GetComponent<Rigidbody>();
            ob_rigidb.velocity =  transform.forward * 50;
            ob.transform.localScale = new Vector3(0.4f, 0.4f, 2);
            balas--;
            if (balas == 0) t = Time.time;


        }
    }

    private void recargar()
    {
        if ((Time.time - t) > 0.1f * t_recarga) balas = cargador;


    }


    public void girar()
    {
        rayo = cam.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(rayo, out hit))
        {
            Vector3 pos = hit.point;
            GameObject ob = hit.collider.gameObject;

        }

        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                                    // Para mirar al ratón pero fijando la y a su valor
        transform.LookAt(targetPosition); //miramos hacia la posicion del raton, siempre que toque el terreno 
            
    }


    public void Jump() //saltamos añadiendo una fuerza sobre el rigidbody
    {
        m_isGrounded = false;
        m_rigidBody.AddForce(Vector3.up * jumpv, ForceMode.Impulse);
        t = Time.time;
    }

    // Verificamos las colisiones
    void OnCollisionEnter(Collision other)
    {
        // Hemos puesto un tag "Ground" sobre el suelo
        if (other.gameObject.CompareTag("Ground"));
        m_isGrounded = true;


        if (other.gameObject.CompareTag("enemyBug")){
            vida -= 10;
            Jump();

        }



        if (other.gameObject.CompareTag("mar"))
        {
            vida -= 10;
            m_rigidBody.AddForce(Vector3.up * 10, ForceMode.Impulse);



        }


    }


}
