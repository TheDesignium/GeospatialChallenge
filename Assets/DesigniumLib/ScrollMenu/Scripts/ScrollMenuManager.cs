using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SGT3V.ScrollSnap;
using UnityEngine.UI;

namespace ScrollMenu.Main
{
    public class ScrollMenuManager : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] public ScrollSnap scrollSnap;

        [Header("Vars")]
        // Specify GameObject to add to Menu
        [SerializeField] private GameObject itemPrefab;
        // Scale of center GameObject
        [SerializeField] private ItemState centerState;
        // Other GameObject scales
        [SerializeField] private ItemState otherState;

        [System.Serializable]
        public class ItemState
        {
            public float scale;
            public float alpha;
        }

        private List<Transform> items;
        private List<CanvasGroup> itemCGs;
        private List<Button> itemBtns;

        //----------------------------------
        //  Event
        //----------------------------------
        #region MenuClickEvent
        [System.Serializable]
        public class MenuClickEvent : UnityEngine.Events.UnityEvent<int> { }

        [SerializeField]
        private MenuClickEvent menuClickEvent = new MenuClickEvent();

        void OnEnable()
        {
            menuClickEvent.AddListener(MenuClick);
        }

        void OnDisable()
        {
            menuClickEvent.RemoveListener(MenuClick);
        }

        void MenuClick(int btnNum)
        {
            //print("MenuClick: " + btnNum);
        }
        #endregion


        //----------------------------------
        //  Public
        //----------------------------------
        public void Start()
        {
          scrollSnap.PagePrefab = itemPrefab;

            // 1. Delete all current Item
            RemoveItems();

            // We have to wait one frame after RemoveItems.
            StartCoroutine(SetMenuWait());
        }

        private IEnumerator SetMenuWait()
        {
            yield return null;

            // init
            items = new List<Transform>();
            itemCGs = new List<CanvasGroup>();
            itemBtns = new List<Button>();

        }

        #region AddItem (Add an Item to the Menu.)
        public void AddItem(Texture2D texture)
        {
            int pageCount = scrollSnap.Content.childCount;

            GameObject pageObj = scrollSnap.AddPage();
            pageObj.name = "item_" + pageCount;
            GameObject btnObj = pageObj.transform.GetChild(0).gameObject;
            Button itemBtn = btnObj.GetComponent<Button>();
            itemBtn.onClick.AddListener(
                () => { MenuBtnClick(pageCount); }
            );
            if (texture != null)
            {
                btnObj.GetComponent<Image>().sprite = Sprite.Create(
                    texture,
                    new Rect(0.0f, 0.0f, texture.width, texture.height), Vector2.zero
                );
            }

            ItemInit(pageCount, pageObj, itemBtn);
        }

        private void ItemInit(int num, GameObject itemObj, Button itemBtn)
        {
            Transform pageTrf = itemObj.transform;
            Debug.Log(pageTrf.name);
            Debug.Log(pageTrf.childCount);
            items.Add(pageTrf.GetChild(0));
            itemCGs.Add(pageTrf.GetComponent<CanvasGroup>());
            itemBtns.Add(itemBtn);

            // init
            scrollSnap.CurrentPageIndex = 0;
            int page = 0;//scrollSnap.CurrentPageIndex;
            float value;
            float value2;
            if (num == page)
            {
                value = centerState.scale;
                value2 = centerState.alpha;
            }
            else
            {
                value = otherState.scale;
                value2 = otherState.alpha;
            }
            items[num].localScale = Vector3.one * value;
            itemCGs[num].alpha = GetAlphaValue(value2);

            if (num == page) itemBtns[num].enabled = true;
            else itemBtns[num].enabled = false;
        }

        private void MenuBtnClick(int itemNum)
        {
            menuClickEvent.Invoke(itemNum);
        }
        #endregion

        #region Delete menu items
        // Delete all menu items.
        // [Note!] Need to wait one frame for it to disappear.
        public void RemoveItems()
        {
            DeleteAllGameObject(scrollSnap.Content.gameObject, false);
            if(items != null) items.Clear();
            if (itemCGs != null) itemCGs.Clear();
            if (itemBtns != null) itemBtns.Clear();
            scrollSnap.ResetScrollSnapUI();
        }
        #endregion


        //----------------------------------
        //  Start / Update
        //----------------------------------

        private int pageIndex;
        private void Update()
        {
            if (scrollSnap.Content.childCount == 0) return;
            if (items == null || items.Count == 0) return;

            // set scale
            int index = Mathf.RoundToInt(scrollSnap.ScrollAmount);
            if (items.Count <= index || items[index] == null) return;
            float baseBlend = Mathf.Abs(Mathf.Cos(scrollSnap.ScrollAmount * Mathf.PI));

            float blend = (index == Mathf.RoundToInt(scrollSnap.ScrollAmount)) ? centerState.scale : otherState.scale;
            blend = Mathf.Lerp(otherState.scale, centerState.scale, baseBlend);
            items[index].localScale = Vector3.one * blend;

            // set alpha
            float blend2 = (index == Mathf.RoundToInt(scrollSnap.ScrollAmount)) ? centerState.alpha : otherState.alpha;
            blend2 = Mathf.Lerp(otherState.alpha, centerState.alpha, baseBlend);
            itemCGs[index].alpha = GetAlphaValue(blend2);

            int nowPage = scrollSnap.CurrentPageIndex;
            if (nowPage != pageIndex) ChangeEnableBtn(nowPage);
            pageIndex = nowPage;

            // Update other items to the desired scale/alpha.
            for (int i = 0; i < itemCGs.Count; i++)
            {
                if (i != index)
                {
                    SetOtherPageState(i);
                }
            }
        }

        private void SetOtherPageState(int num)
        {
            itemCGs[num].alpha = Mathf.Lerp(itemCGs[num].alpha, otherState.alpha, Time.deltaTime * 10);
            Vector3 targetScale = Vector3.one * otherState.scale;
            items[num].localScale = Vector3.Lerp(items[num].localScale, targetScale, Time.deltaTime * 10);
        }

        private void ChangeEnableBtn(int nowPage)
        {
            for (int i = 0; i < itemBtns.Count; i++)
            {
                itemBtns[i].enabled = false;
            }
            itemBtns[nowPage].enabled = true;
        }

        //----------------------------------
        //  Common
        //----------------------------------
        #region GetAlphaValue
        private float GetAlphaValue(float blend)
        {
            return Mathf.Clamp(blend, 0, 1);
        }
        #endregion

        #region DeleteAllGameObject
        // Delete all GameObjects under the parent GameObject.
        private void DeleteAllGameObject(GameObject parentObj, bool delParent)
        {
            List<GameObject> allGameObj = GetChildGameObject(parentObj);
            foreach (GameObject thisObj in allGameObj)
            {
                Destroy(thisObj);
            }

            if (delParent) Destroy(parentObj);
        }

        // Only get the GameObject immediately below.
        private List<GameObject> GetChildGameObject(GameObject parentObj)
        {
            List<GameObject> childrenObj = new List<GameObject>();
            foreach (Transform child in parentObj.transform)
            {
                childrenObj.Add(child.gameObject);
            }

            return childrenObj;
        }
        #endregion
    }
}
