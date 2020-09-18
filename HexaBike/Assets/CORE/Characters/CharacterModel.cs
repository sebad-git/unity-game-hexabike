using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterModel : Recordable {
	public Renderer render;
	public Material rewindMaterial;
	public Transform hip;
	public GameObject charEffectPrefab;
	[HideInInspector]public Animator animator;
	//PRIVATE
	private List<RTransform> frames;
	private GameObject characterEffect;
	private Transform effectHip;
	private Material defaultMaterial;

	void Start () {
		this.animator = gameObject.GetComponent<Animator>();
		this.frames = new List<RTransform>();
		this.defaultMaterial=this.render.material;
	}

	public override void record(){
		this.frames.Add(new RTransform(this.effectHip));
	}
	
	public void activateRagdoll(Vector3 velocity){
		if (this.characterEffect == null) {
			this.render.enabled = false;
			this.characterEffect = (GameObject)Instantiate (this.charEffectPrefab, this.hip.position,
		                                    this.charEffectPrefab.transform.rotation);
			this.effectHip = this.characterEffect.transform.Find ("Hip");
			this.effectHip.GetComponent<Rigidbody>().velocity = velocity;
			if(this.state.Equals(RecordState.RECORDING)){
				Renderer cRender=this.characterEffect.GetComponentInChildren<Renderer>();
				cRender.material=this.rewindMaterial;
			}
		}
	}

	public override void rewind(){
		if (this.frames.Count > 0) {
			RTransform rt= this.frames[(this.frames.Count-1)]; rt.setFrame(this.effectHip);
			this.frames.RemoveAt((this.frames.Count-1));
		}
		else {
			Destroy(this.characterEffect); this.characterEffect=null;
			this.render.material = this.rewindMaterial;
			this.render.enabled = true; this.state=RecordState.NORMAL;
		}
	}

	public void resetMaterial(){
		this.render.material = this.defaultMaterial;
	}
}
