using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Steering : MonoBehaviour
{

    public float weigth = 1.0f;

    public struct Movement
    {
        // Steering
        public Vector3 Linealsteering;
        public float Angularsteering;
    }

    abstract public Movement getSteering(GameObject target, GameObject character, float maxVelocity, float maxAceleration, float maxAngularVelocity, float maxAcelerationVelocity, float orientation, Vector3 characterVelocity);

}
