using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ListView;
using InControl;

public class CardsListController : ListViewScroller{
	
	private InputDevice joystick;

	void Awake(){
		joystick = InputManager.ActiveDevice;
	}

	protected override void HandleInput(){
		if (joystick.LeftBumper.WasPressed) {
			Debug.Log ("left");
		}
		if (joystick.RightBumper.WasPressed){
			Debug.Log ("right");
		
		}
	}

}
