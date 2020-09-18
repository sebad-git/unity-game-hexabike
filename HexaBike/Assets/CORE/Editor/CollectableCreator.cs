using UnityEngine;
using System.Collections;
using UnityEditor;

namespace mountsix.props.collectable{

public class CollectableCreator {

		[MenuItem ("Assets/Create/Mountsix/Props/Collectable")]
		public static void createConfig () {
		CollectableConfig config = ScriptableObject.CreateInstance<CollectableConfig> ();
			ProjectWindowUtil.CreateAsset(config, "collectable.asset");
		}
	}
}
