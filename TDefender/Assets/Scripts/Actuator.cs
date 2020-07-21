using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator : MonoBehaviour
{  
    [SerializeField]
    GameObject target = null;
    [SerializeField]
    Camera cam = null;
    [SerializeField]
    float maxAceleration = 0;
    [SerializeField]
    float maxVelocity = 0; 
    [SerializeField]
    float maxAngularVelocity = 0;
    [SerializeField]
    float maxAcelerationVelocity = 0;


    Vector3 velocity = Vector3.zero; 
    public float orientation = 0;


    // Update is called once per frame
    void FixedUpdate()
    {
     
        // Change rotation and orientation in each frame
        Steering.Movement actualMovement = GetComponent<Steering>().getSteering(target, this.gameObject, maxVelocity, maxAceleration, maxAngularVelocity, maxAcelerationVelocity, orientation, velocity);


        if (actualMovement.Linealsteering == Vector3.zero) // If we recieve Vector3.Zero movement is signal that we are in the objetivo already, we should decelerate
        {
            velocity = velocity - velocity * Time.deltaTime; 
        }
        else
        {
            velocity += actualMovement.Linealsteering * Time.deltaTime; // We change the velocity according the returning values
            if (velocity.magnitude > maxVelocity)
                velocity = velocity.normalized * maxVelocity; // at most the maxVelocity

        }

        orientation += actualMovement.Angularsteering * Time.deltaTime; // We change the rotation according the returning values

        // Apply the changes in our character
        transform.position += velocity * Time.deltaTime;
        transform.rotation = new Quaternion();
        transform.Rotate(Vector3.up, orientation); 

    }

}
