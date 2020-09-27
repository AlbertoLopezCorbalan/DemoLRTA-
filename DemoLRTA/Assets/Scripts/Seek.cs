using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Steering
{
    Vector3 PQ; // Direction vector between the character and the objetive

    override
    public Movement getSteering(GameObject target, GameObject character, float maxVelocity, float maxAceleration, float maxAngularVelocity, float maxAcelerationVelocity, float orientation, Vector3 characterVelocity)
    {
        PQ = target.transform.position - character.transform.position;
        Vector3 Linealsteering;
        if (PQ.magnitude > 1) Linealsteering = PQ.normalized * maxAceleration;
        else Linealsteering = Vector3.zero; // if its so close don't move


        return
            new Movement
            {
                Linealsteering = new Vector3(Linealsteering.x, 0 , Linealsteering.z),
                Angularsteering = 0
            };

    }


}
