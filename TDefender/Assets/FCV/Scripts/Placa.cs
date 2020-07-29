using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placa : MonoBehaviour
{
    bool tieneTorreta;

    [SerializeField]
    public GameObject centro;

    // Start is called before the first frame update
    void Start()
    {
        tieneTorreta = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool existeTorreta ()
    {
        return tieneTorreta;
    }

    public void addTorreta()
    {
        tieneTorreta = true;
    }
}
