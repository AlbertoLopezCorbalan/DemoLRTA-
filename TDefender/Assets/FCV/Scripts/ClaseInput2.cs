using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClaseInput2
{
    public string Horizontal = "Mouse X";
    public string Vertical = "Mouse Y";
    public string Avance = "Vertical";
    public bool puedeDisparar;
    public string Disparo = "Fire1";
    public bool puedeSaltar;
    public string Salto = "Jump";

    public float[] getInput()
    {
        float hor = Input.GetAxis(Horizontal);
        float ver = Input.GetAxis(Vertical);
        float ava = Input.GetAxis(Avance);
        float dis = 0;
        if (puedeDisparar)
        {
            dis = Input.GetAxisRaw(Disparo);
        }
        float sal = 0;
        if (puedeSaltar)
        {
            sal = Input.GetAxisRaw(Salto);
        }
        float[] lista = new float[5];
        lista[0] = hor;
        lista[1] = ver;
        lista[2] = ava;
        lista[3] = dis;
        lista[4] = sal;
        return lista;
    }
}