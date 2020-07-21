using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castillo : MonoBehaviour
{
    
    public Text gameOver;

    public enum Equipo { TeamAzul, TeamRojo }

    public float vidaCastillo = 100;

    [SerializeField]
    private Equipo equipoEnemigos = Equipo.TeamAzul;

    public Slider barraVidaCastillo;


    // Start is called before the first frame update
    void Start()
    {

        gameOver.enabled = false;


    }

    // Update is called once per frame
    void Update()
    {
        barraVidaCastillo.value = vidaCastillo;
        if (vidaCastillo <= 0)
        {
            Time.timeScale = 0; 	//que la velocidad del juego sea 0
            gameOver.enabled = true;

        }
        else
        {
            gameOver.enabled = false;

        }

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(equipoEnemigos.ToString()))
        {

            collision.gameObject.GetComponent<Combate>().morir();
            vidaCastillo -= 5;


        }
    }





}
