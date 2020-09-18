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
class PostPresets_CameraLensEffects : ISupportedPostComponent {
    public const string TITLE = "Final Lens Effects";
    public const string DOWNLOAD_MESSAGE = "Download 'Final Lens Effects' [Wilberforce]";
    public const string DOWNLOAD_LINK = "https://www.assetstore.unity3d.com/#!/content/93120";
    
    string ISupportedPostComponent.GET_TITLE { get { return TITLE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_MESSAGE { get { return DOWNLOAD_MESSAGE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_LINK { get { return DOWNLOAD_LINK; } }
    
    
    public static Type EffectType  = null;
    void ISupportedPostComponent.InitializeTypes()
    {   /* foreach (var assembly in Params.AssemblyList) {
           if (AmplifyEffectType == null) AmplifyEffectType = assembly.GetType( "AmplifyColorEffect", false );
         }*/
        EffectType = Params.GetTypeFromStringName( "CameraLensCommandBuffer" );
        }
    
    string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson( ((ISupportedPostComponent)this).MonoComponent ); }
    void ISupportedPostComponent.CREATE_UNDO(string undoName) { Undo.RecordObject( ((ISupportedPostComponent)this).MonoComponent, undoName ); }
    void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty( ((ISupportedPostComponent)this).MonoComponent ); }
    Type ISupportedPostComponent.MonoComponentType { get { return EffectType; } }
    Type ISupportedPostComponent.SecondMonoComponentType { get { return null; } }
    public void SetMonoComponentDefaultParametrs() { }
    public void SetSecondMonoComponentDefaultParametrs() { }
    
    MonoBehaviour ISupportedPostComponent.MonoComponent { get; set; } //keep this line
    MonoBehaviour ISupportedPostComponent.SecondMonoComponent { get; set; } //keep this line
    
    // SKIP //
    bool ISupportedPostComponent.IsAllowToDraw { get { return true; } }
    bool ISupportedPostComponent.AntiAliasEnable { get { return true; } set { } }
    int ISupportedPostComponent.LutEnable
    {   get { return ((bool)Params.GetFieldValue( "ColorGradingEnabled", ((ISupportedPostComponent)this).MonoComponent, ((ISupportedPostComponent)this).MonoComponentType )) ? 1 : 0; }
        set { Params.SetFieldValue( "ColorGradingEnabled", ((ISupportedPostComponent)this).MonoComponent, (value & 1) != 0, ((ISupportedPostComponent)this).MonoComponentType ); }
    }
    void ISupportedPostComponent.CameraPredrawAction() { }
    void ISupportedPostComponent.CameraPostDrawAction() { }
    // SKIP //
    
    bool ISupportedPostComponent.LutEffectExist
    {   get
        {   if (GRADING_MODE != VARIABLES.gradingMode_LutCase) return false;
            return true;
        }
        set
        {   if (value) GRADING_MODE = VARIABLES.gradingMode_LutCase;
        }
    }
    
    
    static class VARIABLES {
        public static string gradingMode_LutCase           = "LUTTexture"   ;
        public static string GRADING              = "ColorCorrectionMode"   ;
    }
    static class EFFECTS {
        public static string Distortion               = "DistortionEnabled"   ;
        public static string Bloom                      = "BloomEnabled"   ;
        public static string DepthOfField        = "DofEnabled"   ;
        public static string ColorGrading           = "ColorGradingEnabled"   ;
        public static string Vignette               = "VignetteEnabled"   ;
        public static string Chroma               = "ChromaEnabled"   ;
    }
    
    
    public string GRADING_MODE
    {   get
        {   var target = ((ISupportedPostComponent)this).MonoComponent;
            var field = Params.GetField( VARIABLES.GRADING, target, ((ISupportedPostComponent)this).MonoComponentType);
            return Enum.GetName( field.FieldType, Params.GetFieldValue( VARIABLES.GRADING, target, ((ISupportedPostComponent)this).MonoComponentType ) );
            // return (lutLdrTexture.GetValue( target, null )).ToString();
        }
        set
        {   var target = ((ISupportedPostComponent)this).MonoComponent;
            var field = Params.GetField( VARIABLES.GRADING, target, ((ISupportedPostComponent)this).MonoComponentType);
            var res = Enum.Parse( field.FieldType, value );
            if (res != Params.GetFieldValue( VARIABLES.GRADING, target, ((ISupportedPostComponent)this).MonoComponentType ))
            {   Undo.RecordObject( target, target.GetType().Name );
                Params.SetFieldValue( VARIABLES.GRADING, target, res, ((ISupportedPostComponent)this).MonoComponentType );
                EditorUtility.SetDirty( target );
            }
        }
    }
    
    
    Texture2D ISupportedPostComponent.LutTexture
    {   get { return Params.GetFieldValue( "ColorCorrectionLutTexture", ((ISupportedPostComponent)this).MonoComponent, ((ISupportedPostComponent)this).MonoComponentType ) as Texture2D; }
        set { Params.SetFieldValue( "ColorCorrectionLutTexture", ((ISupportedPostComponent)this).MonoComponent, value, ((ISupportedPostComponent)this).MonoComponentType ); }
    }
    
    float ISupportedPostComponent.LutAmount
    {   get { return (float)Params.GetFieldValue( "ColorCorrectionIntensity", ((ISupportedPostComponent)this).MonoComponent, ((ISupportedPostComponent)this).MonoComponentType ); }
        set { Params.SetFieldValue( "ColorCorrectionIntensity", ((ISupportedPostComponent)this).MonoComponent, value, ((ISupportedPostComponent)this).MonoComponentType ); }
    }
    
    
    
    
    
    Dictionary<MonoBehaviour, Editor> p_to_e = new Dictionary<MonoBehaviour, Editor>();
    
    bool ISupportedPostComponent.LeftSideGUI(EditorWindow window, float width)
    {   var c =  ((ISupportedPostComponent)this).MonoComponent;
    
    
        if (!((ISupportedPostComponent)this).LutEffectExist)
        {   GUILayout.Label( "LUT texture disabled", Params.Label );
            if (GUILayout.Button( "enable LUT texture", GUILayout.Height( 200 ) ))
            {   Undo.RecordObject( Params.camera.gameObject, "enable LUT texture" );
                ((ISupportedPostComponent)this).LutEffectExist = true;
                EditorUtility.SetDirty( Params.camera.gameObject );
            }
            //return;
        }
        
        
        if (!p_to_e.ContainsKey( c ))
            p_to_e.Add( c, Editor.CreateEditor( c ) );
        var e = p_to_e[c];
        if (!e)
        {   GUILayout.Label( "Internal Plugin Error", Params.Label );
            return false;
        }
        
        Params.autoRefresh.Set( EditorGUILayout.ToggleLeft( "Automatic refresh when changing", Params.autoRefresh == 1 ) ? 1 : 0 );
        
        GUILayout.Space( 10 );
        
        Params.scroll.x = Params.scrollX;
        Params.scroll.y = Params.scrollY;
        Params.scroll = GUILayout.BeginScrollView( Params.scroll, alwaysShowVertical: true, alwaysShowHorizontal: false );
        Params.scrollX.Set( Params.scroll.x );
        Params.scrollY.Set( Params.scroll.y );
        e.OnInspectorGUI();
        GUILayout.EndScrollView();
        
        return true;
    }
    
    //! TOP FAST BUTTIONS
    void ISupportedPostComponent.TopFastButtonsGUI(EditorWindow window, Rect rect)
    {   rect.width /= 7;
        rect.height = 40;
        
        DrawPostProcessingModelButton( "Distortion", EFFECTS.Distortion, window, rect, 2 );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Bloom", EFFECTS.Bloom, window, rect, 2 );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "DepthOfField", EFFECTS.DepthOfField, window, rect, 2 );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "ColorGrading", EFFECTS.ColorGrading, window, rect, 2 );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Vignette", EFFECTS.Vignette, window, rect, 2 );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Chroma", EFFECTS.Chroma, window, rect, 2 );
        
        GUILayout.Space( 20 );
    }
    void DrawPostProcessingModelButton(string label, string field, EditorWindow window, Rect rect, int offset)
    {   var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
        var enable = (bool)Params.GetFieldValue(field, currentComponent, ((ISupportedPostComponent)this).MonoComponentType);
        if (enable) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
        EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
        if ( Params.TransparentButton( rect, Params.CONTENT( label, "enable/disable " + label.Replace( '\n', ' ' ) ) ))
        {   Undo.RecordObject( currentComponent, "enable/disable" );
            Params.SetFieldValue( field, currentComponent, !enable, ((ISupportedPostComponent)this).MonoComponentType );
            EditorUtility.SetDirty( currentComponent );
            Params.RepaintImages();
        }
    } //! TOP FAST BUTTIONS
    
    
    
    // ////////////////////
    //! FieldsHelper *** //
    
    //! FieldsHelper *** //
    // ////////////////////
}
}
#endif
