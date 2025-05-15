using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : SingletonMono<PlayerControl>
{
    public LayerMask interactableLayer;
    Collider2D raycaster;
    public List<InteractBase> tobeSelectedInt, selectedInt;
    public bool selectingAll, drag;
    // public List<InteractControl.InteractOption> enabledInteract;
    // public InteractControl.InteractOption selectedOption;
    public float longClickTime, longClickTimer;
    public Vector2 clickPnt, cameraPos;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // testInput();
        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     foreach (InteractBase i in selectedInt)
        //     {
        //         i.selected = false;
        //     }
        //     selectedInt.Clear();
        // }
        if (Input.GetMouseButtonDown(0))
        {
            longClickTimer = longClickTime;
            cameraPos = Camera.main.transform.position;
            clickPnt = Input.mousePosition;
        }
        longClickTimer -= Time.deltaTime;
        if (longClickTimer < 0)
            longClickTimer = 0;
        CheckInteract();
        // UpdateInteractList();
        // Interact();
        // ShipAttack();
    }
    // void testInput()
    // {
    //     if (Input.GetKeyDown(KeyCode.P))
    //         selectedOption = InteractControl.InteractOption.Produce;
    //     if (Input.GetKeyDown(KeyCode.C))
    //         selectedOption = InteractControl.InteractOption.ConstructFactory;
    // }
    void CheckInteract()
    {
        for (int i = 0; i < selectedInt.Count; i++)
            if (!selectedInt[i])
            {
                selectedInt.Remove(selectedInt[i]);
            }
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
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {

                        foreach (InteractBase i in selectedInt)
                        {
                            i.selected = false;
                        }
                        selectedInt.Clear();
                    }
                    raycaster.GetComponent<InteractBase>().tobeSelected = false;
                    tobeSelectedInt.Remove(raycaster.GetComponent<InteractBase>());
                    raycaster.GetComponent<InteractBase>().selected = true;
                    if (!selectedInt.Contains(raycaster.GetComponent<InteractBase>()))
                    {
                        selectedInt.Add(raycaster.GetComponent<InteractBase>());
                    }
                }
                // if (Input.GetMouseButtonDown(1))
                // {
                //     ShipMove(raycaster.transform.position);
                // }
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
                // Debug.Log("clear" + tobeSelectedInt.Count);
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    // Debug.Log("not over gameobject");
                    // if (Input.GetMouseButtonUp(0) && longClickTimer > 0)
                    // {
                    // ShipAttack();
                    // }
                    if (Input.GetMouseButtonDown(0))
                        drag = true;
                    if (Input.GetMouseButton(0))
                    {
                        if (drag)
                        {
                            Camera.main.transform.position = (Vector3)cameraPos - Camera.main.ScreenToWorldPoint(Input.mousePosition)
                                + (Vector3)Camera.main.ScreenToWorldPoint(clickPnt);
                            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
                        }
                    }
                    else
                    {
                        drag = false;
                    }
                }
                // if (Input.GetMouseButtonDown(1))
                // {
                // ShipMove(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                // }
            }
        }
    }
    // void UpdateInteractList()
    // {
    //     if (selectedInt.Count > 0)
    //     {
    //         // Debug.Log(selectedInt[0].interactType + "   " + InteractControl.GetInstance().interactOptions[selectedInt[0].interactType]);
    //         enabledInteract = new List<InteractControl.InteractOption>(InteractControl.GetInstance().interactOptions[selectedInt[0].interactType]);
    //         // Debug.Log(InteractControl.GetInstance().interactOptions[InteractControl.InteractType.Base].Count);
    //         foreach (InteractBase interactBase in selectedInt)
    //         {
    //             for (int i = 0; i < enabledInteract.Count; i++)
    //                 if (!InteractControl.GetInstance().interactOptions[interactBase.interactType].Contains(enabledInteract[i]))
    //                 {
    //                     enabledInteract.Remove(enabledInteract[i]);
    //                 }
    //         }
    //     }
    //     else
    //     {
    //         if (enabledInteract.Count > 0)
    //             enabledInteract.Clear();
    //     }
    // }
    // void Interact()
    // {
    //     foreach (InteractBase interactBase in selectedInt)
    //     {
    //         interactBase.interactOption = selectedOption;
    //     }
    //     selectedOption = InteractControl.InteractOption.None;
    // }
    // void ShipMove(Vector2 movePos)
    // {
    //     foreach (InteractBase interactBase in selectedInt)
    //     {
    //         interactBase.enableMove = true;
    //         interactBase.moveOption = movePos;
    //     }
    // }
    // void ShipAttack()
    // {
    //     foreach (InteractBase interactBase in selectedInt)
    //     {
    //         interactBase.attackOption = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     }
    // }
    // public void ButtonInteract(InteractControl.InteractOption option)
    // {
    //     selectedOption = option;
    // }
}