using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

/// <summary>
/// This script should load save data from a file about a player. It should send that player data
/// to the GameManager.
/// </summary>

public static class SaveLoad {

	public static List<PlayerData> savedgames = new List<PlayerData> ();


	public static PlayerData CreatePlayerData(Player p, string mrlevel){
		PlayerData pd = new PlayerData ();
		/*pd.voltage = p.GetVoltage ();
		pd.health = p.getHealth ();
		pd.fightpts = p.GetFightingPoints ();
		pd.hackpts = p.GetHackingPoints ();
		pd.stealthpts = p.GetStealthPoints ();
		pd.equipped1 = p.GetEquippedItem (1);
		pd.equipped2 = p.GetEquippedItem (2);

		//pd.items = p.GetInventory().GetSerializableInventory();
		//prob need to separate this out, so it can be selectively used, escpecially on game load
		pd.levels = GameManager.instance.GetLevelGraph ();
		//this should always be updated when creating data to save
		pd.levellist = GameManager.masterlevelslist;
		pd.mostRecentLevel = mrlevel;*/
		return pd;
	}

	//add data for world map, levels unlocked, most recent level, etc to our currently used PlayerData object
	public static void AddLevelsData(PlayerData pd, LevelGraph lg, List<Level> levels, string mostRecent) {
		/*pd.levels = lg;
		pd.levellist = levels;
		pd.mostRecentLevel = mostRecent;
		*/
	}
		
	public static Player CreatePlayer(PlayerData pd){
		Player p = new Player();
		/*p.SetVoltage(pd.voltage);
		p.SetHealth (pd.health);
		p.SetFightingPoints (pd.fightpts);
		p.SetHackingPoints (pd.hackpts);
		p.SetStealthPoints (pd.stealthpts);

		//add items to the player object
		//this will equip the first two it finds, if any
		if (pd.items != null) {
			foreach (SerializableKeyValue<string, Item> it in pd.items) {
				Debug.Log ("jbdjf: ");
				it.value.DisplayStats ();
				p.AddItem (it.value, it.value.GetScriptName());
			}
		}

		//if player had items equipped, re-equip them to this created player
		if (pd.equipped1 != null) {
			p.EquipItem (pd.equipped1);
		}
		if (pd.equipped2 != null) {
			p.EquipItem (pd.equipped2);
		}
		*/

		return p;
	}

	public static void AssignToSaveSlot(Player p, int slot, string mrlevel) {
		Debug.Log ("slott 0: " + savedgames[slot]);

		savedgames [slot] = CreatePlayerData(p, mrlevel);
	}

	public static void WriteData (int slot) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames" + slot + ".gd");
		bf.Serialize(file, SaveLoad.savedgames[slot]);
		file.Close();
		Debug.Log ("Player saved to: " + Application.persistentDataPath);
	}

	//destroy a save file corresponding to a particular slot
	public static void DeleteData(int slot) {
		File.Delete (Application.persistentDataPath + "/savedGames" + slot + ".gd");
	}

	//saves the newly updated 'Master Levels List'
	public static void SaveLevelData(string mrlevel){
		savedgames[GameManager.instance.GetSaveSlot()].levellist = GameManager.masterlevelslist;
		savedgames[GameManager.instance.GetSaveSlot()].levels = GameManager.instance.GetLevelGraph ();
		savedgames [GameManager.instance.GetSaveSlot ()].mostRecentLevel = mrlevel;

		WriteData (GameManager.instance.GetSaveSlot ());
	}

	//gives name of most recent level, according to the file of our currently used save slot
	public static string GetMostRecentLevelName(){
		return GetDataFromFile(GameManager.instance.GetSaveSlot ()).mostRecentLevel;
	}

	// Currently only creates player via player constructor. Needs to load save data!!!

	public static PlayerData LoadFromSlot(int slot){
		PlayerData p = savedgames [slot];
		return new PlayerData ();
	}

	//makes the static save slot contain info from its file, if it exists. 
	public static void LoadDataFromFile(int slot){
		SaveLoad.savedgames [slot] = GetDataFromFile (slot);
	}

	//returns a playerdata object containing info from a file
	public static PlayerData GetDataFromFile(int slot) {
		PlayerData returnData;
		Debug.Log ("standalone data path: " + Application.persistentDataPath);
		if (File.Exists (Application.persistentDataPath + "/savedGames" + slot + ".gd")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/savedGames" + slot + ".gd", FileMode.Open);
			returnData = (PlayerData)bf.Deserialize (file);
			file.Close ();
		} else {
			//there should be an instance player before this is EVER called
			returnData = CreatePlayerData (GameManager.instance.GetPlayer(), savedgames[slot].mostRecentLevel);
		}
		return returnData;
	}

	//does a file exist for the given slot?
	public static bool FileExists(int slot){
		return File.Exists (Application.persistentDataPath + "/savedGames" + slot + ".gd");
	}

	//should load all save data from computer's hard drive located in game folder/savedGames...
	public static void LoadAllData(){
		for(int i = 0; i < 3; i++) {
			LoadDataFromFile (i);
		}
		Debug.Log ("savedgames array: " + savedgames);
	}

	/*public static void DeserializeStats(SortedDictionary<string, Stat> statvalues, List<SerializableKeyValue<string, Stat>> serstatvals){
		//clear the dictionary
		statvalues.Clear ();
		//then add each key value pair from the serializable list back in
		foreach (SerializableKeyValue<string, Stat> keyval in serstatvals) {
			statvalues.Add (keyval.key, keyval.value);
		}
	}*/
		
	//this may or may not be best way to determine new gamedness
	//time and experience will tell
	public static bool IsNewGame(int whichSlot){
		//get the player data from the appropriate save file and see how it compares to what we think a new game should be
		PlayerData data = GetDataFromFile(whichSlot);
		foreach (Level level in data.levellist) {
			Debug.Log ("data level list? " + level.name);
		}
		//Debug.Log ("data level list initial value? " + ( data.levellist == GameManager.masterlevelslist));
			//Debug.Log ("master level: " + GameManager.masterlevelslist.Find (x => x.name == level.name));
		//}
		/*if (data.equipped1 == null &&
		    data.equipped2 == null &&
		    data.fightpts == 0 &&
		    data.hackpts == 0 &&
		    data.stealthpts == 0 &&
		    data.levellist == GameManager.masterlevelslist) {
			return true;
		} else {
			return false;
		}
		*/
		return true;
	}
}

//this class represents serializable save data for a player
[System.Serializable]
public class PlayerData {

	//last known health and voltage 
	//public int voltage;
	public int health;
	//items acquired
	//public List<SerializableKeyValue<string, Item>> items;
	//the two equipped items
	//public Item equipped1;
	//public Item equipped2;
	//represents pools of available points
	//public int hackpts, fightpts, stealthpts;
	//'graph' of levels unlocked for the worldmap
	public LevelGraph levels;
	//list of Level objects for game management
	public List<Level> levellist;
	//most recent level played
	public string mostRecentLevel;
}

[System.Serializable]
public class SerializableKeyValue<T1, T2> {
	public T1 key{ get; set; }
	public T2 value{ get; set; }

	public SerializableKeyValue(T1 k, T2 v){
		key = k;
		value = v;
	}

	public SerializableKeyValue(){

	}
}

//a vector3 for use with file saving/loading
[System.Serializable]
public struct FileVector3
{
	public float x;
	public float y;
	public float z;

	public void Fill(Vector3 v3)
	{
		x = v3.x;
		y = v3.y;
		z = v3.z;
	}

	public Vector3 V3
	{ get { return new Vector3(x, y, z); } }
}
