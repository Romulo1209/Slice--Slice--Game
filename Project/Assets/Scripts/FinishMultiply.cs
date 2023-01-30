using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishMultiply : MonoBehaviour
{
    //Script para delegar os blocos de multiplicação
    [SerializeField] [Range(1, 100)] int multiplier;
    [SerializeField] Color boxColor;
    [SerializeField] TextMesh text;
    [SerializeField] Renderer render;

    public int Multiplier { get { return multiplier; } }

    private void Start()
    {
        render.material.SetColor("_Color", boxColor);
        text.text = multiplier.ToString() + "x";
    }
}
