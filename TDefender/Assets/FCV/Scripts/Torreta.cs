using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torreta : MonoBehaviour
{
    private Transform objetivo;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Atributos")]

    public float rango = 30f;
    public float velocidadGiro = 10f;


    public float cadencia = 1f;
    private float contadorDisparo = 0f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateObjetivo", 0f, 0.5f);
    }

    void UpdateObjetivo()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("enemyBug");
        GameObject enemigoCercano = null;
        float distanciaMasCercana = Mathf.Infinity;
        foreach (GameObject enemigoActual in enemigos)
        {
            float distanciaEnemigo = Vector3.Distance(transform.position, enemigoActual.transform.position); 
            if (distanciaEnemigo < distanciaMasCercana)
            {
                distanciaMasCercana = distanciaEnemigo;
                enemigoCercano = enemigoActual;
            }
        }

        if (enemigoCercano != null && distanciaMasCercana <= rango)
        {
            objetivo = enemigoCercano.transform;
        }
        else
        {
            objetivo = null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (objetivo == null) return;

        // Miramos nuestro target
        Vector3 direccion = objetivo.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direccion);
        Vector3 rotacion = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * velocidadGiro).eulerAngles;
        // Euler -> para que sea entre -360 y 360 como estamos acostumbrados.
        this.transform.rotation = Quaternion.Euler(0f, rotacion.y, 0f);
        

        if (contadorDisparo <= 0f)
        {
            Shoot();
            contadorDisparo = 1f / cadencia;
        }
        contadorDisparo -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject disparo = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.rotation);
        
        disparo.transform.forward = transform.forward;
        Rigidbody ob_rigidb = disparo.GetComponent<Rigidbody>();
        ob_rigidb.velocity = transform.forward * 50;

        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, rango); // Se dibuja una esfera alrededor  de la torreta con range indicado
    }

}
