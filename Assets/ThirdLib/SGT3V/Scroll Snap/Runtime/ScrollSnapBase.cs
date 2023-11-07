/* 
    Copyright (C) 2021 SGT3V, Sercan Altundas
    
    Visit for details: https://sercan-altundas.gitbook.io/asset-store/ui-components/scroll-snap
*/

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SGT3V.ScrollSnap
{
  public abstract class ScrollSnapBase : UIBehaviour, IDragHandler, IEndDragHandler
  {
    /// <summary>
    ///     Container for scroll snap pages.
    /// </summary>
    public RectTransform Content;

    /// <summary>
    ///     Axis of the pages.
    /// </summary>
    public RectTransform.Axis ScrollSnapAxis = RectTransform.Axis.Horizontal;

    /// <summary>
    ///     Align start of the pages to the top or to the bottom of the <see cref="Content"/>.
    /// </summary>
    public VerticalAlignment VerticalPageAlignment = VerticalAlignment.Bottom;

    /// <summary>
    ///     Align start of the pages to the left side or to the right side of the <see cref="Content"/>.
    /// </summary>
    public HorizontalAlignment HorizontalPageAlignment = HorizontalAlignment.Left;
    
    /// <summary>
    ///     Speed of snap between 1 to 10. Value 10 snaps immediately.
    /// </summary>
    [Range(0, 1)] public float SnapSpeed = 0.5f;

    public GameObject PagePrefab;
    
    /// <summary>
    ///     Amount of the scroll from 0 to number of pages.
    /// </summary>
    public float ScrollAmount { get; protected set; }
    
    [Serializable] public class ScrollSnapEvent : UnityEvent<int> { }

    /// <summary>
    ///     Event called on page changes. Returns the changed index.
    /// </summary>
    public ScrollSnapEvent OnPageChanged = new ScrollSnapEvent();
    
    /// <summary>
    ///     Size of the center area in pixels, that is necessary for activating the snap. If the next page enters this range page will snap.
    /// </summary>
    [Min(0)] public float SnapAreaSize = 200;
    
    protected bool isDragged;
    protected int dragDirection;
    
    /// <summary>
    ///     Width of <see cref="ScrollSnap"/> page.
    /// </summary>
    public float PageWidth { get; protected set; }

    /// <summary>
    ///     Height of <see cref="ScrollSnap"/> page.
    /// </summary>
    public float PageHeight { get; protected set; }
    
    protected bool IsHorizontal => ScrollSnapAxis == RectTransform.Axis.Horizontal;

    protected bool IsLeftAligned => HorizontalPageAlignment == HorizontalAlignment.Left;

    protected bool IsBottomAligned => VerticalPageAlignment == VerticalAlignment.Bottom;

    protected float PageSize => IsHorizontal ? PageWidth : PageHeight;
    
    protected float AnchoredPosition => IsHorizontal ? Content.anchoredPosition.x : Content.anchoredPosition.y;
    
    private new void Start()
    {
      base.Start();

      ResetScrollSnapUI();
    }
    
    private void LateUpdate()
    {
      if (Content != null && Application.isPlaying)
      {
        SnapToPage();
      }
    }

    protected override void OnRectTransformDimensionsChange()
    {
      ResetScrollSnapUI();
    }
    
    public void ResetScrollSnapUI()
    {
      if (IsContentNull()) return;

      SetPageSizes();
      SetPagePositions();
      SetContentPosition();
    }
    
    protected abstract void SetPageSizes();
    
    protected abstract void SetPagePositions();
    
    protected abstract void SetContentPosition();
    
    protected abstract void SnapToPage();

    protected bool IsContentNull()
    {
      if (Content == null)
      {
        Debug.LogWarning($"ScrollSnap.ResetScrollSnapUI: Content field is null on ScrollSnap component.");
        return true;
      }

      return false;
    }

    public abstract void OnEndDrag(PointerEventData eventData);
    
    public abstract void OnDrag(PointerEventData eventData);
    
    protected abstract void ProcessDrag(Vector2 delta);
    
    protected abstract void SetCurrentPageIndex();

    public GameObject AddPage()
    {
      //GameObject prefab = Resources.Load<GameObject>("Prefabs/Page");
      GameObject page = Instantiate(PagePrefab, Content);
      page.name = "Page";
      ResetScrollSnapUI();

      return page;
    }
    
    protected abstract void OnDrawGizmosSelected();
    
    private new void OnDestroy()
    {
      base.OnDestroy();
            
      OnPageChanged.RemoveAllListeners();
    }
  }
}
