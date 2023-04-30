using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public enum FloorColour
    {
        green,
        blue,
        red,
        purple,
    }

    [SerializeField] private FloorColour floorColour;
    [SerializeField] private float lowerSpeed = 2f;
    private bool lowerTile;

    private void Update() 
    {
        if (lowerTile)
        {
            transform.Translate(Vector3.down * lowerSpeed * Time.deltaTime);
        }    
    }

    public void StopLowering()
    {
        lowerTile = false;
    }

    public void LowerTile(FloorColour currentColour)
    {
        if (currentColour != floorColour)
        {
            lowerTile = true;
        }
    }
}
