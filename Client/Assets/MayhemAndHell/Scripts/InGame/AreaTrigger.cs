using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;
using System.Collections;

public class AreaTrigger : MonoBehaviour
{

    public int QuestId;
    public string AreaName;

    private InGame _view;

	// Use this for initialization
	void Start ()
	{
	    _view = FindObjectOfType<InGame>();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private void OnTriggerEnter2D(Collider2D coll)
    {
        _view.Controller.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, MessageSubCode.QuestAreaTrigger},
                {(byte)ClientParameterCode.Object, QuestId},
                {(byte)ClientParameterCode.Object2, AreaName}
            }
        }, true, 0, false);
        Debug.Log("SENT QUEST AREA");
    }

}
