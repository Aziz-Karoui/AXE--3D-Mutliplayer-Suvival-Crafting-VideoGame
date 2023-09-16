using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuManager : MonoBehaviour
{

    public GameObject SettingPanel;
    public GameObject ProfilePanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void settingOpen()
    {
        SettingPanel.SetActive(true);
    }
    public void settingClose()
    {
        SettingPanel.SetActive(false);
    }
    public void ProfileOpen()
    {
        ProfilePanel.SetActive(true);
    }
    public void ProfileClose()
    {
        ProfilePanel.SetActive(false);
    }
}
