using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deisgnium.RayConnection
{
  #region Object utilities

  //-----------------------------------//
  //             Object Utility        //
  //-----------------------------------//
  static class ObjectUtilExtensions
  {
      public static void Destroy(this UnityEngine.Object o)
      {
          if (o == null) return;
          if (Application.isPlaying)
              UnityEngine.Object.Destroy(o);
          else
              UnityEngine.Object.DestroyImmediate(o);
      }

      public static void ReleaseGameObjectList(this List<GameObject> objList)
      {
          if (objList == null) return;

          for (int i=objList.Count-1; i>=0; i--)
          {
              Destroy(objList[i]);
              objList.RemoveAt(i);
          }
          objList = null;
      }
  }

  #endregion
}
