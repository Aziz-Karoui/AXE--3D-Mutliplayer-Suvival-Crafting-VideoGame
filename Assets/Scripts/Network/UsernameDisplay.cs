using UnityEngine;
using UnityEngine.UI;

public class UsernameDisplay : MonoBehaviour
{
    public Text textComponent;

    void Start()
    {
       textComponent.text = SessionData.username;
    }
}
