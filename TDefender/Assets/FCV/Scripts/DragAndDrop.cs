using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public GameObject torreta;
    GameObject PrefabFlotante;

    [SerializeField]
    int precio;

    GameObject placaActual = null;

    // Use this for initialization
    void Start()
    {
        PrefabFlotante = Instantiate(torreta);
        PrefabFlotante.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
    }


    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (checkPlaca(hit))
            {
                placaActual = hit.collider.gameObject;
                PrefabFlotante.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                PrefabFlotante.SetActive(true);
            }
            else
            {
                PrefabFlotante.SetActive(false);
                placaActual = null;
            }
        }
    }

    // Para chequear que es una placa donde poder poner las torretas
    bool checkPlaca(RaycastHit hit)
    {
        return (hit.collider.gameObject.tag.Equals("Placa"));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // If the prefab instance is active after dragging stopped, it means
        // it's in the arena so (for now), just drop it in.
        if (PrefabFlotante.activeSelf)
        {
            //comprobamos que el oro que tenemos es superior al precio que cuesta la torreta
            if (movimientoPersonaje.oro >= precio) {
                movimientoPersonaje.oro = movimientoPersonaje.oro - precio;
                Placa placa = placaActual.GetComponent<Placa>();
                if (!placa.existeTorreta()) Instantiate(torreta, placa.centro.transform.position, PrefabFlotante.transform.rotation);
                // Se comprueba antes de instanciar quen o haya una torreta en el sitio
                placa.addTorreta(); // Indicas que ya hay una torreta en ese sitio
            }
            
        }

        // Then set it to inactive ready for the next drag!
        PrefabFlotante.SetActive(false);
    }
}
