using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
//system.linq contains definitionfor sorteddictionary.elementat(index) which is very useful
using System.Linq;
using System;
using UnityEngine.UI;
using M8.Animator;

// A class that represents a player in the game.

//[System.Serializable]
public class Player {

	//private Inventory inv;
	private int voltage;
	private int health;
	//private Item equipped1;
	//private Item equipped2;
	//represents pools of available points
	private int hackpts, fightpts, stealthpts;

	// Use this for initialization
	public Player () {
		//To Do:
		//load player data from save file
		//assign values to this classes private attributes
		voltage = 40;
		health = 48;

		fightpts = 0;
		hackpts = 0;
		stealthpts = 0;
		//equipped1 = null;
		//equipped2 = null;
		//inv = new Inventory ();
	}

	public void init(){
		
		//equipped1 = inv.GetItemsList ().ElementAt (0).Value;
		//equipped1.AssignSlot (1);
		//equipped2 = inv.GetItemsList ().ElementAt (1).Value;
		//equipped2.AssignSlot (2);
	}

	// Update is called once per frame
	void Update () {
	
	}

	/*public void useWeapon(Weapon w){
		voltage -= w.GetCost();
	}*/

	public int GetVoltage(){
		return voltage;
	}

	public void SetVoltage(int v){
		voltage = v;
	}

	//if there are more than 0 items in this player's inventory, return true
	public bool HasItems(){
		//return inv.GetItemsList ().Count != 0;
		return false;
	}

	public bool HasItem(string itName){
		//return inv.GetItemsList ().Exists (x => x.GetName () == itName);
		return false;
	}

	//add a specific amount of voltage to the total
	public void addVoltage(int amount){
		voltage += amount;
	}

	public int getHealth(){
		return health;
	}

	public void AddHealth(int amount){
		GameManager.instance.GetPlayer().health += amount;
		health += amount;
	}

	public void SetHealth(int amount){
		health = amount;
	}

	//probably should be a static hud controller
	public void UpdateHealth(){
		GameObject.Find ("Health").GetComponent<Text> ().text = "Health: " + health;
	}

