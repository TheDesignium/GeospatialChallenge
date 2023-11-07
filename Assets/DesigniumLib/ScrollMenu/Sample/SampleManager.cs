using System.Collections;
using System.Collections.Generic;
using ScrollMenu.Main;
using UnityEngine;

public class SampleManager : MonoBehaviour
{
    [SerializeField] private ScrollMenuManager scrollMenu;
    [SerializeField] private List<Texture2D> menuTextureList;
    [SerializeField] private Texture2D addItemTexture;

    // Start is called before the first frame update
    void Start()
    {
        //scrollMenu.SetMenu(menuTextureList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddItemBtnTap()
    {
        scrollMenu.AddItem(addItemTexture);
    }

    public void RemoveItemsTap()
    {
        scrollMenu.RemoveItems();
    }

    public void MenuBtnClickEvent(int menuNum)
    {
        print("Menu click: " + menuNum);
    }
}
