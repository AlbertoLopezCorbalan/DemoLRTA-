using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveAcelerado : SteeringAcelerado
{
    Vector3 velocidad;
    Vector3 PQ; // Vector direccion entre el personaje y el target

    [SerializeField]
    float targetRadius = 5;
    [SerializeField]
    float slowRadius = 15;

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidadJugador)
    {
        // Se calcula el vector posicion destino
        PQ = target.transform.position - personaje.transform.position;

        if (PQ.magnitude < targetRadius) // Si estamos suficientemente cerca no hace avanzar
            return
                new MovimientoAcelerado
                {
                    steeringLineal = Vector3.zero,
                    steeringAngular = 0
                };

        float targetSpeed;
        if (PQ.magnitude > slowRadius) // Establece como máximo avance la velocidad 
            targetSpeed = maxVelocidad;
        else
            targetSpeed = maxVelocidad * PQ.magnitude / slowRadius;

        Vector3 targetVelocity = PQ.normalized;
        targetVelocity *= targetSpeed; // Velocidad deseada


        Vector3 steeringlineal = targetVelocity - velocidadJugador; // Velocidad es la velocidad del jugador actualmente

        if ( steeringlineal.magnitude > maxAceleracion)
        {
            steeringlineal = steeringlineal.normalized;
            steeringlineal *= maxAceleracion; // Si lleva demasiada aceleracion se limita
        }

        return
            new MovimientoAcelerado
            {
                steeringLineal = steeringlineal,
                steeringAngular = 0
            };

    }

    private void OnDrawGizmos() // Gizmo: una línea en la dirección del target
    {
        Gizmos.color = Color.yellow;   // Slow Radius
        Gizmos.DrawWireSphere(transform.position, slowRadius);

        Gizmos.color = Color.red;        // Target Radius
        Gizmos.DrawWireSphere(transform.position, targetRadius);
    }


}
