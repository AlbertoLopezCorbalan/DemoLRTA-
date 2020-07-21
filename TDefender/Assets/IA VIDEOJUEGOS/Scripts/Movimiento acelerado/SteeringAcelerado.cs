using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringAcelerado : MonoBehaviour
{

    public float weigth = 1.0f;

    public struct MovimientoAcelerado
    {
        // Steering
        public Vector3 steeringLineal;
        public float steeringAngular;
    }

    abstract public  MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidadJugador);


    // Ahora habrá dos tipos de steering, los que aplican una velocidad angular, y los que aplican una velocidad lineal.
    // Por ejemplo, el seek tendrá velocidad angular = 0, y aplicará una una velocidad linear.


    public float getOrientacion(GameObject target) {
        return target.transform.rotation.eulerAngles.y;
    }

    public Vector3 getVectorOrientacion (GameObject target, float orientacion)
    {
        Vector3 z = new Vector3(0, 0, 1); // El forward está en la dirección z
        z = Quaternion.Euler(0, orientacion, 0) * z;
        return z;
    }


}
