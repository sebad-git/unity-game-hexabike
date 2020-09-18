using UnityEngine;
using UnityEditor;

namespace mountsix.player{

	public class BikeCreator {

		[MenuItem ("Assets/Create/Mountsix/Player/Bike")]
		public static void createBikeConfig () {
			BikeConfig config = ScriptableObject.CreateInstance<BikeConfig>();
			ProjectWindowUtil.CreateAsset(config, "bikeConfig.asset");
		}
	}

}