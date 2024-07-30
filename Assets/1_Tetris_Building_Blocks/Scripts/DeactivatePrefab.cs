using UnityEngine;

public class DeactivatePrefab : MonoBehaviour
{
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}