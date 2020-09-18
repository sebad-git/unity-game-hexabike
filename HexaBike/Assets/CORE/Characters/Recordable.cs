using UnityEngine;
using System.Collections;

public abstract class Recordable : MonoBehaviour {

	public const float FRAMERATE=0.06f;

	public enum RecordState{NORMAL=0,RECORDING=1,REWIND=2};
	[HideInInspector]public RecordState state;

	public abstract void record();
	public abstract void rewind();

	public bool isRecording(){ return this.state.Equals(RecordState.RECORDING); }
	public bool isReWinding(){ return this.state.Equals(RecordState.REWIND); }

}
