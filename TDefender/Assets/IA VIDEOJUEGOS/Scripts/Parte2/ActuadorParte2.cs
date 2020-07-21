using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActuadorParte2 : MonoBehaviour
{

    // Start is called before the first frame update

    public enum Modo { GuerraTotal, Ataque, Defensa, Custom}
    public Modo modo = Modo.Defensa;
    private Modo modoAnterior = Modo.Defensa;

    private GameObject target;

    [SerializeField]
    GameObject CastilloEnemigo = null;

    [SerializeField]
    GameObject CastilloAliado = null;


    [SerializeField]
    GameObject rutaPatrulla = null;

    public enum ColorEnemigo { Rojo, Azul }
    public ColorEnemigo colorEnemigo;

    Combate Combate;


    // Las tropas enemigas se tomarán mediante el tag

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
    public float ralentizacionAnimacion = 1.0f;

    private void Start()
    {
        esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Establecemos tamaño
        esfera.SetActive(false);
        esfera.hideFlags = HideFlags.HideInHierarchy;

        animator = GetComponentInChildren<Animator>();
        Combate = GetComponent<Combate>();
        target = CastilloEnemigo;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        esfera.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        comportamientoTactico();

        // Decisiones del comprotamiento táctico para determinar el nuevo target.
        SteeringAcelerado.MovimientoAcelerado movimientoActual = GetComponent<SteeringAcelerado>().getSteering(target, this.gameObject, maxVelocidad, maxAceleracion, maxVelocidadAngular, maxAceleracionAngular, orientacion, velocidad);


        // Modificación de nuestros parámetros actuales

        if (movimientoActual.steeringLineal == Vector3.zero) //Si la velocidad es cero, bajamos la inercia a la mitad
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
        transform.position += velocidad * dificultadTerreno * ralentizacionAnimacion * Time.deltaTime; // Modificamos la posición con la velocidad
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

        if (!this.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            TerminaAtaque();
        }


        // VARIABLES PARA COMPORTAMIENTO TÁCTICO

        if (Input.GetKey("g"))
        {
            modo = Modo.GuerraTotal;
        }

        if (Input.GetKey("a") && seleccionado) // Se cambia de modo si está seleccionado
        {
            seleccionado = false;
            esfera.SetActive(false);
            modo = Modo.Ataque;
        }

        if (Input.GetKey("d") && seleccionado)
        {
            seleccionado = false;
            esfera.SetActive(false);
            modo = Modo.Defensa;
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

    public void Ataque ()
    {
        ralentizacionAnimacion = 0.0f;
    }

    public void TerminaAtaque()
    {      
        ralentizacionAnimacion = 1.0f;
    }




    public void comportamientoTactico()
    {
        if (Input.GetMouseButtonDown(1) && seleccionado) // SI SE DA UN TARGET POR TECLADO TIENE PREFERENCIA
        {
            Ray rayo = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
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
            seleccionado = false;
            esfera.SetActive(false);
            modoAnterior = modo;
            modo = Modo.Custom;
            return;
        }




        if (Combate.vida <= 10) // BUSCO CURAS URGENTE
        {
            GameObject cura = curaMasCercana();
            if (cura != null) target = cura;
            return;
        }


        Castillo castilloAliadoScript = CastilloAliado.GetComponent<Castillo>();
        if (castilloAliadoScript.vidaCastillo < 20) // SI NUESTRO CASTILLO ESTÁ A MENOS DE UN 20% DE VIDA, TODOS LOS RANGED SE QUEDAN DEFENDIENDO
        {
            if (Combate.rango >= 10) this.modo = Modo.Defensa;
        }


        Castillo castilloEnemigoScript = CastilloEnemigo.GetComponent<Castillo>();
        if (castilloEnemigoScript.vidaCastillo < 20) // SI EL CASTILLO ENEMIGO ESTÁ A MENOS DE UN 20% DE VIDA, TODOS LOS MELESS ATACAN
        {
            if (Combate.rango < 10) this.modo = Modo.Ataque;
        }




        GameObject enemigoMasCerca = enemigoCercano(); // SI HAY UN  ENEMIGO CERCANO

        if (enemigoMasCerca != null && Vector3.Distance(enemigoMasCerca.transform.position, this.transform.position) < 10)
        { // 10 metros
            if (Combate.rango < 10)
                target = enemigoMasCerca; // SI SOY MELEE VOY A POR ÉL DIRECTAMENTE

            // SI SOY RANGED MANTENTO LA POSICION PARA GOLPEAR
            else { 
                GameObject pseudoObjetivo = new GameObject();
                pseudoObjetivo.name = "Nuevo Objetivo";
                pseudoObjetivo.SetActive(false);
                pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy;
                pseudoObjetivo.transform.position = this.transform.position;
                target = pseudoObjetivo;
            }

        
            return;

        }

        if (modo == Modo.Custom)
        {
            if (Vector3.Distance(this.transform.position, target.transform.position) < 5) modo = modoAnterior;
            return;
        }


        if (Modo.GuerraTotal == modo) // MODO GUERRA TOTAL
        {
            target = CastilloEnemigo; // Vamos al castillo enemigo a toda costa, ya que es la manera de poder ganar
            return;
        }


        if (Combate.vida <= 40) // BUSCO CURAS
        {
            GameObject cura = curaMasCercana();
            if (cura != null) target = cura;
            return;
        }

        if (Modo.Ataque == modo) // MODO ATAQUE
        {

  
            target = CastilloEnemigo; // Vamos al castillo enemigo
            return;
        }

        if (Modo.Defensa == modo) // MODO DEFENSA
        {
            GameObject siguientePunto = siguientePuntoRuta();
            if (siguientePunto != null) target = siguientePunto;
        }

        




    }


          
    public GameObject enemigoCercano()
    {
        GameObject[] enemigos = null;
        switch (colorEnemigo)
        {
            case ColorEnemigo.Rojo: { enemigos = GameObject.FindGameObjectsWithTag("TeamRojo"); break; }
            case ColorEnemigo.Azul: { enemigos = GameObject.FindGameObjectsWithTag("TeamAzul"); break; }
        }

        float distanciaMinima = Mathf.Infinity;
        GameObject masCerca = null;
        foreach (GameObject g in enemigos)
        {
            Vector3 distancia = new Vector3(g.transform.position.x, 0, g.transform.position.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
            if (distancia.magnitude < distanciaMinima && g.GetComponent<Combate>().getIsAlive()) // <---Solo comprobamos con los enemigos que estén vivos
            {

                distanciaMinima = distancia.magnitude;
                masCerca = g;
                
            }
        }
        return masCerca; //Solo seleccionamos el enemigo mas cercano si está vivo

    }


    public GameObject curaMasCercana()
    {
        GameObject [] curas = GameObject.FindGameObjectsWithTag("Cura");

        float distanciaMinima = Mathf.Infinity;
        GameObject masCerca = null;
        foreach (GameObject g in curas)
        {
            Vector3 distancia = new Vector3(g.transform.position.x, 0, g.transform.position.z) - new Vector3(this.transform.position.x, 0, this.transform.position.z);
            if (distancia.magnitude < distanciaMinima)
            {
                distanciaMinima = distancia.magnitude;
                masCerca = g;
            }
        }
        return masCerca;

    }


    public GameObject siguientePuntoRuta()
    {
        if (rutaPatrulla == null) return null;

        List<GameObject> listaPuntos = new List<GameObject>(rutaPatrulla.transform.childCount);
        int numPuntos = rutaPatrulla.transform.childCount;
        for (int i = 0; i < numPuntos; i++)
        {
            GameObject g = rutaPatrulla.transform.GetChild(i).gameObject;
            listaPuntos.Insert(i, g);
        }

        if (numPuntos == 1) return listaPuntos[0];

        //inicializamos el objetivo
        GameObject masCercano = listaPuntos[0];
        GameObject objetivo = listaPuntos[1];
        float distMaxima = (listaPuntos[0].transform.position - this.transform.position).magnitude;

        //Buscamos el punto mas cercano
        for (int i = 0; i < numPuntos; i++)
        {
            float dist = (listaPuntos[i].transform.position - this.transform.position).magnitude;
            if (dist < distMaxima)
            {
                distMaxima = dist;
                masCercano = listaPuntos[i];
                objetivo = listaPuntos[(i + 1) % numPuntos];
            }

        }

        return objetivo;

    }


}
