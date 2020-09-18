#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace EModules.PostPresets {
class PostPresets_AmplifyPostGUI : ISupportedPostComponent {
    public const string TITLE = "Amplify Color";
    public const string DOWNLOAD_MESSAGE = "Download 'Amplify Color' [Amplify]";
    public const string DOWNLOAD_LINK = "https://www.assetstore.unity3d.com/#!/content/1894";
    
    
    
    string ISupportedPostComponent.GET_TITLE { get { return TITLE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_MESSAGE { get { return DOWNLOAD_MESSAGE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_LINK { get { return DOWNLOAD_LINK; } }
    
    
    public static Type AmplifyBaseType = null;
    public static Type AmplifyEffectType = null;
    void ISupportedPostComponent.InitializeTypes()
    {   /* foreach (var assembly in Params.AssemblyList) {
           if (AmplifyEffectType == null) AmplifyEffectType = assembly.GetType( "AmplifyColorEffect", false );
         }*/
        AmplifyEffectType = Params.GetTypeFromStringName( "AmplifyColorEffect" );
            if (AmplifyEffectType != null) AmplifyBaseType = AmplifyEffectType.BaseType;
    }
    
    string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson( ((ISupportedPostComponent)this).MonoComponent ); }
    void ISupportedPostComponent.CREATE_UNDO(string undoName) { Undo.RecordObject( ((ISupportedPostComponent)this).MonoComponent, undoName ); }
    void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty( ((ISupportedPostComponent)this).MonoComponent ); }
    Type ISupportedPostComponent.MonoComponentType { get { return AmplifyEffectType; } }
    Type ISupportedPostComponent.SecondMonoComponentType { get { return null; } }
    public void SetMonoComponentDefaultParametrs() { }
    public void SetSecondMonoComponentDefaultParametrs() { }
    
    MonoBehaviour ISupportedPostComponent.MonoComponent { get; set; } //keep this line
    MonoBehaviour ISupportedPostComponent.SecondMonoComponent { get; set; } //keep this line
    
    // SKIP //
    bool ISupportedPostComponent.IsAllowToDraw { get { return true; } }
    bool ISupportedPostComponent.AntiAliasEnable { get { return true; } set { } }
    int ISupportedPostComponent.LutEnable { get { return 1; } set { } }
    void ISupportedPostComponent.CameraPredrawAction() { }
    void ISupportedPostComponent.CameraPostDrawAction() { }
    // SKIP //
    
    bool ISupportedPostComponent.LutEffectExist { get { return true; } set { } }
    
    Texture2D ISupportedPostComponent.LutTexture
    {   get { return LutTexture.GetValue( ((ISupportedPostComponent)this).MonoComponent ) as Texture2D; }
        set { LutTexture.SetValue( ((ISupportedPostComponent)this).MonoComponent, value ); }
    }
    
    float ISupportedPostComponent.LutAmount
    {   get { return 1 - (float)BlendAmount.GetValue( ((ISupportedPostComponent)this).MonoComponent ); }
        set { BlendAmount.SetValue( ((ISupportedPostComponent)this).MonoComponent, Mathf.Clamp01( 1 - value ) ); }
    }
    
    
    
    
    
    Dictionary<MonoBehaviour, Editor> p_to_e = new Dictionary<MonoBehaviour, Editor>();
    
    bool ISupportedPostComponent.LeftSideGUI(EditorWindow window, float width)
    {   var c =  ((ISupportedPostComponent)this).MonoComponent;
    
        if (!p_to_e.ContainsKey( c ))
            p_to_e.Add( c, Editor.CreateEditor( c ) );
        var e = p_to_e[c];
        if (!e)
        {   GUILayout.Label( "Internal Plugin Error", Params.Label );
            return false;
        }
        
        
        Params.scroll.x = Params.scrollX;
        Params.scroll.y = Params.scrollY;
        Params.scroll = GUILayout.BeginScrollView( Params.scroll, alwaysShowVertical: true, alwaysShowHorizontal: false );
        Params.scrollX.Set( Params.scroll.x );
        Params.scrollY.Set( Params.scroll.y );
        e.OnInspectorGUI();
        GUILayout.EndScrollView();
        
        
        return true;
    }
    
    void ISupportedPostComponent.TopFastButtonsGUI(EditorWindow window, Rect rect)
    {   rect.height = 40;
        GUI.Label( rect, "The fast component button is not available for AmplifyColor" );
        
        GUILayout.Space( 20 );
        
    }
    
    
    // ////////////////////
    //! FieldsHelper *** //
    
    FieldInfo _mBlendAmount;
    FieldInfo BlendAmount { get { return _mBlendAmount ?? (_mBlendAmount = AmplifyBaseType.GetField( "BlendAmount", (BindingFlags)int.MaxValue )); } }
    
    FieldInfo _mLutTexture;
    FieldInfo LutTexture { get { return _mLutTexture ?? (_mLutTexture = AmplifyBaseType.GetField( "LutTexture", (BindingFlags)int.MaxValue )); } }
    
    //! FieldsHelper *** //
    // ////////////////////
}
}
#endif
