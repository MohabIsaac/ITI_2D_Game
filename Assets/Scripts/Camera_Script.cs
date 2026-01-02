using UnityEngine;

public class Camera_Script : MonoBehaviour
{
    [SerializeField] private Camera target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        Vector3 rot = new Vector3(0,0,0);
        target.transform.rotation = Quaternion.Euler(rot);
     
}

}
