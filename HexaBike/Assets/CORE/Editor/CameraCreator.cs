using UnityEngine;
using UnityEditor;

namespace mountsix.camera{

	public class CameraCreator {

		[MenuItem ("Assets/Create/Mountsix/Util/Camera")]
		public static void createCameraConfig () {
			CameraConfig config = ScriptableObject.CreateInstance<CameraConfig> ();
			ProjectWindowUtil.CreateAsset(config, "camera.asset");
		}
	}

}
