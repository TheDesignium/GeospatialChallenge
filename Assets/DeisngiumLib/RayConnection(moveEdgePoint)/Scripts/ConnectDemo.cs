using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Deisgnium.RayConnection
{
  public class ConnectDemo : MonoBehaviour
  {
      [SerializeField] private Camera _cam;
      [SerializeField] private GameObject _connectorPrefab;
      [SerializeField] private GameObject Cursor;

      // draw connection
      private int _nodeMaxNums = 2;
      [SerializeField]
      private int _count = 0;
      private List<GameObject> _connectors = null;
      public List<GameObject> connectors => _connectors;
      private GameObject _currentLineObj = null;
      private LineNode _currentLine = null;

      private float touchTime;
      private float m_Offset = 0.03f;

      void Start()
      {
#if UNITY_EDITOR
         // incase frame rate is too hight on Eidtor, the item pops up timing is wrong
         Application.targetFrameRate = 60;
#endif
      }

      void OnDestroy()
      {
        _connectors.ReleaseGameObjectList();
      }


      void Update()
      {

        if (Input.GetMouseButtonDown(0))
        {
          Vector2 mousePos = Input.mousePosition;
          Ray ray = _cam.ViewportPointToRay(_cam.ScreenToViewportPoint(mousePos));
          if (Time.time - touchTime > 0.3f)
          {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000))
            {
              Vector3 plane = Vector3.ProjectOnPlane(Vector3.forward + Vector3.right, hit.normal);
              Cursor.transform.position = hit.point + hit.normal * m_Offset;
              Cursor.transform.localRotation = Quaternion.LookRotation(plane, hit.normal);
            }
            DrawCheck();
            touchTime = Time.time;
          }
        }

      }

      void FixedUpdate()
      {
        if (_currentLine != null)
        {
          _currentLine.UpdateDraw(Cursor.transform.position);
        }
      }

      private void DrawCheck()
      {
        if(_count < _nodeMaxNums) {
          AddNode();
        }
        if(_count ==  _nodeMaxNums)
        {
          EndNode();
        }

        // animation the cursor for hinting behaviour
        Cursor.transform.DOScale(0.2f, 0.15f)
                        .SetLoops(2, LoopType.Yoyo)
                        .SetRelative(true)
                        .SetEase(Ease.OutCubic);
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

      }

      private void EndNode()
      {
        print ("End Node");
        _currentLine.EndNode();
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
      }
  }

}
