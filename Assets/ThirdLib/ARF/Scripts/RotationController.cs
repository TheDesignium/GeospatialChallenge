using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.ARFoundation.Samples
{

    public class RotationController : MonoBehaviour
    {

        public Transform target;
        Vector3 lua = new Vector3(0,0,0);
        public Slider slider;

        public Vector3 v3;

        public void SliderValueChanged(float _f)
        {
              var angle = _f;
              lua.y = angle;
              target.localEulerAngles = lua;
              v3 = target.eulerAngles;
        }

        void Awake()
        {

        }

        void OnEnable()
        {
            if (slider != null)
            {
              slider.value = target.rotation.y;
            }
        }

        void LateUpdate()
        {
          target.eulerAngles = v3;
        }

    }
}
