using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using ProtoBuf.Meta;
using UnityEngine;
using System.Collections;

public class LootContainer : MonoBehaviour
{
    private List<ContainerItem> _items = new List<ContainerItem>();

    private int _rowsPerPage = 3;
    private int _currentPage = 0;

    private int _itemCount;

    public int LastPage { get { return (_itemCount / _rowsPerPage) + 1; } }
    public int CurrentPage {get { return _currentPage + 1; }}

    public List<LootRow> Rows;

    public UILabel PageNumber;

    public UIButton PreviousButton;
    public UIButton NextButton;

    public UIAtlas Atlas;

    private Gui _gui;

	// Use this for initialization
	void Start ()
	{
        RuntimeTypeModel.Default.Add(typeof(ContainerData), true)[3].SupportNull = true;
        _gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(gameObject, false);

        
        //UpdateRows();
	}

    public void Close()
    {
        NGUITools.SetActive(gameObject, false);
    }

    public void TakeItemAt(int position)
    {
        int itemIndex = _currentPage*_rowsPerPage + position;

        _gui.GameView.Controller.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, MessageSubCode.TakeItem},
                {(byte)ClientParameterCode.Object, itemIndex}
            }
        }, true, 0, false);
    }

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < _itemCount)
        {
            _items[index] = null;
        }
        UpdateRows();
    }

    public void Show(List<ContainerItem> items)
    {
        _currentPage = 0;
        _itemCount = items.Count;
        while (_itemCount > _items.Count)
        {
            _items.Add(null);
        }

        for (int i = 0; i < _itemCount; i++)
        {
            if (items[i] != null)
            {
                items[i].Atlas = Atlas;
                _items[i] = items[i];
            }
        }
        UpdateRows();
        NGUITools.SetActive(gameObject, true);
    }

    public void NextPage()
    {
        if (HasNextPage())
        {
            _currentPage++;
            UpdateRows();
        }
    }

    public void PreviousPage()
    {
        if (HasPrevPage())
        {
            _currentPage--;
            UpdateRows();
        }
    }

    private bool HasNextPage()
    {
        return _currentPage < LastPage - 1;
    }

    private bool HasPrevPage()
    {
        return _currentPage > 0;
    }

    public void UpdateRows()
    {
        foreach (var lootRow in Rows)
        {
            int itemIndex = _currentPage*_rowsPerPage + lootRow.Index;
            if (_items != null && _itemCount > itemIndex && _items[itemIndex] != null)
            {
                lootRow.Slot.Icon.atlas = _gui.IconAtlas;
                lootRow.Slot.Icon.spriteName = _items[itemIndex].SpriteName;
                lootRow.Slot.Icon.enabled = true;
                lootRow.Slot.Icon.MakePixelPerfect();
                lootRow.Name.text = _items[itemIndex].Name;
                NGUITools.SetActive(lootRow.Slot.gameObject, true);
                NGUITools.SetActive(lootRow.TakeButton.gameObject, true);
                NGUITools.SetActive(lootRow.Slot.Icon.gameObject, true);
            }
            else
            {
                lootRow.Slot.Icon.enabled = false;
                lootRow.Name.text = "<Nothing>";
                NGUITools.SetActive(lootRow.Slot.gameObject, false);
                NGUITools.SetActive(lootRow.TakeButton.gameObject, false);
            }
        }

        PageNumber.text = CurrentPage + "/" + LastPage;
        NGUITools.SetActive(NextButton.gameObject, HasNextPage());

        NGUITools.SetActive(PreviousButton.gameObject, HasPrevPage());
        NGUITools.SetActive(PageNumber.parent.gameObject, LastPage > 1);
    }

    public ContainerItem GetItem(int index)
    {
        return _items != null && _itemCount > index ? _items[index] : null;
    }
}
