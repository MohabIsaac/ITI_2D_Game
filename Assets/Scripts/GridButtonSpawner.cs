using UnityEngine;

public class GridButtonSpawner : MonoBehaviour
{
    public Transform gridParent;      // Object with GridLayoutGroup
    public GameObject buttonPrefab;

    public int buttonCount = 0;

    void Start()
    {
        GenerateButtons();
    }

    public void GenerateButtons()
    {
        // Clear old buttons
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.GetChild(i).gameObject);
        }

        // Spawn new buttons
        for (int i = 0; i < buttonCount; i++)
        {
            Instantiate(buttonPrefab, gridParent);
        }
    }
}
