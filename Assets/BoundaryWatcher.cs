using UnityEngine;
using System.Collections.Generic;
using TMPro; // Make sure to include this for using TextMeshPro

public class BoundaryWatcher : MonoBehaviour
{
    public GameObject boundaryCube;
    public GameObject PauseMenu;
    public TextMeshProUGUI textMeshPro; // Reference to the TextMeshProUGUI component for displaying messages
    private AudioSource audioSource;
    private AudioManager1 audioManager;

    private float yBoundary;
    private Dictionary<GameObject, float> cubeTimer = new Dictionary<GameObject, float>();

    void Start()
    {
        if (boundaryCube != null)
        {
            yBoundary = boundaryCube.transform.position.y + boundaryCube.transform.localScale.y / 2.0f;
        }
        else
        {
            Debug.LogError("Boundary Cube not set or found.");
        }
    }

    void Update()
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            // Check if the cube is above the boundary
            if (cube.transform.position.y > yBoundary)
            {
                // If the cube is already being tracked, increment its timer
                if (cubeTimer.ContainsKey(cube))
                {
                    cubeTimer[cube] += Time.deltaTime;
                }
                else
                {
                    // Start tracking a new cube
                    cubeTimer[cube] = 0;
                }

                // Check if the cube has been above the boundary for more than 5 seconds
                if (cubeTimer[cube] >= 5.0f)
                {
                    Debug.Log("Game Over: A 'Cube' has crossed the y-axis boundary at " + yBoundary);
                    textMeshPro.text = "You Lose!"; // Display message
                    Time.timeScale = 0f; // Stop the game
                    AudioManager1.Instance.PlayMusic("Theme4");
                    PauseMenu.SetActive(true);
                    break; // Exit loop after the first detection
                }
            }
            else
            {
                // If the cube is no longer above the boundary, remove it from the dictionary
                if (cubeTimer.ContainsKey(cube))
                {
                    cubeTimer.Remove(cube);
                }
            }
        }
    }
}
