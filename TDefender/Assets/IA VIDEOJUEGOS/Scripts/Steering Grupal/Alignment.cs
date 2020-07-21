using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringAcelerado
{
    //GameObject que contiene todos los compañeros
    [SerializeField]
    GameObject targets = null;

    [SerializeField]
    float threshold = 4;

    public override MovimientoAcelerado getSteering(GameObject target, GameObject personaje, float maxVelocidad, float maxAceleracion, float maxVelAngular, float maxAceleracionAngular, float orientacion, Vector3 velocidadJugador)
    {
        //Definimos un steering en el caso de que no haya que mover el personaje
        MovimientoAcelerado noMover = new MovimientoAcelerado
        {
            steeringAngular = orientacion,
            steeringLineal = Vector3.zero,
        };

        //si no hay gameObject que evitar
        if (targets == null) return noMover;

        //Extraemos del GameObject targets
        List<GameObject> listaTargets = new List<GameObject>(targets.transform.childCount);
        int numTargets = targets.transform.childCount;
        for (int i = 0; i < numTargets; i++)
        {
            GameObject g = targets.transform.GetChild(i).gameObject;

            if (!g.Equals(this)) listaTargets.Insert(i, g);
        }
        numTargets--; //Restamos al numeró de targets nosotros mismos.


        if (numTargets < 1) return noMover; //En el caso de que no haya mas targets

        int count = 0;

        float heading = 0;
        foreach (GameObject t in listaTargets)
        {
            Vector3 direccion = t.transform.position - personaje.transform.position;
            float distance = Mathf.Abs(direccion.magnitude);
            if (distance < threshold)
            {
                heading += getOrientacion(t);
                count++;

            }

        }

        if(count > 0)
        {
            heading /= count;
            heading -= orientacion;
        }

        return new MovimientoAcelerado
        {
            steeringAngular = heading,
            steeringLineal = Vector3.zero,
        };





    }

}
