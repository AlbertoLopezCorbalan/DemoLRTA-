using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    public GameObject bulletPrefab;

    [SerializeField]
    GameObject firePoints = null;

    [SerializeField]
    GameObject upperBody = null;


    public float range = 30f;
    public float turnVelocity = 2f;
    
    public float cadence = 1f;
    private float shootCount = 0f;

    
    void Start()
    {
        InvokeRepeating("UpdateObjetivo", 0f, 0.5f);
    }

    void UpdateObjetivo()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closerEnemy = null;
        float closerEnemyDistance = Mathf.Infinity;
        foreach (GameObject actualEnemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, actualEnemy.transform.position); 
            if (distance < closerEnemyDistance)
            {
                closerEnemyDistance = distance;
                closerEnemy = actualEnemy;
            }
        }

        if (closerEnemy != null && closerEnemyDistance <= range)
        {
            target = closerEnemy.transform;
        }
        else
        {
            target = null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        // Check the target
        Vector3 direccion = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direccion);
        Vector3 rotacion = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnVelocity).eulerAngles;

        // Put the cannon looking at the target (mobile part)

        if (upperBody != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - upperBody.transform.position);
            upperBody.transform.rotation = Quaternion.Lerp(upperBody.transform.rotation, targetRotation, Time.deltaTime * turnVelocity);
        }


        // Euler -> between -360 and 360
        this.transform.rotation = Quaternion.Euler(0f, rotacion.y, 0f);
        

        if (shootCount <= 0f)
        {
            Shoot();
            shootCount = 1f / cadence;
        }
        shootCount -= Time.deltaTime;
    }

    void Shoot()
    {

        //We shoot from each Fire Point
        if (firePoints != null)
        {
            int numFirePoints = firePoints.transform.childCount;
            for (int i = 0; i < numFirePoints; i++)
            {
                GameObject firePoint = firePoints.transform.GetChild(i).gameObject;


                GameObject shooting = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
                shooting.hideFlags = HideFlags.HideInHierarchy;         
                shooting.transform.forward = target.position - firePoint.transform.position;
                Rigidbody ob_rigidb = shooting.GetComponent<Rigidbody>();
                ob_rigidb.velocity = shooting.transform.forward * 50;


            }
        }
       
    }
}
