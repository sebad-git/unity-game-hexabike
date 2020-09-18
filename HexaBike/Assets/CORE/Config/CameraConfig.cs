using UnityEngine;
using System.Collections;

namespace mountsix.camera{

	public class CameraConfig : ScriptableObject {

		[Range(-300,300)]public float ofssetY=0f;
		[Range(-300,300)]public float ofssetX=0f;
		[Range(-300,300)]public float ofssetZ=0f;
		
		public bool ignoreY=false;
		public bool ignoreX=false;
		public bool ignoreZ=false;
		
		[Range(0,20)]public float cameraSpeed=3.2f;
	}

}
