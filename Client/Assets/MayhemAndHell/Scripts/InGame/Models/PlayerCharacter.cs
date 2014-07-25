using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCommon.MessageObjects.Views;
using UnityEngine;

public class PlayerCharacter : GameCharacter
{
    public override void InitializeFromView(ObjectView view)
    {
        base.InitializeFromView(view);
        PlayerView playerView = (PlayerView) view;
    }

}
