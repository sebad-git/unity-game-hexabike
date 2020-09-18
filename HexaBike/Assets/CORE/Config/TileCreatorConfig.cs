using UnityEngine;

namespace mountsix.runner{
	
	public class TileCreatorConfig : ScriptableObject {
		public float tileLenght=30f;
		[Range(1,10)]public int tilesPerScreen=5;
		public float destroyTime=15f;
		public GameObject[] tiles;
		public Vector2 spawnTimeRange = new Vector2(10f,30f);
		public Vector2 spawnDistanceRange = new Vector2(10f,40f);
		public GameObject[] powerUps;
	}
	
}
