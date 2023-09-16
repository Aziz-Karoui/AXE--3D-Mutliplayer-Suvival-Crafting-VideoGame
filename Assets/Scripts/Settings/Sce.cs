using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Sce : NetworkBehaviour
{
    public GameObject imagedie;
    public GameObject imagewin;

    public float health;
    // Start is called before the first frame update
    void Start()
    {
     

        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerHunger playerHealth = gameObject.GetComponent<PlayerHunger>();
            health = playerHealth.health;

               if (health <= 0)
                {
                    if(isLocalPlayer)
                    {
                        DieClientRpc(); // serveur to client
                    }else
                    {
                        WinClientRpc();
                    }
                
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    if(isLocalPlayer)
                    {
                        DieClientRpc(); 
                    }else
                    {
                        WinClientRpc();
                    }
                
                }
            
     
    }
    public bool KeyA(float health)
    {   
        if(health <= 0)
        {
            Input.GetKeyDown(KeyCode.A);    
            Input.GetKeyUp(KeyCode.A);
            return true;
        }
        return false;
    }

    [ClientRpc]
    void WinClientRpc() 
    { 
        imagewin.SetActive(true);      
    }
    [ClientRpc]
    void DieClientRpc() 
    { 
        imagedie.SetActive(true);      
    }

    public void BackToMenu()
    {
         SceneManager.LoadScene(1);
    }
}
