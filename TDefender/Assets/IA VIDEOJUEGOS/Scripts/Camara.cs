using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{

    public float velX = 1f;
    private float movX, movY, movZ;
    private float inputX;


    // Use this for initialization
    void Start()
    {

    }


    void FixedUpdate()
    {



        if (Input.GetKey(KeyCode.RightArrow))
        {
            movX = transform.position.x + (velX);
            transform.position = new Vector3(movX, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(1, 1, 1);


        }


        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movX = transform.position.x - (velX);
            transform.position = new Vector3(movX, transform.position.y, transform.position.z);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movZ = transform.position.z + (velX);
            transform.position = new Vector3(transform.position.x, transform.position.y, movZ);
            transform.localScale = new Vector3(1, 1, 1);


        }


        if (Input.GetKey(KeyCode.DownArrow))
        {
            movZ = transform.position.z - (velX);
            transform.position = new Vector3(transform.position.x, transform.position.y, movZ);
            transform.localScale = new Vector3(-1, 1, 1);
        }



        if (Input.mouseScrollDelta.y > 0f)
        {
            movY = transform.position.y - (velX);
            transform.position = new Vector3(transform.position.x, movY, transform.position.z);
            transform.localScale = new Vector3(1, 1, 1);


        }


        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            movY = transform.position.y + (velX);
            transform.position = new Vector3(transform.position.x, movY, transform.position.z);
            transform.localScale = new Vector3(-1, 1, 1);
        }





    }
}
