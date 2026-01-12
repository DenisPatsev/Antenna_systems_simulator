using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader
{
   private ICoroutineRunner _coroutineRunner;

   public SceneLoader(ICoroutineRunner coroutineRunner)
   {
      _coroutineRunner = coroutineRunner;
   }

   public void LoadScene(string sceneName, UnityAction onLoaded = null)
   {
      _coroutineRunner.StartCoroutine(Load(sceneName, onLoaded));
   }
   
   private IEnumerator Load(string sceneName, UnityAction onLoaded = null)
   {
      AsyncOperation wait = SceneManager.LoadSceneAsync(sceneName);
      
      while (!wait.isDone)
         yield return null;
      
      onLoaded?.Invoke();
   }
}