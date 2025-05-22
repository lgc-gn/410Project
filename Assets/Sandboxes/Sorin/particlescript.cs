using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class particlescript : MonoBehaviour
{
    public Vector3 speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}
