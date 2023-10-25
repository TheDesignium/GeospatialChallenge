using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deisgnium.RayConnection
{
  [System.Serializable]
  public class ObjTag
  {
    public string name;
    public GameObject obj;
  }

  [CreateAssetMenu(fileName = "ObjResource", menuName = "ScriptableObjects/ObjResource")]
  public class ObjResource : ScriptableObject
  {
    public string category;
    public List<ObjTag> objlist;
  }

}
