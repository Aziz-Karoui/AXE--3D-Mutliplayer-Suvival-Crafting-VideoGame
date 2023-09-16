using UnityEngine;
using System.Collections;

public class ActivateAfterDelay : MonoBehaviour
{
    public GameObject objectToActivate;
    public float onTime = 60f; 
    public float offTime = 120f; 

    IEnumerator Start()
    {
        while (true) 
        {
            objectToActivate.SetActive(false); 
            yield return new WaitForSeconds(offTime);

            objectToActivate.SetActive(true); 
            yield return new WaitForSeconds(onTime); 
        }
    }
}
