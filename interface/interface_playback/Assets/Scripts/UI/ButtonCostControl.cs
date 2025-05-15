using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonCostControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public GameObject cost;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        cost.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        cost.SetActive(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    // Update is called once per frame
    void Update()
    {

    }

}
