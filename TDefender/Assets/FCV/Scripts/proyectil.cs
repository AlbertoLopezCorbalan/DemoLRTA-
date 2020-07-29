using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class proyectil : MonoBehaviour
{
    [SerializeField]
    public float lifetime = 10f;

    [SerializeField]
    public int dmg;

    [SerializeField]
    public GameObject ImpactEffect;

    // Update is called once per frame
    void Update()
    {
        if(lifetime > 0)
        {
            lifetime -= Time.deltaTime;
            if(lifetime <= 0)
            {
                Destruction();
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemyBug"))
        {
            GameObject effectoBala = (GameObject) Instantiate(ImpactEffect, transform.position, transform.rotation);
            Destroy(effectoBala, 1f);

            Destroy(this.gameObject);
        }


        if (collision.gameObject.CompareTag("Castle"))
        {
            GameObject effectoBala = (GameObject)Instantiate(ImpactEffect, transform.position, transform.rotation);
            Destroy(effectoBala, 1f);

            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Walls"))
        {
            GameObject effectoBala = (GameObject)Instantiate(ImpactEffect, transform.position, transform.rotation);
            Destroy(effectoBala, 1f);

            Destroy(this.gameObject);
        }


    }

    private void Destruction()
    {       
        Destroy(this.gameObject);
    }
}
