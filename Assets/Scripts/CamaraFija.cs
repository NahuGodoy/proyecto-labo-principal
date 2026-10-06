using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private Transform CameraPivot;
    [SerializeField]
    private float velocidadCamara = 120;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, CameraPivot.position, velocidadCamara * Time.deltaTime);
    }
}
