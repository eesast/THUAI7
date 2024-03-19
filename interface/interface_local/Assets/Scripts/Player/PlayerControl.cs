using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : SingletonMono<PlayerControl>
{
    public LayerMask interactableLayer;
    Collider2D raycaster;
    public List<InteractBase> tobeSelectedInt, selectedInt;
    public bool selectingAll;
    public List<InteractControl.InteractOption> enabledInteract;
    public InteractControl.InteractOption selectedOption;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        testInput();
        CheckInteract();
        UpdateInteractList();
        Interact();
        ShipMpve();
        ShipAttack();
    }
    void testInput()
    {
    }
    void CheckInteract()
    {
        raycaster = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition), interactableLayer);
        if (raycaster)
        {
            // Debug.Log("raycasthit");
            if (!selectingAll)
            {
                raycaster.GetComponent<InteractBase>().tobeSelected = true;
                if (!tobeSelectedInt.Contains(raycaster.GetComponent<InteractBase>()))
                {
                    tobeSelectedInt.Add(raycaster.GetComponent<InteractBase>());
                }
                if (Input.GetMouseButtonDown(0))
                {
                    foreach (InteractBase i in selectedInt)
                    {
                        i.selected = false;
                    }
                    selectedInt.Clear();
                    raycaster.GetComponent<InteractBase>().tobeSelected = false;
                    tobeSelectedInt.Remove(raycaster.GetComponent<InteractBase>());
                    raycaster.GetComponent<InteractBase>().selected = true;
                    if (!selectedInt.Contains(raycaster.GetComponent<InteractBase>()))
                    {
                        selectedInt.Add(raycaster.GetComponent<InteractBase>());
                    }
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

                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
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
    void UpdateInteractList()
    {
        if (selectedInt.Count > 0)
        {
            enabledInteract = new List<InteractControl.InteractOption>(InteractControl.GetInstance().interactOptions[selectedInt[0].interactType]);
            // Debug.Log(InteractControl.GetInstance().interactOptions[InteractControl.InteractType.Base].Count);
            foreach (InteractBase interactBase in selectedInt)
            {
                foreach (InteractControl.InteractOption interactOption in enabledInteract)
                    if (!InteractControl.GetInstance().interactOptions[interactBase.interactType].Contains(interactOption))
                    {
                        enabledInteract.Remove(interactOption);
                    }
            }

        }
        else
        {
            if (enabledInteract.Count > 0)
                enabledInteract.Clear();
        }
    }
    void Interact()
    {
        foreach (InteractBase interactBase in selectedInt)
        {
            interactBase.interactOption = selectedOption;
        }
        selectedOption = InteractControl.InteractOption.None;
    }
    void ShipMpve()
    {
        if (Input.GetMouseButtonDown(1))
        {
            foreach (InteractBase interactBase in selectedInt)
            {
                interactBase.moveOption = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
    void ShipAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (InteractBase interactBase in selectedInt)
            {
                interactBase.attackOption = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
    }
}