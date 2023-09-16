using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitEnter : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("fruit"))
    {
        Debug.Log("Player collided with a fruit!");
    }
}
}
