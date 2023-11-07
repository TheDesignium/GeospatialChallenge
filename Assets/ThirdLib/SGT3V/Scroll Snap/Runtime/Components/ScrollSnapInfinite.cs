/* 
    Copyright (C) 2021 SGT3V, Sercan Altundas
    
    Visit for details: https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap
*/

using System;
using UnityEngine;
using SGT3V.Common;
using UnityEngine.EventSystems;

namespace SGT3V.ScrollSnap
{
    [HelpURL("https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap/api#scrollsnapinfinite.cs")]
    [SelectionBase]
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Scroll Snap/Scroll Snap Infinite")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class ScrollSnapInfinite : ScrollSnapBase
    {
        /// <summary>
        ///     Index of the current page in the <see cref="Content"/>.
        /// </summary>
        public int CurrentPageIndex
        { 
            get 
            {
                return (currentPageIndex * AlignmentDirection).Mod(Content.childCount);
            }  
            set 
            {
                currentPageIndex = value;
            } 
        }
        private int currentPageIndex;

        private readonly Color SnapAreaSizeGizmoColor = Color.red;

        private int AlignmentDirection => IsHorizontal ? (IsLeftAligned ? 1 : -1) : (IsBottomAligned ? 1 : -1);

        // if infinite scroll send another index
        private int infiniteScrollPivotStart, infiniteScrollPivotEnd;

        /// <summary>
        ///     Resets the page size and position.
        /// </summary>
        protected override void SetPageSizes()
        {
            RectTransform rt = transform as RectTransform;
            if(rt == null) return; 
            
            Rect rect = rt.rect;
            
            PageWidth = rect.width;
            PageHeight = rect.height;

            infiniteScrollPivotStart = AlignmentDirection > 0 ? 0 : 1;
            infiniteScrollPivotEnd = AlignmentDirection > 0 ? (Content.childCount - 1) : (2 - Content.childCount);

            for (int i = 0; i < Content.childCount; i++)
            {
                RectTransform child = Content.GetChild(i) as RectTransform;
                if(child == null) return; 
                
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, PageWidth);
                child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, PageHeight);
            }
        }

        protected override void SetPagePositions()
        {
            for (int i = 0; i < Content.childCount; i++)
            {
                RectTransform rect = Content.GetChild(i) as RectTransform;
                if (rect == null) return;
                
                if (IsHorizontal)
                {
                    float x = Mathf.Abs((i + currentPageIndex) * PageWidth * AlignmentDirection) * AlignmentDirection;
                    rect.anchoredPosition = new Vector2(x, 0);
                }
                else
                {
                    rect.anchoredPosition = new Vector2(0, (i + currentPageIndex) * PageHeight * AlignmentDirection);
                }
            }
        }

        protected override void SetContentPosition()
        {
            float value = currentPageIndex * PageSize * -AlignmentDirection;
            float x = IsHorizontal ? value : 0;
            float y = IsHorizontal ? 0 : value;
            Content.anchoredPosition = new Vector2(x, y);
        }

        protected override void SnapToPage()
        {
            if (!isDragged)
            {
                float value = Mathf.SmoothStep(AnchoredPosition, -currentPageIndex * PageSize, SnapSpeed * 0.2f);
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
        }

        protected override void ProcessDrag(Vector2 delta)
        {
            float x = IsHorizontal ? delta.x : 0;
            float y = IsHorizontal ? 0 : delta.y;
            Content.anchoredPosition += new Vector2(x, y);
            
            // Set drag direction for SetCurrentPageIndex
            dragDirection = IsHorizontal ? (delta.x > 0 ? 1 : -1) : (delta.y > 0 ? 1 : -1);

            if (AlignmentDirection > 0)
            {
                SwapPage((-AnchoredPosition > infiniteScrollPivotEnd * PageSize), true, ref infiniteScrollPivotEnd);
                SwapPage((-AnchoredPosition < infiniteScrollPivotStart * PageSize), false, ref infiniteScrollPivotStart);
            }
            else
            {
                SwapPage((-AnchoredPosition > (infiniteScrollPivotStart - 1) * PageSize), false, ref infiniteScrollPivotStart);
                SwapPage((-AnchoredPosition < (infiniteScrollPivotEnd - 1) * PageSize), true, ref infiniteScrollPivotEnd);
            }
        }
        
        private void SwapPage(bool moveCondition, bool moveFirstChild, ref int pivot)
        {
            if (moveCondition)
            {
                infiniteScrollPivotEnd -= dragDirection;
                infiniteScrollPivotStart -= dragDirection;

                Transform child;

                if (moveFirstChild)
                {
                    child = Content.GetChild(0);
                    child.SetAsLastSibling();
                }
                else
                {
                    child = Content.GetChild(Content.childCount - 1);
                    child.SetAsFirstSibling();
                }

                float x = IsHorizontal ? (pivot - 1 * (AlignmentDirection > 0 ? 0 : 1)) * PageSize : 0;
                float y = IsHorizontal ? 0 : (pivot - 1 * (AlignmentDirection > 0 ? 0 : 1)) * PageSize;
                (child as RectTransform).anchoredPosition = new Vector2(x, y);
            }
        }

        protected override void SetCurrentPageIndex()
        {
            int newIndex = -Mathf.RoundToInt((AnchoredPosition + SnapAreaSize / 2 * dragDirection) / PageSize);

            if (currentPageIndex != newIndex)
            {
                currentPageIndex = newIndex;

                int value = (newIndex * AlignmentDirection).Mod(Content.childCount);

                OnPageChanged?.Invoke(value);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            RectTransform rect = (transform as RectTransform);
            
            if (rect == null) return;
            
            Vector2 pivot = rect.pivot;
            
            Gizmos.matrix = rect.localToWorldMatrix;

            Gizmos.color = SnapAreaSizeGizmoColor;
            Vector2 center = new Vector2((pivot.x - 0.5f) * -PageWidth, (pivot.y - 0.5f) * -PageHeight);
            Vector2 areaSize = new Vector2(IsHorizontal ? SnapAreaSize : PageWidth - 2, IsHorizontal ? PageHeight : SnapAreaSize - 2);
            Gizmos.DrawWireCube(center, areaSize);
        }
    }
}
