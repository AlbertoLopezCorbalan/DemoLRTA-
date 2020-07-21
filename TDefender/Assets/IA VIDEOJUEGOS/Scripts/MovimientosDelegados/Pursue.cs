using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursue : SeekAcelerado
{

    Vector3 direccion; // Vector direccion entre el personaje y el objetivo
    float distancia;
    float speed; //Nuestra velocidad escalar

    float prediccion; //Tiempo en el que tardaríamos en alcanzar al objetivo
    GameObject pseudoObjetivo; //Nuevo target imaginario, que describe la posición donde predecimos que estará el target original.

    [SerializeField]
    float maxPrediccion = 0.5f; //Tiempo maximo de predicción para alcanzar al objetivo

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {


        // Se calcula el vector posicion destino
        direccion = target.transform.position - personaje.transform.position;
        distancia = direccion.magnitude; //Modulo de la direccion entre el personaje y el objetivo

        speed = velocidad.magnitude;

        //Check si la velocidad es demasiado pequeña, para darle un tiempo de predicción razonable.
        if (speed <= distancia / maxPrediccion)
            prediccion = maxPrediccion;

        else prediccion = distancia / speed;    // V = D/T -> T = D/V

        Vector3 nuevoObjetivo = target.transform.position + (target.transform.position * prediccion);
        pseudoObjetivo.transform.position = nuevoObjetivo;

        return base.getSteering(pseudoObjetivo, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxAceleracionAngular, orientacion, velocidad);



    }

    private void Start()
    {
        pseudoObjetivo = new GameObject();
        pseudoObjetivo.name = "Target Imaginario";
        pseudoObjetivo.SetActive(false);
        pseudoObjetivo.hideFlags = HideFlags.HideInHierarchy; // Para que no se muestre en la jerarquía
    }
}
