using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Button LoadSceneBtn;
    [SerializeField] private GameObject LoadingGO;
    [SerializeField] private GameObject UIMangerGO;

    private void Awake()
    {
        LoadSceneBtn.onClick.AddListener(OnLoadScene);
    }

    public void OnLoadScene()
    {
        StartCoroutine(IELoadScene());
    }
    
    public IEnumerator IELoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Gameplay", LoadSceneMode.Additive);

        operation.completed += OnLoadSceneComplete;

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

    public void OnLoadSceneComplete(AsyncOperation _handle)
    {
        Debug.Log("Load Complete");
        LoadingGO.SetActive(false);
        UIMangerGO.SetActive(true);
    }
    
}
