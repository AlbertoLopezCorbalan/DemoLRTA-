using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class Combate : MonoBehaviour
{
    public float vida = 100;
    public float rango = 1;
    public float damage = 10; //Daño de ataque del personaje
    public float probabilidadCritico = 0.05f;
    public float MultiplicadorCritico = 2f;
    private float tRespawn = 5; //Tiempo que se tarda en reaparecer
    private float tMuerte; //Instante de tiempo en el que murió el personaje
    private bool isAlive = true;


    [SerializeField]
    private GameObject puntoRespawn = null;


    // Start is called before the first frame update
    private float tiempoUltAtaque;


    public enum Equipo { TeamAzul, TeamRojo }

    [SerializeField]
    private Equipo equipoEnemigos = Equipo.TeamAzul;

    [SerializeField]
    public GameObject ImpactEffect;

    Animator animator = null; // Posible animator, que tendría un hijo, al que habrá que notificarle si estoy en movimiento o no.
    private LinkedList<GameObject> enemigos = new LinkedList<GameObject>();


    void Start()
    {
        //vida = 100;
        tiempoUltAtaque = Time.time;
        animator = GetComponentInChildren<Animator>();


        //Cogemos todos los enemigos que tenga el tag de su equipo y lo añadimos a la lista
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag(equipoEnemigos.ToString());
        foreach (GameObject g in enemies)
        {
            enemigos.AddLast(g);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cura"))
            vida = 100;
    }



    // Update is called once per frame
    void Update()
    {
        atacar();
        if (isAlive && vida <= 0) morir();

        float t = Time.time;
        if (!isAlive && (t - tMuerte) > tRespawn) revivir();




    }


    void atacar()
    {
        ActuadorParte2 actuador = this.gameObject.GetComponent<ActuadorParte2>();
        foreach (GameObject g in enemigos)
        {
            
            float t = Time.time;
            if(Vector3.Distance(this.transform.position, g.transform.position) <= rango && ((t - tiempoUltAtaque) > 0.5f) ) // si se cumple esta condidición atacamos al target mas cercano
            {
                tiempoUltAtaque = Time.time;
                if (animator != null)
                {
                    animator.ResetTrigger("Attack1Trigger");
                    animator.SetTrigger("Attack1Trigger"); // Ataca el personaje     
                    actuador.Ataque();
                }

                if (Random.Range(0.0f, 1.0f) < probabilidadCritico)
                    g.GetComponent<Combate>().vida -= damage * MultiplicadorCritico;
                else g.GetComponent<Combate>().vida -= damage;

            }
        }
    }

    public void morir()
    {

        GameObject effectoBala = (GameObject)Instantiate(ImpactEffect, transform.position, transform.rotation);
        Destroy(effectoBala, 1f);


        tMuerte = Time.time;
        isAlive = false;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 1000, this.transform.position.z);
        //this.gameObject.SetActive(false);



    }

    public void revivir()
    {
        //this.gameObject.SetActive(true);
        isAlive = true;
        vida = 100;
        this.transform.position = new Vector3(puntoRespawn.transform.position.x, puntoRespawn.transform.position.y, puntoRespawn.transform.position.z);


    }

    public bool getIsAlive()
    {
        return isAlive;

    }





}
