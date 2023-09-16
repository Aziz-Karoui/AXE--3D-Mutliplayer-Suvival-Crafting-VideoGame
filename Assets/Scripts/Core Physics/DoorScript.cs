using UnityEngine;
public class DoorScript : MonoBehaviour
{

    public string boxChildName = "boxy1";
    private GameObject boxChild;
    private GameObject boxObject;
    private bool isRotated = false;

    void Start()
    {
        // if box1 true 90 if box2&true wait 2sec false -90 
        // wal 3aksou bel 3aks 
    }

    void Update()
    {
            boxChild = GameObject.Find(boxChildName);
            if (boxChild != null)
            {
                
                boxObject=  boxChild.transform.parent.gameObject;
            }
        

        if (Input.GetKeyDown(KeyCode.Y) && isRotated == true)
        {
            if (boxObject != null)
            {
                boxObject.transform.Rotate(new Vector3(0, 90, 0));
                isRotated = !isRotated; // 7el
            }
        }
        else if (Input.GetKeyDown(KeyCode.Y) && isRotated == false)
        {
            if (boxObject != null)
            {
                boxObject.transform.Rotate(new Vector3(0, -90, 0));
                isRotated = !isRotated; // sakker
            }
        }
    }
}
