﻿using UnityEngine;
using UnityEngine.UI;

namespace SimpleCraft.UI{    public class ActionText : MonoBehaviour{
        private Text _Text;

        [SerializeField]
        private GameObject _panel;
        
        private string _text;

        public string Text{
            set{
                _text = value;
                _Text.text = _text;
                _panel.SetActive(_text != "");
            }
        }

        void Start(){
            _Text = this.gameObject.GetComponent<Text>();
        }
    }
}