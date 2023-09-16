using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class ImageUploadButton : MonoBehaviour
{
    public GameObject PanelView;
    public Image myAvatar;
    public Image Avatar1;
    public Image Avatar2;
    public Image Avatar3;

    
    
    public void OpenPanelView()
    {
        PanelView.SetActive(true);
    }
    
    public void changeAvatar1(){
        myAvatar.sprite = Avatar1.sprite;
    }
     public void changeAvatar2(){
        myAvatar.sprite = Avatar2.sprite;
    }
     public void changeAvatar3(){
        myAvatar.sprite = Avatar3.sprite;
        
    }
    public void ClosePanelView()
    {
        PanelView.SetActive(false);
    }
}
