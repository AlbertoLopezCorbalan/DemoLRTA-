using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringAcelerado
{
    Vector3 velocidad;
    Vector3 PQ; // Vector direccion entre el personaje y el objetivo
    float rotacion;
    float rotationSize;

    [SerializeField]
    float targetRadius = 5;
    [SerializeField]
    float slowRadius = 30;

    override
    public MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidad)
    {
        float orientacionTarget = getOrientacion(target); // Se obtiene la orientación del target
 
        // Angulo entre los 2 personajes
        rotacion = orientacionTarget - orientacion;
        // Te lo devolverá como máximo en 180 grados (entre -180 grados y 180), y el signo con respecto a la componente de arriba
        rotacion = MapToRange(rotacion);
        rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius) // Si estamos suficientemente cerca no hace falta girar
            return
                new MovimientoAcelerado
                {
                    steeringLineal = Vector3.zero,
                    steeringAngular = 0
                };

        float targetRotation;
        if (rotationSize > slowRadius) // Establece como máximo giro la maxima velocidad angular, poniendo el targetRotation a la velocidad maxima
            targetRotation = maxVelAngular;
        else
            targetRotation = maxVelAngular * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize; // Para saber el sentido de la rotación

        float steeringangular = targetRotation - orientacion;

        if (Mathf.Abs(steeringangular) > maxAceleracionAngular)
        {
            steeringangular /= Mathf.Abs(rotacion);
            steeringangular *= maxAceleracionAngular; // Se pone a la máxima aceleración angular si va demasiado acelerado
        }

        return
            new MovimientoAcelerado
            {
                steeringLineal = Vector3.zero,
                steeringAngular = targetRotation
            };

    }

    public float MapToRange (float rotation)
    {
        rotation %= 360.0f;
        if (Math.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }



    private void OnDrawGizmos() // Gizmo: una línea en la dirección del objetivo
    {
        Vector3 from = transform.position; // Origen de la línea
     
        Gizmos.color = Color.yellow;        // Mirando en la dirección de la orientación.
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(from, direction);
    }

}
