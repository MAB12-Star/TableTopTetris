using UnityEngine;
using UnityEngine.UI;

public class DeactivateButton : MonoBehaviour
{
    private Button button;
    [SerializeField] GameObject HowToInfo;
    [SerializeField] GameObject objectToInstantiate; // Reference to the GameObject prefab you want to instantiate

    // Start is called before the first frame update
    void Start()
    {
        // Get the Button component attached to the same GameObject
        button = GetComponent<Button>();

        // Ensure the button component is found and add the OnClick listener
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogError("Button component not found on the GameObject.");
        }
    }

    // Method to be called when the button is clicked
    public void OnButtonClicked()
    {
        // Instantiate the specified GameObject prefab
        if (objectToInstantiate != null)
        {
            Instantiate(objectToInstantiate, transform.position, Quaternion.identity);
        }

        // Deactivate the button and disable its interactable state
        HowToInfo.SetActive(false);
    }

    public void DeactivateHowToInfo()
    {
        // Deactivate HowToInfo GameObject
        HowToInfo.SetActive(false);
    }
}
