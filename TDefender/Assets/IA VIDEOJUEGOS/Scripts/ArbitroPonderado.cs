using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitroPonderado : SteeringAcelerado
{

    [SerializeField]
    GameObject controladorScript = null;

    public override MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidadJugador)
    {
        SteeringAcelerado [] steeringAcelerado = controladorScript.GetComponents<SteeringAcelerado>();

        Vector3 lineal = Vector3.zero;
        float angular = 0;

        for (int i = 0; i < steeringAcelerado.Length; i++)
        {
            MovimientoAcelerado aux = steeringAcelerado[i].getSteering(target, personaje, maxVelocidad, maxAceleracion, maxVelAngular, maxVelAngular, orientacion, velocidadJugador);
            lineal += aux.steeringLineal * steeringAcelerado[i].weigth;
            angular += aux.steeringAngular * steeringAcelerado[i].weigth;
        }

        return new MovimientoAcelerado
        {
            steeringAngular = angular,
            steeringLineal = new Vector3(lineal.x, 0, lineal.z),
        };

    }

}
