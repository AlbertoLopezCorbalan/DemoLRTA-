using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject effectoBala = (GameObject) Instantiate(ImpactEffect, transform.position, transform.rotation);
            effectoBala.hideFlags = HideFlags.HideInHierarchy;
            Destroy(effectoBala, 1f);

            Destroy(this.gameObject);
        }

    }

    private void Destruction()
    {       
        Destroy(this.gameObject);
    }
}
