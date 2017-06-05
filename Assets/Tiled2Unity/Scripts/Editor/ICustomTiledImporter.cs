using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Tiled2Unity
{
    interface ICustomTiledImporter
    {
        // A game object within the prefab has some custom properites assigned through Tiled that are not consumed by Tiled2Unity
        // This callback gives customized importers a chance to react to such properites.
        void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties);

        // Called just before the prefab is saved to the asset database
        // A last chance opporunity to modify it through script
        void CustomizePrefab(GameObject prefab);
    }
}

// Examples

[Tiled2Unity.CustomTiledImporter]
class CustomImporterAddComponent : Tiled2Unity.ICustomTiledImporter
{
    public void HandleCustomProperties(UnityEngine.GameObject gameOb,
        IDictionary<string, string> props)
    {
		// Load the prefab assest and Instantiate it
		string prefabPath;
		GameObject spawn;
        if (props.ContainsKey("door"))
        {
			prefabPath = "Prefabs/Door1";
			spawn = (GameObject)Resources.Load(prefabPath);
			//do some stuff here with Doors
			//replace tiled imported object with the custom door object
			Transform oldObj = gameOb.transform.FindChild("TileObject");
			GameObject.DestroyImmediate (oldObj.gameObject);
			GameObject spawninstance = (GameObject)GameObject.Instantiate (spawn);
			spawninstance.name = spawn.name;
			spawninstance.transform.SetParent (gameOb.transform);
			spawninstance.transform.localPosition = new Vector3(16f,16f,0f);
			spawninstance.transform.localScale = new Vector3 (20f,20f,20f);
        }
		//if (props.ContainsKey ("scene")) {
		//	gameOb.GetComponentInChildren<BuildingEnterScript> ().sceneName = props ["scene"];
		//}
		if (props.ContainsKey ("npc")) {
			prefabPath = "Prefabs/npcTiledPrefab";
			spawn = (GameObject)Resources.Load(prefabPath);
			Transform oldObj = gameOb.transform.FindChild("TileObject");
			GameObject.DestroyImmediate (oldObj.gameObject);
			GameObject spawninstance = (GameObject)GameObject.Instantiate (spawn);
			//spawninstance.name = spawn.name;
			spawninstance.transform.SetParent (gameOb.transform);
			spawninstance.transform.localPosition = new Vector3(16f,16f,0f);
			spawninstance.transform.localScale = new Vector3 (20f,20f,20f);

			//set up instance-specific stuff
			spawninstance.name = props["npc"];
			//there should be an animatorcontroller sharing the name of the npc property
			spawninstance.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animators/" + props["npc"]) as RuntimeAnimatorController;
		}
    }

	public void ReplaceTiledObject(GameObject gameOb, GameObject newobj){
		Transform oldObj = gameOb.transform.FindChild("TileObject");
		GameObject.DestroyImmediate (oldObj.gameObject);
		GameObject spawninstance = (GameObject)GameObject.Instantiate (newobj);
		spawninstance.name = newobj.name;
		spawninstance.transform.SetParent (gameOb.transform);
		spawninstance.transform.localPosition = new Vector3(16f,16f,0f);
		spawninstance.transform.localScale = new Vector3 (20f,20f,20f);
	}


    public void CustomizePrefab(GameObject prefab)
    {
		// Find all the polygon colliders in the pefab
		PolygonCollider2D[] polygon2Ds = prefab.GetComponentsInChildren<PolygonCollider2D>();
		if (polygon2Ds == null)
			return;

		// Find all *ground* polygon colliders
		int groundMask = LayerMask.NameToLayer("Ground");

		//add a platform effector to the collision polygon(s)
		foreach (PolygonCollider2D poly in polygon2Ds) {
			poly.gameObject.AddComponent<PlatformEffector2D> ();
			poly.usedByEffector = true;
		}
		EdgeCollider2D[] edgecolliders = prefab.GetComponentsInChildren<EdgeCollider2D> ();
		if (edgecolliders == null)
			return;
		foreach (EdgeCollider2D coll in edgecolliders) {
			coll.usedByEffector = true;
		}

    }
}

