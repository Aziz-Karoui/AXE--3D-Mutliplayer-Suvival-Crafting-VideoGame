using UnityEngine;
using UnityEngine.UI;

namespace SimpleCraft.UI{
    public class QuickMessage : MonoBehaviour{
        Text _Message;

        [SerializeField]
        private GameObject _panel;

        void Start(){
            _Message = this.GetComponent<Text>();
        }

        /// <param name="message"></param>
        /// <param name="seconds"></param>
        public void ShowMessage(string message,int seconds = 2){
            _panel.SetActive(true);
            _Message.text = message;
            CancelInvoke();
            Invoke("ClearMessage", seconds);
        }

        public void ClearMessage(){
            _panel.SetActive(false);
            _Message.text = "";
        }
    }	
}
