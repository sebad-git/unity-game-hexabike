using UnityEngine;
using System.Collections;

public class RTransform {

	public Quaternion rRotation;
	public Vector3 rPosition;
	public Vector3 rSize;

	public RTransform(){}

	public RTransform(Transform transform){
		rRotation = transform.localRotation;
		rPosition= transform.localPosition;
		rSize = transform.localScale;
	}

	public void setFrame(Transform transform){
		transform.localRotation = rRotation;
		transform.localPosition = rPosition;
		transform.localScale = rSize;
	}
}
