using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class staonmeRot : MonoBehaviour
{
    public Transform _target;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _target.position;
        transform.rotation = _target.rotation;
    }
}
