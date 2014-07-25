using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class DialogWindow : MonoBehaviour
{
    public UILabel BoxTitle;
    public UILabel Text;
    public UILabel LeftButton;

    public UITable ContentTable;

    public UIScrollView View;

    private GameObject _dialogButtonPrefab;

    private List<DialogLinkButton> _linkButtons;

    private Gui _gui;

    private int _currentPageId;

    // Use this for initialization
    private void Start()
    {
        _gui = FindObjectOfType<Gui>();
        _dialogButtonPrefab = Resources.Load<GameObject>("Prefabs/Gui/DialogButton");
        _linkButtons = new List<DialogLinkButton>();

        NGUITools.SetActive(gameObject, false);

        //ShowDialog(new DialogPageData()
        //{
        //    LeftButtonEnabled = true,
        //    LeftButtonText = "Accept",
        //    Links = new List<DialogLinkData>()
        //    {
        //        new DialogLinkData(){Text = "I don't like it", Type = DialogLinkType.Page},
        //        new DialogLinkData(){Text = "I am a fish", Type = DialogLinkType.QuestComplete},
        //        new DialogLinkData(){Text = "I too am a big sand withd lol text fish 2", Type = DialogLinkType.QuestComplete},
        //    },
        //    NpcName = "Skull The Bull",
        //    PageId = 0,
        //    Text = "It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', "
        //});
    }

    public void ShowDialog(DialogPageData data)
    {
        NGUITools.SetActive(this.gameObject, true);
        _currentPageId = data.PageId;
        BoxTitle.text = data.NpcName;
        Text.text = data.Text;
        Text.GetComponent<BoxCollider>().size = Text.localSize;

        NGUITools.SetActive(LeftButton.parent.gameObject, data.LeftButtonEnabled);
        LeftButton.text = data.LeftButtonText;

        int index = 0;
        if (data.Links != null)
        {
            foreach (var linkData in data.Links)
            {
                if (index < _linkButtons.Count)
                {
                    NGUITools.SetActive(_linkButtons[index].gameObject, true);
                    _linkButtons[index].Text.text = linkData.Text;
                }
                else
                {
                    var linkButton = NGUITools.AddChild(ContentTable.gameObject, _dialogButtonPrefab).GetComponent<DialogLinkButton>();
                    linkButton.name = "B" + index;
                    linkButton.GetComponent<UIDragScrollView>().scrollView = View;
                    linkButton.Text.text = linkData.Text;
                    linkButton.Index = index;
                    linkButton.Window = this;
                    _linkButtons.Add(linkButton);
                }

                _linkButtons[index].Icon.spriteName = GetLinkIconFromType(linkData.Type);
                _linkButtons[index].Icon.MakePixelPerfect();
                index++;
            }
        }

        while (index < _linkButtons.Count)
        {
            NGUITools.SetActive(_linkButtons[index].gameObject, false);
            index++;
        }
        ContentTable.Reposition();
    }

    public string GetLinkIconFromType(DialogLinkType type)
    {
        switch (type)
        {
            
            case DialogLinkType.QuestStart:
                return "quest_icon";
            case DialogLinkType.QuestComplete:
                return "quest_complete_icon";
        }

        return "chat_icon";
    }

    // Update is called once per frame
	void Update () {
	
	}

    public void CloseDialog()
    {
        NGUITools.SetActive(transform.gameObject, false);
    }

    public void OnLeftButtonClick()
    {
        SendDialogAction(-2);
        CloseDialog();
    }

    public void OnLinkClick(int index)
    {
        SendDialogAction(index);
    }

    private void SendDialogAction(int index)
    {
        var dialogAction = new DialogAction()
        {
            CurrentPage = _currentPageId,
            Index = index
        };

        var para = new Dictionary<byte, object>
        {
            {(byte) ClientParameterCode.SubOperationCode, MessageSubCode.DialogAction},
            {(byte) ClientParameterCode.Object, PacketHandler.Serialize(dialogAction)}
        };

        _gui.GameView.InGameController.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = para
        }, true, 0, false);
    }
}
