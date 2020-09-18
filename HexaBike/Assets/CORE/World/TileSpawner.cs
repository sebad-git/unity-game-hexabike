using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using mountsix.characters;

namespace mountsix.runner{

	public class TileSpawner : MonoBehaviour {

		public TileCreatorConfig config;
		//PRIVATE
		private BPlayer player;
		private float spawnZ;
		private List<GameObject> activeTiles;
		private List<GameObject> activePowerUps;
		private float powerInterval;
		private float timer;

		void Start () {
			this.player=GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).GetComponent<BPlayer>();
			this.activeTiles = new List<GameObject>(); this.activePowerUps = new List<GameObject>();
			for (int i=0; i<this.config.tilesPerScreen; i++) { this.spawnTile(); }
			this.powerInterval = Random.Range(this.config.spawnTimeRange.x,this.config.spawnTimeRange.y);
		}

		void Update () {
			this.timer += Time.deltaTime;
			float spawnPos = (this.spawnZ - this.config.tilesPerScreen * this.config.tileLenght);
			if (player.transform.position.z > spawnPos ) { this.spawnTile(); }
			if (this.player.frames.Count > 0 && this.player.frames[0].rPosition.z > spawnPos) { 
				this.StartCoroutine(this.deleteTile()); this.deletePWP();
			}
			if(this.timer>=this.powerInterval){ this.spawnPowerUp(); }
		}

		private void spawnTile(){
			GameObject go = null;
			if (this.activeTiles.Count == 0) { go = (GameObject)Instantiate(config.tiles[0]); } 
			else {
				go = (GameObject)Instantiate(config.tiles[Random.Range(0,this.config.tiles.Length)]);		
			}
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.Euler(Vector3.zero);
			go.transform.position = (Vector3.forward * this.spawnZ);
			this.spawnZ += this.config.tileLenght;
			this.activeTiles.Add(go);
		}

		private IEnumerator deleteTile(){
			yield return new WaitForSeconds(this.config.destroyTime);
			if (this.activeTiles.Count > 0) {
				Destroy (this.activeTiles[0]); this.activeTiles.RemoveAt(0);
			}
		}

		private void spawnPowerUp(){
			GameObject pwp = (GameObject)Instantiate(config.powerUps[Random.Range(0,this.config.powerUps.Length)]);
			pwp.transform.rotation = Quaternion.identity;
			float distance = Random.Range(this.config.spawnDistanceRange.x,this.config.spawnDistanceRange.y);
			Vector3 pos = Vector3.zero; pos.z = (this.player.transform.position.z+distance);
			pwp.transform.position = pos; this.activePowerUps.Add(pwp);
			this.powerInterval = Random.Range(this.config.spawnTimeRange.x,this.config.spawnTimeRange.y);
			this.timer = 0;
		}

		private void deletePWP(){
			if (this.activePowerUps.Count>0) {
				Destroy (this.activePowerUps[0]); this.activePowerUps.RemoveAt(0);
			}
		}

	}
}
