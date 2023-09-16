﻿using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleCraft.UI{
	public class PauseMenu : MonoBehaviour {

		public void Quit(){
			Application.Quit();
		}

		public void Restart(){
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}
}