	//get the currently equipped item
	/*public Item GetEquippedItem(int which){
		if (which == 1) {
			//Debug.Log ("get equipped item 1: " + equipped1);
			return equipped1;
		} else if (which == 2) {
			return equipped2;
		} else {
			Debug.Log ("uh-oh, bad choice of item");
			return null;
		}
	}

	public void SetEquippedItem(int slot, Item item){
		if (slot == 1) {
			equipped1 = item;
		} else if (slot == 2) {
			equipped2 = item;
		} else {
			Debug.Log ("bad stuff in setequippeditem");
		}
	}

	public bool HasItemInSlot(int slot) {
		if (slot == 1) {
			return equipped1 != null;
		} else if (slot == 2) {
			return equipped2 != null;
		} else {
			Debug.Log ("non existent item slot: " + slot);
			return false;
		}
	}

	public Item getItem(string itname){
		//should return the requested item from the Inventory object
		return inv.GetItemByName(itname);
	}

	public Inventory GetInventory(){
		return inv;
	}

	//does all things to add an new item to a player's inventory

	//currently tested in dialogue choices script, when a dialogue is finished

	public void AddItem(Item it, string scriptname) {
		//needs to: 


		//add the monobehavior component to the player gameobject first
		AddItemScriptToPlayer(it);

		//then set up its info related to where it is equipped and such


		//add item to inventory items list
		inv.AddToInventory(it);
		Debug.Log ("number of items in inventory: " + inv.GetNumberOfItems ());
		foreach( SerializableKeyValue<string, Item> item in inv.GetSerializableInventory()){
			Debug.Log ("an item? " + item.key);
		}
		//if there is an empty item slot, go ahead and equip this new item, and update stuff accordingly
		if (inv.GetNumberOfItems () <= 2) {
			
			EquipItem(it);
		}

	}

	public void RemoveItem(Item it){
		//un-equip item if it is equipped 
		if (equipped1.GetName () == it.GetName ()) {
			equipped1 = null;
			ItemMenu.UpdateEquippedItemIcon (null, 1);
		} else if (equipped2.GetName () == it.GetName ()) {
			equipped2 = null;
			ItemMenu.UpdateEquippedItemIcon (null, 2);
		}

		//remove item from the inventory
		inv.RemoveFromInventory(it);

		//remove the script from the player gameobject
		RemoveItemScript (it);
	}

	//for the first time adding an item to a player in-game
	//basically this excludes adding items on save game load
	public void AddItemWithCutscene(Item it, string scriptname, string npcName){
		//add the monobehavior component to the player gameobject first
		AddItemScriptToPlayer(it);

		//then set up its info related to where it is equipped and such


		//add item to inventory items list
		inv.AddToInventory(it);
		Debug.Log ("number of items in inventory: " + inv.GetNumberOfItems ());
		foreach( SerializableKeyValue<string, Item> item in inv.GetSerializableInventory()){
			Debug.Log ("an item? " + item.key);
		}
		//if there is an empty item slot, go ahead and equip this new item, and update stuff accordingly
		if (inv.GetNumberOfItems () <= 2) {

			EquipItem(it);

		}

		//Add the item getting cutscene to the cutscene queue

		switch(it.GetClass()){
		case "Fighting":
			GameObject.Find (npcName).GetComponent<DialogueController> ().toPlayAfter.Enqueue (GameObject.Find ("FightItemGet").GetComponent<Cutscene> ());
			break;
		case "Hacking":
			GameObject.Find (npcName).GetComponent<DialogueController> ().toPlayAfter.Enqueue (GameObject.Find ("HackItemGet").GetComponent<Cutscene> ());
			break;
		case "Stealth":
			GameObject.Find (npcName).GetComponent<DialogueController> ().toPlayAfter.Enqueue (GameObject.Find ("StealthItemGet").GetComponent<Cutscene> ());
			break;
		}
	}

	public void AddItemScriptToPlayer(Item it){
		//add the script component
		Type ty = Type.GetType(it.GetScriptName());
		GameObject.FindGameObjectWithTag("Player").AddComponent(ty);
	}

	public void RemoveItemScript(Item it){
		Type ty = Type.GetType (it.GetScriptName ());
		UnityEngine.Object.Destroy (GameObject.FindGameObjectWithTag ("Player").GetComponent (ty));
	}

	//simply gives item a slot and assigns the appropriate player variable to this item
	public void EquipItem(Item it) {
		Debug.Log ("Trying to Equip Item!! - " + it.GetName());
		string scenename = SceneManager.GetActiveScene ().name;
		//if there's an empty slot and it is not already equipped, assign it to it(set it as equipped1 or equipped2 and call assignslot())
		if (equipped1 == null) {
			Debug.Log ("equipping item and assigning to slot!");
			equipped1 = it;
			//we should not update HUD every time an item is added
			//wait, why not? does something break sometimes?
			//put this in a hud controller
			if(scenename != "menu1" && scenename != "worldmap")	ItemMenu.UpdateEquippedItemIcon (it, 1);

			ItemMenu.PutItemInSlot(it, 1);
		} else if (equipped2 == null && equipped1 != it) {
			equipped2 = it;
			//we should not update HUD every time an item is added
			//wait, why not?
			if(scenename != "menu1" && scenename != "worldmap") ItemMenu.UpdateEquippedItemIcon (it, 2);
			ItemMenu.PutItemInSlot(it, 2);
		} else {
			//do nothing?
		}
	}

	public bool pointsAreAvailable(string itemclass){
		bool retval;
		switch (itemclass) {
		case "Fighting":
			retval = fightpts > 0;
			break;
		case "Hacking":
			retval = hackpts > 0;
			break;
		case "Stealth":
			retval = stealthpts > 0;
			break;
		default:
			retval = false;
			break;
		}
		return retval;
	}

	public int GetFightingPoints(){
		return fightpts;
	}

	public int GetHackingPoints(){
		return hackpts;
	}

	public int GetStealthPoints(){
		return stealthpts;
	}

	public void AddFightingPoints(int amt){
		fightpts += amt;
	}

	public void AddHackingPoints(int amt){
		hackpts += amt;
	}

	public void AddStealthPoints(int amt){
		stealthpts += amt;
	}

	public void SetFightingPoints(int amt){
		fightpts = amt;
	}

	public void SetHackingPoints(int amt){
		hackpts = amt;
	}

	public void SetStealthPoints(int amt){
		stealthpts = amt;
	}
	*/
}
