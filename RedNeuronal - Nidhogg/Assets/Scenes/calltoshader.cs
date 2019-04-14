using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class calltoshader : MonoBehaviour
{
    public Material mat;
    [ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
