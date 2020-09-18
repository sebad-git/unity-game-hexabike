using UnityEngine;
using UnityEditor;

namespace mountsix.runner{

	public class TileCreator {

		[MenuItem ("Assets/Create/Mountsix/Runner/Tile Creator")]
		public static void createConfig () {
			TileCreatorConfig config = ScriptableObject.CreateInstance<TileCreatorConfig> ();
			ProjectWindowUtil.CreateAsset(config, "tileConfig.asset");
		}
	}
}
