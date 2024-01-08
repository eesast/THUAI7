using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
        target.transform.Find("mask1").GetComponent<SpriteRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
