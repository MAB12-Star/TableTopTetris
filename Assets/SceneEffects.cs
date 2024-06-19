using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEffects : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer; 
    // Start is called before the first frame update
    public void LowerOpacity()
    {
        float newOpacity = Mathf.Clamp01(passthroughLayer.textureOpacity - .02f);
        passthroughLayer.textureOpacity = newOpacity;
    }

    public void ChangeColor()
    {
        passthroughLayer.edgeColor = Color.blue;
    }
    public void RaiseOpacity()
    {
        passthroughLayer.textureOpacity = 1f;

    }
}
