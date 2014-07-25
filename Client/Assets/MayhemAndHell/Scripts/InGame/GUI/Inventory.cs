using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour
{
    public ItemStorage Storage;
    public UILabel Label;

    private Gui _gui;

    // Use this for initialization
    void Start()
    {
        _gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(this.gameObject, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateData(ContainerData data)
    {
        int index = 0;
        foreach (var itemData in data.Items)
        {
            Storage.ForceReplace(index, itemData == null ? null : new ContainerItem(itemData)
            {
                Atlas = _gui.IconAtlas
            });
            index ++;
        }
    }

    public void Show()
    {
        NGUITools.SetActive(this.gameObject, true);
    }

    public void CloseWindow()
    {
        NGUITools.SetActive(this.gameObject, false);
    }
}
