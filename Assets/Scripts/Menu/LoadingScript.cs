using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Slider LoadingBarFills;
    //public float speed;


    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {       

        
        LoadingScreen.SetActive(true);
        //yield return new WaitForSeconds(1);   
        LoadingBarFills.value = 0;
            yield return new WaitForSeconds(1);
            LoadingBarFills.value = 0.3f;
            yield return new WaitForSeconds(1);
            LoadingBarFills.value = 0.9f;
            yield return new WaitForSeconds(1);
            LoadingBarFills.value = 1;
            //yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);   
        
        while(!operation.isDone)
        {   
            /*Debug.Log("Loading progress: " + operation.progress);
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFills.value = progressValue;*/


            yield return null;
        }
        
        SceneManager.LoadScene(sceneId);
    }
}