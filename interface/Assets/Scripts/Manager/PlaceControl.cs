using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class PlaceControl : MonoBehaviour
{
    public PlaceType placeType;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(placeType, gameObject.transform);
    }
}
