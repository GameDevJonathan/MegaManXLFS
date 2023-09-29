using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LightSaber : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] public Transform startPos;
    [SerializeField] public Transform endPos;
    [SerializeField] private bool on;
    [SerializeField] private Vector3 extendedPosition = new Vector3();

    [SerializeField] public Transform energyBeam;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        extendedPosition = endPos.localPosition;

    }

    private void Update()
    {
        Vector3 vector3Extend = new Vector3(.9f,.9f,.9f);
        Vector3 vector3Close = new Vector3(.1f,.1f,.1f);
        



        if (energyBeam != null)
        {
            switch (on)
            {
                case true:
                    energyBeam.localScale = new Vector3(
                        Mathf.Lerp(energyBeam.localScale.x, 1, Time.deltaTime * 5f),
                        Mathf.Lerp(energyBeam.localScale.y, 1, Time.deltaTime * 5f), 
                        Mathf.Lerp(energyBeam.localScale.z, 1, Time.deltaTime * 5f));
                    if(energyBeam.localScale.x > vector3Extend.x)
                        energyBeam.localScale = Vector3.one;
                    
                    break;

                case false:
                    energyBeam.localScale = new Vector3(
                        Mathf.Lerp(energyBeam.localScale.x, 0, Time.deltaTime * 5f),
                        Mathf.Lerp(energyBeam.localScale.y, 0, Time.deltaTime * 5f),
                        Mathf.Lerp(energyBeam.localScale.z, 0, Time.deltaTime * 5f));
                    if (energyBeam.localScale.x < vector3Close.x)
                        energyBeam.localScale = Vector3.zero;


                    break;
            }

        }



        if (lineRenderer != null)
        {
            //extend the line
            switch (on)
            {
                case true:
                    endPos.localPosition = Vector3.Lerp(endPos.localPosition, extendedPosition, Time.deltaTime * 5f);
                    break;

                case false:
                    endPos.localPosition = Vector3.Lerp(endPos.localPosition, startPos.localPosition, Time.deltaTime * 5f);

                    break;
            }


            lineRenderer?.SetPosition(0, startPos.position);
            lineRenderer?.SetPosition(1, endPos.position);

        }


    }
}
