using UnityEngine;
using System.Collections;

namespace mountsix.player{

	public class BikeConfig : ScriptableObject {

		public CharacterPhysics physics;
		//Sounds
		[Range(0f,1f)]public float VOLUME=0.6f;
		public CharacterAudio sounds;
		public BikeAnimations animations;
		public float RECORD_INTERVAL=2f;
	}

}
