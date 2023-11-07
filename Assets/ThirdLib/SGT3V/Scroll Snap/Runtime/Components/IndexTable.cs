/* 
    Copyright (C) 2021 SGT3V, Sercan Altundas
    
    Visit for details: https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace SGT3V.ScrollSnap
{
    [HelpURL("https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap/api#indextable.cs")]
    [SelectionBase]
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Scroll Snap/Index Table")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class IndexTable : UIBehaviour
    {
        /// <summary>
        ///     The parent Scroll Snap component, that the Index Table will work with.
        /// </summary>
        public ScrollSnap ScrollSnap;
        
        /// <summary>
        ///     The size of the Toggle by height.
        /// </summary>
        [Min(3)] public float ToggleSize;
        
        /// <summary>
        ///     The padding between the buttons of the Index Table.
        /// </summary>
        [Min(0)] public float TogglePadding;

        private Toggle[] toggles;

        private new IEnumerator Start()
        {
            base.Start();
            
            ScrollSnap.OnPageChanged.AddListener(OnPageChanged);

            yield return null;
            
            SetUpToggles();
        }

        private void SetUpToggles()
        {
            toggles = GetComponentsInChildren<Toggle>();

            for (var i = 0; i < toggles.Length; i++)
            {
                var index = i;
                toggles[index].onValueChanged.AddListener((active) => {
                    if (active)
                    {
                        SetSnapScrollIndex(index);
                    }
                });
            }
        }

        private void OnPageChanged(int index)
        {
            toggles[index].isOn = true;
        }

        private void SetSnapScrollIndex(int index)
        {
            ScrollSnap.CurrentPageIndex = index;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            
            ScrollSnap.OnPageChanged.RemoveListener(OnPageChanged);
            foreach (var toggle in toggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }
    }
}
