using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Separation : SteeringAcelerado
{


    //GameObject que contiene todos los compañeros
    [SerializeField]
    public GameObject targets = null;

    [SerializeField]
    public float threshold = 4;

    [SerializeField]
    public float decayCoefficient = 5; //k para la función Inverse square law

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


        Vector3 sumaFuerzas = Vector3.zero;
        foreach (GameObject t in listaTargets)
        {
            Vector3 direccion = t.transform.position - personaje.transform.position;
            float distance = direccion.magnitude;
            if(distance < threshold)
            {

                //Calculamos la fuerza de repulsión
                float strength = Mathf.Min(decayCoefficient / (distance * distance), maxAceleracion);

                //Añadimos la aceleración
                sumaFuerzas += strength * direccion.normalized *-1; //El -1 es para que huya de los targets, en lugar de ir hacia ellos


            }

        }

        return new MovimientoAcelerado
        {
            steeringAngular = 0,
            steeringLineal = sumaFuerzas,
        };





    }



}
