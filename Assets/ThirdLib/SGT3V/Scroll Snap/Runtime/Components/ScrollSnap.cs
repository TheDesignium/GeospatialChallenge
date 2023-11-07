/* 
    Copyright (C) 2021 SGT3V, Sercan Altundas
    
    Visit for details: https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap
*/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SGT3V.ScrollSnap
{
    [HelpURL("https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap/api#scrollsnap.cs")]
    [SelectionBase]
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Scroll Snap/Scroll Snap Basic")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ScrollSnap : ScrollSnapBase
    {
        /// <summary>
        ///     Index of the current page in the <see cref="Content"/>.
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return currentPageIndex;
            }
            set
            {
                currentPageIndex = Mathf.Clamp(value, 0, Content.childCount - 1);
            }
        }
        private int currentPageIndex;
        
        /// <summary>
        ///     Size of the side areas in pixels to scroll out of the first and the last pages.
        /// </summary>
        [Min(0)] public float ScrollOutMargin = 100;

        private readonly Color ScrollOutMarginGizmoColor = Color.red;

        private readonly Color SnapAreaSizeGizmoColor = Color.blue;

        private float fullWidth, fullHeight;

        private int Direction => IsHorizontal ? (IsLeftAligned ? -1 : 1) : (IsBottomAligned ? -1 : 1);

        protected override void SetPageSizes()
        {
            RectTransform rt = transform as RectTransform;
            if(rt == null) return; 
            
            Rect rect = rt.rect;
            
            PageWidth = rect.width;
            PageHeight = rect.height;

            int count = Content.childCount;

            for (int i = 0; i < count; i++)
            {
                RectTransform child = Content.GetChild(i) as RectTransform;
                if(child == null) return; 
                
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PageWidth);
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PageHeight);
            }

            fullWidth = (count - 1) * PageWidth;
            fullHeight = (count - 1) * PageHeight;
        }

        protected override void SetPagePositions()
        {
            int count = Content.childCount;
            for (int i = 0; i < count; i++)
            {
                RectTransform rect = Content.GetChild(i) as RectTransform;
                if (rect == null) return;
                
                if (IsHorizontal)
                {
                    int direction = IsLeftAligned ? 1 : -1;
                    rect.anchoredPosition = new Vector2(i * PageWidth * direction, 0);
                }
                else
                {
                    int direction = IsBottomAligned ? 1 : -1;
                    rect.anchoredPosition = new Vector2(0, i * PageHeight * direction);
                }
            }

            fullWidth = (count - 1) * PageWidth;
            fullHeight = (count - 1) * PageHeight;
        }

        protected override void SetContentPosition()
        {
            float value = currentPageIndex * PageSize * Direction;
            float x = IsHorizontal ? value : 0;
            float y = IsHorizontal ? 0 : value;
            Content.anchoredPosition = new Vector2(x, y);
        }

        protected override void SnapToPage()
        {
            if (!isDragged)
            {
                float value = Mathf.SmoothStep(AnchoredPosition, currentPageIndex * PageSize * Direction, SnapSpeed * 0.2f);
                float x = IsHorizontal ? value : 0;
                float y = IsHorizontal ? 0 : value;
                Content.anchoredPosition = new Vector2(x, y);
            }

            ScrollAmount = Math.Abs((float)Math.Round(-AnchoredPosition / PageSize, 2));
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (IsContentNull()) return;

            isDragged = false;
            SetCurrentPageIndex();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (IsContentNull()) return;

            isDragged = true;
            ProcessDrag(eventData.delta);
            StopDrag();
        }

        private void StopDrag()
        {
            float fullSize = IsHorizontal ? fullWidth : fullHeight;

            if (AnchoredPosition * -Direction >= ScrollOutMargin)
            {
                Content.anchoredPosition = IsHorizontal ? new Vector2(ScrollOutMargin * -Direction, 0) : new Vector2(0, ScrollOutMargin * -Direction);
            }

            if (Direction * AnchoredPosition >= fullSize + ScrollOutMargin)
            {
                Content.anchoredPosition = IsHorizontal ? new Vector2(Direction * (fullSize + ScrollOutMargin), 0) : new Vector2(0, Direction * (fullSize + ScrollOutMargin));
            }
        }

        protected override void ProcessDrag(Vector2 delta)
        {
            float x = IsHorizontal ? delta.x / Screen.width * PageSize : 0;
            float y = IsHorizontal ? 0 : delta.y / Screen.width * PageSize;
            Content.anchoredPosition += new Vector2(x, y);

            dragDirection = IsHorizontal ? (delta.x > 0 ? 1 : -1) : (delta.y > 0 ? 1 : -1);
        }

        protected override void SetCurrentPageIndex()
        {
            int newIndex = Mathf.Clamp(Direction * Mathf.RoundToInt((AnchoredPosition + SnapAreaSize / 2 * dragDirection) / PageSize), 0, Content.childCount - 1);

            if (currentPageIndex != newIndex)
            {
                currentPageIndex = newIndex;
                OnPageChanged?.Invoke(newIndex);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            RectTransform rect = (transform as RectTransform);
            
            if (rect == null) return;
            
            Vector2 pivot = rect.pivot;
            
            Gizmos.matrix = rect.localToWorldMatrix;
            Gizmos.color = ScrollOutMarginGizmoColor;

            if (IsHorizontal)
            {
                Vector2 size = new Vector2(ScrollOutMargin - 2, PageHeight - 2);
                Vector2 centerLeft = new Vector2((ScrollOutMargin - PageWidth) / 2 - (pivot.x - 0.5f) * PageWidth, (pivot.y - 0.5f) * -PageHeight);
                Vector2 centerRight = new Vector2((PageWidth - ScrollOutMargin) / 2 - (pivot.x - 0.5f) * PageWidth, (pivot.y - 0.5f) * -PageHeight);

                Gizmos.DrawWireCube(centerLeft, size);
                Gizmos.DrawWireCube(centerRight, size);
            }
            else
            {
                Vector2 size = new Vector2(PageWidth - 2, ScrollOutMargin - 2);
                Vector2 centerTop = new Vector2((pivot.x - 0.5f) * -PageWidth, (PageHeight - ScrollOutMargin) / 2 - (pivot.y - 0.5f) * PageHeight);
                Vector2 centerBottom = new Vector2((pivot.x - 0.5f) * -PageWidth, (ScrollOutMargin - PageHeight) / 2 - (pivot.y - 0.5f) * PageHeight);

                Gizmos.DrawWireCube(centerTop, size);
                Gizmos.DrawWireCube(centerBottom, size);
            }

            Gizmos.color = SnapAreaSizeGizmoColor;

            Vector2 center = new Vector2((rect.pivot.x - 0.5f) * -PageWidth, (pivot.y - 0.5f) * -PageHeight);
            Vector2 areaSize = new Vector2(IsHorizontal ? SnapAreaSize : PageWidth - 2, IsHorizontal ? PageHeight : SnapAreaSize - 2);
            Gizmos.DrawWireCube(center, areaSize);
        }
    }
}
