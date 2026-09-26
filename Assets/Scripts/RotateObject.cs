using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public Vector3 rotation;
    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
