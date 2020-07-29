using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAcelerado : SteeringAcelerado
{
    Vector3 velocidad;
    Vector3 PQ; // Vector direccion entre el personaje y el target

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        // Se calcula el vector posicion destino
        PQ = target.transform.position - personaje.transform.position;

        // Si esta demasiaco cerca, no avanzamos el personaje
        Vector3 vectorSteeringLineal;
        if (PQ.magnitude > 1) vectorSteeringLineal = PQ.normalized * maxAceleracion;
        else vectorSteeringLineal = Vector3.zero; // Si está muy cerca no se movería


        return
            new MovimientoAcelerado
            {
                steeringLineal = new Vector3(vectorSteeringLineal.x, 0 , vectorSteeringLineal.z),
                steeringAngular = 0
            };

    }


}
