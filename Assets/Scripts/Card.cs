using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListView;

public class Card : ListViewItem<CardItemData> {

	public override void Setup(CardItemData data){
		base.Setup (data);
	}
}

[System.Serializable]
public class CardItemData: ListViewItemData{
	public int color;
	public int value;
}