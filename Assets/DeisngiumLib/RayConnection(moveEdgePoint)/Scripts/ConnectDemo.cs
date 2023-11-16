using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using TMPro;

namespace Deisgnium.RayConnection
{
  public class ConnectDemo : MonoBehaviour
  {
      [SerializeField] private Camera _cam;
      [SerializeField] private GameObject _connectorPrefab;
      [SerializeField] private GameObject Cursor;

      public GameObject[] objectToSpawn;
      public GameObject debugObject;
      public GameObject taptoplace;

        // draw connection
        public int _nodeMaxNums = 2;
      public int maxObjects = 20;
      public float distanceLimit;
      [SerializeField]
      private int _count = 0;
      public List<GameObject> _connectors = null;
      public List<GameObject> connectors => _connectors;
      private GameObject _currentLineObj = null;
      private LineNode _currentLine = null;

      private float touchTime;
      private float m_Offset = 0.03f;
      float distance;

      public Vector3 startPosition;
      public Vector3 endPosition;

      public TMP_Text textUI;

      public float angleThreshold = 45f;

      List<GameObject> objects = new List<GameObject>();

      void Start()
      {
#if UNITY_EDITOR
         // incase frame rate is too hight on Eidtor, the item pops up timing is wrong
         Application.targetFrameRate = 60;
#endif
#if !UNITY_EDITOR
         // incase frame rate is too hight on Eidtor, the item pops up timing is wrong
         Application.targetFrameRate = 30;
         debugObject.SetActive(false);
#endif
        }

      void OnDestroy()
      {
        _connectors.ReleaseGameObjectList();
      }


      void Update()
      {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
          Vector2 mousePos = Input.mousePosition;

          Ray ray = _cam.ScreenPointToRay(mousePos);
          RaycastHit hit;
          if (Physics.Raycast(ray, out hit, Mathf.Infinity))
          {
              Vector3 hp = hit.point;

              if (hit.collider != null)
              {
                  float angle = Vector3.Angle(hit.normal, Vector3.up);

                  if (angle < angleThreshold)
                  {
                    Debug.Log("Hit a horizontal surface");
                    SpawnObject(hit.point);
                  }
                  else
                  {
                    Debug.Log("Hit a vertical surface");
                    if (Time.time - touchTime > 0.3f)
                    {
                        Cursor.transform.position = hp;
                        if(_count > 0)
                        {
                          float distance = Vector3.Distance(startPosition, hp);
                          Debug.Log(distance);
                          if(distance < distanceLimit)
                          {
                              DrawCheck(hp);
                          }
                          else
                          {
                            StopCoroutine("fadeOutUI");
                            StartCoroutine("fadeOutUI");
                          }
                        }
                        else
                        {
                          DrawCheck(hp);
                        }
                        touchTime = Time.time;
                    }
                  }
              }
          }
        }
#endif
#if !UNITY_EDITOR

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = _cam.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity) && Input.GetTouch(0).position.y > 300)
                    {
                        Vector3 hp = hit.point;

                        if (hit.collider != null)
                        {
                            taptoplace.SetActive(false);

                            float angle = Vector3.Angle(hit.normal, Vector3.up);

                            if (angle < angleThreshold)
                            {
                              Debug.Log("Hit a horizontal surface");
                              SpawnObject(hit.point);
                            }
                            else
                            {
                              Debug.Log("Hit a vertical surface");
                              if (Time.time - touchTime > 0.3f)
                              {
                                  Cursor.transform.position = hp;
                                  if(_count > 0)
                                  {
                                    float distance = Vector3.Distance(startPosition, hp);
                                    Debug.Log(distance);
                                    if(distance < distanceLimit)
                                    {
                                        DrawCheck(hp);
                                    }
                                    else
                                    {
                                      StopCoroutine("fadeOutUI");
                                      StartCoroutine("fadeOutUI");
                                    }
                                  }
                                  else
                                  {
                                    DrawCheck(hp);
                                  }
                                  touchTime = Time.time;
                              }
                            }
                        }
                    }
                }
            }
#endif

        }

        void SpawnObject(Vector3 position)
        {
          Vector3 lookPos = new Vector3(Camera.main.transform.position.x, position.y, Camera.main.transform.position.z);
          Vector3 relativePos = lookPos - position;
          Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
          int rando = Random.Range(0, objectToSpawn.Count());
          GameObject g = Instantiate(objectToSpawn[rando], position, rotation);
          objects.Add(g);
          float randoY = Random.Range(-50, 50);
          Vector3 v3 = g.transform.eulerAngles;
          v3.y += randoY;
          g.transform.eulerAngles = v3;
          resetTouch();
        }

      void FixedUpdate()
      {
        if (_currentLine != null)
        {
          _currentLine.UpdateDraw(Cursor.transform.position);
        }
      }

      private void DrawCheck(Vector3 hp)
      {
        Debug.Log(hp);

        if(_count == 0)
        {
          startPosition = hp;
        }

        if(_count < _nodeMaxNums)
        {
          AddNode();
        }
        if(_count == _nodeMaxNums)
        {
          endPosition = hp;
          distance = Vector3.Distance(startPosition, endPosition);
          Debug.Log(distance);
          if(distance < distanceLimit)
          {
              EndNode();
          }
        }
/*
        // animation the cursor for hinting behaviour
        Cursor.transform.DOScale(0.2f, 0.15f)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetRelative(true)
                        .SetEase(Ease.OutCubic);
*/
      }

      private void AddNode()
      {
        if (_connectors == null)  _connectors = new List<GameObject>();

        // no line object, create one
        if (_currentLine == null)
        {
          GameObject con = Instantiate(_connectorPrefab, Vector3.zero, Quaternion.identity);
          con.transform.SetParent(this.transform, false);
          con.SetActive(true);
          _connectors.Add(con);
          _currentLineObj = con;
          if (_currentLineObj.TryGetComponent<LineNode>(out _currentLine))
          {
            _count = 0;
            print ("get lineNode");
          }
        }

        // add node into line
        if (_currentLine != null)
        {
          _currentLine.AddNode(Cursor);
          print ("Add Node");
          _count ++;
        }

        if (_connectors.Count >= maxObjects)
        {
            // Remove and delete the oldest GameObject (first in the list)
            GameObject toRemove = _connectors[0];
            _connectors.RemoveAt(0); // Removes the first element
            Destroy(toRemove); // Deletes the GameObject from the scene
        }

      }

      private void EndNode()
      {
        print ("End Node");
        _currentLine.EndNode();

        _currentLineObj.transform.GetChild(0).gameObject.GetComponent<LineRendererParticleEmitter>().setUp(distance, distanceLimit);

        _currentLineObj = null;
        _currentLine = null;
        _count = 0;
      }

      public void resetTouch()
      {
        _currentLineObj = null;
        _currentLine = null;
        _count = 0;
      }

      public void CleanConnections()
      {
        _connectors.ReleaseGameObjectList();
        _currentLineObj = null;
        _currentLine = null;
        _count = 0;
        foreach(GameObject g in objects)
        {
          Destroy(g);
        }
        objects.Clear();
      }

      IEnumerator fadeOutUI()
      {
        Color textcolour = Color.white;
        float alph = textcolour.a;
        textUI.color = textcolour;
        textUI.gameObject.SetActive(true);

        while(alph > 0)
        {
          alph = textcolour.a;
          alph -= 0.01f;
          textcolour.a = alph;
          textUI.color = textcolour;
          yield return new WaitForEndOfFrame();
        }
        textUI.gameObject.SetActive(false);
      }
  }

}
