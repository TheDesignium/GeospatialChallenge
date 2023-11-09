using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{

    public void loadScene(string _s)
    {
        StartCoroutine(loadNext(_s));
    }

    IEnumerator loadNext(string _s)
    {
      yield return new WaitForSeconds(1);
      SceneManager.LoadSceneAsync(_s);
    }
}
