using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public LayerMask interactableLayer;
    RaycastHit raycastHit;
    public List<InteractBase> tobeSelectedInt, selectedInt;
    public bool selectingAll;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, interactableLayer))
        {
            if (!selectingAll)
            {
                Debug.Log("hit");
                raycastHit.collider.GetComponent<InteractBase>().tobeSelected = true;
                tobeSelectedInt.Add(raycastHit.collider.GetComponent<InteractBase>());
                if (Input.GetMouseButtonDown(0))
                {
                    raycastHit.collider.GetComponent<InteractBase>().tobeSelected = false;
                    tobeSelectedInt.Remove(raycastHit.collider.GetComponent<InteractBase>());
                    raycastHit.collider.GetComponent<InteractBase>().selected = true;
                    selectedInt.Add(raycastHit.collider.GetComponent<InteractBase>());
                }
            }
        }
        else
        {
            if (!selectingAll)
            {
                foreach (InteractBase i in tobeSelectedInt)
                {
                    i.tobeSelected = false;
                }
                tobeSelectedInt.Clear();

                if (Input.GetMouseButtonDown(0))
                {
                    foreach (InteractBase i in selectedInt)
                    {
                        i.selected = false;
                    }
                    selectedInt.Clear();
                }
            }
        }
    }
}