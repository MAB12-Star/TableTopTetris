using UnityEngine;
using TMPro;

public class EncouragingWords : MonoBehaviour
{
    public string[] encouragingPhrases = { "WOW", "You're doing great!", "Keep it up!", "You got this!" };

    // Reference to the TextMeshPro component
    public TextMeshProUGUI textMeshPro;

    private void Start()
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("TextMeshPro reference not set. Searching for TextMeshPro in children...");
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void DisplayEncouragingWord()
    {
        if (encouragingPhrases.Length > 0)
        {
            int randomIndex = Random.Range(0, encouragingPhrases.Length);
            textMeshPro.text = encouragingPhrases[randomIndex];
        }
        else
        {
            Debug.LogWarning("No encouraging phrases available.");
        }
    }

    public void ClearText()
    {
        // Clear the text
        textMeshPro.text = string.Empty;
    }
}

