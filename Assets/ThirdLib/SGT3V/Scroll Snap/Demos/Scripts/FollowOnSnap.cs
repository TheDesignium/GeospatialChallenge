using UnityEngine;

namespace SGT3V.ScrollSnap.Demo
{
    public class FollowOnSnap : MonoBehaviour
    {
        [SerializeField] private ScrollSnap follower;

        private void Start()
        {
            follower = GetComponent<ScrollSnap>();
        }

        public void OnPageChanged(int index)
        {
            follower.CurrentPageIndex = index;
        }
    }
}
