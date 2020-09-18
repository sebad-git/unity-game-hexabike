using UnityEngine;
using System.Collections;

namespace mountsix.player{

	[System.Serializable]
	public class CharacterPhysics {
		[Range(0f,1000f)]public float BIKE_SPEED=18f;
		[Range(50,200f)]public float BOOST_SPEED=120f;
		[Range(1,30f)]public float BOOST_BREAK_SPEED=3f;
		[Range(0,100f)]public float GROUND_DISTANCE = 2f;
		public Vector3 FALL_SPEED = new Vector3(2,5,10);
		public string GROUND_LAYER_NAME="Ground";
		[HideInInspector]public int GROUND_LAYER_VALUE;
	}

}
