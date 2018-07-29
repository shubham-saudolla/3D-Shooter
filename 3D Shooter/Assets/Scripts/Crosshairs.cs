/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public float rotateSpeed;
    public LayerMask targetMask;
    private Color highlightDotColor = Color.red;
    private Color originalDotColor;
    public SpriteRenderer dot;

    void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = highlightDotColor;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
