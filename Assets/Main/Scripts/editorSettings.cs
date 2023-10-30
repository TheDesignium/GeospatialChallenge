using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Immersal.Samples.ContentPlacement;

[ExecuteInEditMode]
public class editorSettings : MonoBehaviour
{

    public EditorCameraMovement ecm;

    void Start()
    {
      #if UNITY_EDITOR
        ecm.enabled = true;
      #endif
      #if !UNITY_EDITOR
        ecm.enabled = false;
      #endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
