using UnityEngine;
using UnityEngine.InputSystem;

public class Color_Switch : MonoBehaviour
{
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            GetComponent<Renderer>().material = redMaterial;
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            GetComponent<Renderer>().material = blueMaterial;
        }
    }
}
