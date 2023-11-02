using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killself : MonoBehaviour
{

    public float _wait;

    void Start()
    {
        StartCoroutine(dienow());
    }

    IEnumerator dienow()
    {
        yield return new WaitForSeconds(_wait);
        Destroy(gameObject);
    }
}
