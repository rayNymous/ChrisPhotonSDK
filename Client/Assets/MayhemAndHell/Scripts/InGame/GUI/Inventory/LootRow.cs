using UnityEngine;
using System.Collections;

public class LootRow : MonoBehaviour
{

    public int Index;
    public UILabel Name;
    public LootSlot Slot;
    public UIButton TakeButton;
    public LootContainer LootContainer;

	// Use this for initialization
	void Start () {
	    
	}

    public void OnTake()
    {
        LootContainer.TakeItemAt(Index);
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
