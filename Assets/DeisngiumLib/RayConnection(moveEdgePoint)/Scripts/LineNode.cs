using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deisgnium.RayConnection
{
  public class LineNode : MonoBehaviour
  {
      [SerializeField] private LineRenderDraw _lineRenderer;
      [SerializeField] private GameObject _nodePrefab;

      [SerializeField] private Rope _displayRope;
      [SerializeField] private bool _lineShowEnableAfterEnd = false;
      [SerializeField] private bool _keepUpdateEdgePoint = false;

      private bool _updateLine = false;
      private int _nodeMaxNums = 1;
      [SerializeField] private int _count = 0;
      [SerializeField] private List<GameObject> _nodeObjs;
      private List<Vector3> _linePoints;

      void Start()
      {

      }

      void FixedUpdate()
      {
        if (_nodeObjs.Count > 0 && _keepUpdateEdgePoint)
        {
          _displayRope.UpdateStartPt(_nodeObjs[0].transform.position);
          _displayRope.UpdateEndPt(_nodeObjs[_nodeObjs.Count - 1].transform.position);
        }
      }

      void OnDestroy()
      {
        _nodeObjs.ReleaseGameObjectList();
      }

      public void UpdateDraw(Vector3 point)
      {
        if (_nodeObjs.Count >= 1)
        {
          _linePoints = new List<Vector3>();
          for (int i=0; i< _nodeObjs.Count; i++)
            _linePoints.Add(_nodeObjs[i].transform.position);
          _linePoints.Add(point);
          _lineRenderer.setLines(_linePoints);
          ShowLine(false);
        }
        else
        {
          ShowLine(false);
        }
      }

      public void AddNode(GameObject nodeObj)
      {
        if(_nodeObjs == null) _nodeObjs = new List<GameObject>();
        GameObject node = Instantiate(_nodePrefab, Vector3.zero, Quaternion.identity);
        node.name = $"node_{_count}";
        node.transform.SetParent(this.transform, false);
        node.transform.position = nodeObj.transform.position;
        node.transform.rotation = nodeObj.transform.rotation;
        _nodeObjs.Add(node);
        _count ++;
      }

      public void EndNode()
      {
        if (_nodeObjs.Count > 1)
        {
          _linePoints = new List<Vector3>();
          for (int i=0; i< _nodeObjs.Count; i++)
            _linePoints.Add(_nodeObjs[i].transform.position);
          _lineRenderer.setLines(_linePoints);

          // set Rope: start position, end position
          _displayRope.Set(_linePoints[0], _linePoints[_linePoints.Count - 1]);
          _displayRope.gameObject.SetActive(true);

          ShowLine(_lineShowEnableAfterEnd);
        }

        StartCoroutine(stopSim());
      }

      IEnumerator stopSim()
      {
              yield return new WaitForSeconds(5);
              _displayRope.switchOff();
              Debug.Log("disable rope");
      }

      public void ShowLine(bool enable)
      {
        _lineRenderer.setEnabled(enable);
      }

      public void ShowNode(bool enable)
      {
        foreach(GameObject node in _nodeObjs)
          node.SetActive(enable);
      }
  }
}
