using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalChessboardBox : MonoBehaviour
{
    public Vector2 position;
    public Collider myCollider;
    public Renderer myRenderer;
    public bool isWhite = false;

    public void SetHilightedColor()
    {
        myRenderer.material.EnableKeyword("_EMISSION");
    }

    public void ResetColor()
    {
        myRenderer.material.DisableKeyword("_EMISSION");
    }
}
