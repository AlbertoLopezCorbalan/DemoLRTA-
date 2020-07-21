using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAcelerado : SteeringAcelerado
{
    Vector3 velocidad;
    Vector3 PQ; // Vector direccion entre el personaje y el objetivo

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        // Se calcula el vector posicion destino
        PQ = target.transform.position - personaje.transform.position;

        return
            new MovimientoAcelerado
            {
                steeringLineal = -PQ.normalized * maxAceleracion,
                steeringAngular = 0
            };

    }
}
