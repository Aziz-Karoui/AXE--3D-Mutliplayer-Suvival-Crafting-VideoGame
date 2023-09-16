using UnityEngine;

public class RotateLight : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        //x-axis
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}
 