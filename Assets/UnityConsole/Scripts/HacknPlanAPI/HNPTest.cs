using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//5a1b44d8a798498b9058d0953c63d57c
public class HNPTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PostBugToAPI());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PostBugToAPI()
    {
        var webRequest = UnityWebRequest.Get("https://api.hacknplan.com/v0/projects");
        SetRequestHeaders(ref webRequest);

        yield return webRequest.SendWebRequest();

        string reponse = webRequest.downloadHandler.text;
        Debug.Log(reponse);
    }

    /// <summary>
    /// Set the Headers WebRequest
    /// Sets the content type and Authorization headers
    /// </summary>
    private void SetRequestHeaders(ref UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "ApiKey {API KEY}");
    }
    
    
}
