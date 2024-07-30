using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEffects : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public List<GameObject> particlePrefabs; // Serialized list to hold particle prefabs

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

    void OnEnable()
    {
        Debug.Log("SceneEffects: OnEnable");
        OVRManager.HMDMounted += HandleHMDMounted;
        OVRManager.HMDUnmounted += HandleHMDUnmounted;
        OVRManager.VrFocusAcquired += HandleVrFocusAcquired;
        OVRManager.VrFocusLost += HandleVrFocusLost;
    }

    void OnDisable()
    {
        Debug.Log("SceneEffects: OnDisable");
        OVRManager.HMDMounted -= HandleHMDMounted;
        OVRManager.HMDUnmounted -= HandleHMDUnmounted;
        OVRManager.VrFocusAcquired -= HandleVrFocusAcquired;
        OVRManager.VrFocusLost -= HandleVrFocusLost;
    }

    private void HandleHMDMounted()
    {
        Debug.Log("SceneEffects: HandleHMDMounted");
        ResumeGame();
    }

    private void HandleHMDUnmounted()
    {
        Debug.Log("SceneEffects: HandleHMDUnmounted");
        PauseGame();
    }

    private void HandleVrFocusAcquired()
    {
        Debug.Log("SceneEffects: HandleVrFocusAcquired");
        ResumeGame();
    }

    private void HandleVrFocusLost()
    {
        Debug.Log("SceneEffects: HandleVrFocusLost");
        PauseGame();
    }

    private void PauseGame()
    {
        Debug.Log("SceneEffects: PauseGame");
        Time.timeScale = 0;
        // Optionally, show a pause menu UI here
    }

    private void ResumeGame()
    {
        Debug.Log("SceneEffects: ResumeGame");
        Time.timeScale = 1;
        // Optionally, hide the pause menu UI here
    }

    public void SpawnParticle(int index, Vector3 position)
    {
        if (index >= 0 && index < particlePrefabs.Count)
        {
            Instantiate(particlePrefabs[index], position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid particle prefab index: " + index);
        }
    }
}
