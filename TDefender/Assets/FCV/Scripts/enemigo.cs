using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class enemigo : MonoBehaviour
{
    [SerializeField]
    int vida, velocidad, rangoVision;

    [SerializeField]
    GameObject punto1, punto2, punto3; //3 objetos intermedios hacia los que irá, cuando llegue a uno va a por el siguiente
                                       //Para poder crear una path


    [SerializeField]
    Transform spawn;

    private bool haciaJugador;

    Transform target;
    private int rotationSpeed = 6;
    bool saltar = true;



    // Start is called before the first frame update
    void Start()
    {
        //this.transform.position = spawn.position;
        haciaJugador = false; //Booleanos que indican si han llegado a los puntos de path

        var seed = Environment.TickCount;
        var random = new Random(seed);      
        var value = random.Next(0, 100);      //Generamos un número entre 1 y 2

        if (value < 15)// prob de ir hacia el enemigo
        {
            target = GameObject.FindWithTag("Player").transform;
            haciaJugador = true;
        }
        else
        {
            target = punto1.transform;
            
        }; //probabilidad de ir hacia el castillo
    }

    // Update is called once per frame
    void Update()
    {
        if (vida <= 0)
        {
            enemySpawn.enemigosGlobales++;
            movimientoPersonaje.oro = movimientoPersonaje.oro + 10;
            Destroy(this.gameObject);
        }

        //Calcular distancia
        float distancia;
        distancia = Vector3.Distance(target.transform.position, transform.position);

        //Si la distancia es menor a 4
        if (distancia < rangoVision)
        {

            //Voltear
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
            Quaternion.LookRotation(target.position - this.transform.position), rotationSpeed * Time.deltaTime);
            //Caminar
            this.transform.position += this.transform.forward * velocidad * Time.deltaTime;
            //Lineas de debug que aparecen en la ventana Scene
            Debug.DrawLine(target.transform.position, transform.position, Color.red, Time.deltaTime, false);

        }

        if (!haciaJugador) //En el caso de que el enemigo vaya a por el castillo
        {
            if(Vector3.Distance(transform.position, punto1.transform.position) < 1) target = punto2.transform;
            if (Vector3.Distance(transform.position, punto2.transform.position) < 1) target = punto3.transform;
            if (Vector3.Distance(transform.position, punto3.transform.position) < 1) target = GameObject.FindWithTag("Castle").transform;


        }

        if(saltar) Invoke("salto", 2);
        saltar = false;

    }


    void salto()
    {

        this.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
        saltar = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Disparo"))
            vida = vida - 1;


        if (collision.gameObject.CompareTag("Castle")) // Si colisiona con el castillo muere bajandole vida
        {
            Destroy(this.gameObject);
            enemySpawn.enemigosGlobales++;
            castillo.vidaCastillo = castillo.vidaCastillo - 10;
        }



        if (collision.gameObject.CompareTag("mar")) //rebota al pisar el mar
        {
            salto();
        }


    }
}




