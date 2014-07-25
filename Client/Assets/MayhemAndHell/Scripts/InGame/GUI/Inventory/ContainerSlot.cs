using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ContainerSlot : MonoBehaviour
{
    public UISprite Icon;
    public UIWidget Background;
    public UILabel Label;

    public AudioClip GrabSound;
    public AudioClip PlaceSound;
    public AudioClip ErrorSound;

    public ContainerItem Item;
    private string _text;

    private bool _disabled;
    public bool Disabled { get { return _disabled; } }
    
    protected abstract ContainerItem ObservedItem { get; }
    protected abstract ContainerItem Replace(ContainerItem item);
    protected abstract void OnSwitch(ContainerSlot source); 

    private static ContainerSlot _draggedItemSlot;

    public int SlotIndex = 0;

    private static Color _defaultBackgroundColor = new Color(0.22f, 0.22f, 0.22f);

    public void Disable()
    {
        _disabled = true;
        var color = Icon.color;
        color.a = 0.5f;
        Icon.color = color;
    }

    public void Enable()
    {
        _disabled = false;
        var color = Icon.color;
        color.a = 1f;
        Icon.color = color;
    }

    void OnTooltip(bool show)
    {
        var item = show ? Item : null;

        if (item != null)
        {
            string text = "[" + NGUIText.EncodeColor(Item.Color) + "]" + Item.Name + "[-]\n";
            text += "[AFAFAF]Level " + item.ItemLevel + " item";

            if (item.Stats != null)
            {
                foreach (var stat in item.Stats)
                {
                    if (stat.Value > 0)
                    {
                        text += "\n[00FF00]" + String.Format(stat.Key, stat.Value);
                    }
                    else
                    {
                        text += "\n[FF0000]" + String.Format(stat.Key, stat.Value);
                    }

                    text += "[-]";
                }

            }
            
            if (!string.IsNullOrEmpty(item.Description))
            {
                text += "\"\n[FF9900]" + item.Description + "\"";
            }
            UITooltip.ShowText(text);
            return;
        }

        UITooltip.ShowText(null);
    }

    public virtual void OnClick()
    {
        if (_draggedItemSlot != null)
        {
            OnDrop(null);
        }
        else if (Item != null)
        {
            Disable();
            _draggedItemSlot = this;
            if (_draggedItemSlot != null) NGUITools.PlaySound(GrabSound);
            UpdateCursor();
        }
    }

    public virtual void OnDrag(Vector2 delta)
    {
        if (_draggedItemSlot == null && Item != null && !Disabled)
        {
            Disable();
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
            _draggedItemSlot = this;
            NGUITools.PlaySound(GrabSound);
            UpdateCursor();
        }
    }

    public virtual bool CanBeDroppedHere(ContainerItem item)
    {
        return true;
    }

    void OnDrop(GameObject go)
    {
        if (Disabled && _draggedItemSlot != null)
        {
            _draggedItemSlot.Enable();
            _draggedItemSlot = null;
            UICursor.Clear();
            return;
        }

        if (_draggedItemSlot != null && _draggedItemSlot.Item != null
            && CanBeDroppedHere(_draggedItemSlot.Item))
        {
            Disable();
            Switch(_draggedItemSlot, this);
            UICursor.Clear();
        }
    }

    void Switch(ContainerSlot source, ContainerSlot target)
    {
        var oldItem = Replace(_draggedItemSlot.Item);
        if (oldItem != _draggedItemSlot.Replace(oldItem))
        {
            OnSwitch(source);
        }
        _draggedItemSlot = null;
    }

    void UpdateCursor()
    {
        if (_draggedItemSlot != null)
        {
            UICursor.Set(_draggedItemSlot.Item.Atlas, _draggedItemSlot.Item.SpriteName);
        }
        else
        {
            UICursor.Clear();
        }
    }

    void Update()
    {
        ContainerItem item = ObservedItem;
        if (Item != item)
        {
            Item = item;

            if (Item == null && Background != null)
            {
                Background.color = _defaultBackgroundColor;
                Icon.enabled = false;
            }

            if (Icon != null)
            {
                if (Item == null || Item.Atlas == null)
                {
                    Icon.enabled = false;
                }
                else
                {
                    Icon.spriteName = item.SpriteName;
                    Icon.atlas = Item.Atlas;
                    Icon.enabled = true;
                    Icon.MakePixelPerfect();
                }
            }
            else
            {
                Debug.Log("No atlas found for container slot item " + (Item.Atlas != null));
            }

            if (Item != null && Background != null)
            {
                Background.color = Item.Color;
            }

        }
    }

}
