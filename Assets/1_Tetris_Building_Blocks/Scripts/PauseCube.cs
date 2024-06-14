using UnityEngine;
using UnityEngine.SceneManagement;

public class CubePause : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject CloseInfo;

    void Start()
    {
        // Optionally, you can initialize references here if needed
    }

    public void Pause()
    {
        // Set the time scale to 0 to pause the game
        PauseMenu.SetActive(true);
        

        // Find all objects with CubeMovement and set their rigidbodies to kinematic
        CubeMovement[] cubeMovements = FindObjectsOfType<CubeMovement>();
        foreach (CubeMovement cubeMovement in cubeMovements)
        {
            cubeMovement.SetKinematic(true);
            cubeMovement.EnableGravity(false);
        }
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        

        // Find all objects with CubeMovement and unset their rigidbodies from kinematic and enable gravity
        CubeMovement[] cubeMovements = FindObjectsOfType<CubeMovement>();
        foreach (CubeMovement cubeMovement in cubeMovements)
        {
            cubeMovement.SetKinematic(false);
            cubeMovement.EnableGravity(true);
        }
    }

    public void Home()
    {
       
       SceneManager.LoadScene("1-Scene");
    }

    public void Close()
    {
        CloseInfo.SetActive(false);
    }
}
