using UnityEngine;
using System.Collections;

namespace mountsix.camera{

	public class PlayerCamera2D : MonoBehaviour {

		public CameraConfig config;
		private GameObject player;
		private float XCoord;
		private float YCoord;
		private float ZCoord;
		//private float angle;
		private bool followPlayer;
		private Vector3 lastPosition;
		private Quaternion lastRotation;
		
		void Start () {
			this.followPlayer=true; 
			GameObject[] players = GameObject.FindGameObjectsWithTag (Tags.PLAYER_TAG);
			if (players.Length > 1) {Debug.LogError("Player Tag Repeated."); }
			if (players.Length == 0) {Debug.LogError("Player Not Found."); }
			this.player = players [0];
		}
		
		void Update () {
			if (followPlayer == true) { this.cameraFollow(); }
		}
		
		public void look(Vector3 position){
			lastPosition = transform.position; lastRotation = transform.rotation;
			followPlayer = false; transform.position = position;
		}
		
		private void cameraFollow(){
			if(player!=null){
				XCoord=(this.config.ignoreX==false) ? (player.transform.position.x+this.config.ofssetX) : transform.position.x;
				YCoord=(this.config.ignoreY==false) ? (player.transform.position.y+this.config.ofssetY) : transform.position.y;
				ZCoord=(this.config.ignoreZ==false) ? (player.transform.position.z+this.config.ofssetZ) : transform.position.z;
				Vector3 playerPos=new Vector3(XCoord,YCoord,ZCoord); 
				transform.position = Vector3.Lerp (transform.position, playerPos, (Time.deltaTime * this.config.cameraSpeed) );
				//transform.LookAt(player.transform); 
			}
		}

		public void releaseCamera(){
			if(followPlayer == false){ 
				transform.position=lastPosition; 
				transform.rotation = lastRotation;
				followPlayer = true;
			}
		}
	}

}
