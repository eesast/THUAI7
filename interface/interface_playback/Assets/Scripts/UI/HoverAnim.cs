using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoverAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public Vector3 sacleOrigin;
    public Vector3 scaleChange;

    // Start is called before the first frame update
    void Start()
    {
        sacleOrigin = transform.localScale;
        scaleChange = sacleOrigin * 1.1f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = scaleChange;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = sacleOrigin;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = sacleOrigin;
    }
    // Update is called once per frame
    void Update()
    {

    }

}
