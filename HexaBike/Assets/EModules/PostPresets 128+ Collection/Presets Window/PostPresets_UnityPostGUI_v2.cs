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
class PostPresets_UnityPostGUI_v2 : ISupportedPostComponent {
    public const string TITLE = "Unity PostProcessing v2.0 [FREE]";
    public const string DOWNLOAD_MESSAGE = "[FREE] Download Unity 'PostProcessing Stack v2.0'";
    public const string DOWNLOAD_LINK = "https://github.com/Unity-Technologies/PostProcessing";
    
    public const string NAMESPACE = "UnityEngine.Rendering.PostProcessing";
    const string PROFILEFIELD = "sharedProfile";
    
    string ISupportedPostComponent.GET_TITLE { get { return TITLE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_MESSAGE { get { return DOWNLOAD_MESSAGE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_LINK { get { return DOWNLOAD_LINK; } }
    
    public static Type PostProcessingBehaviourType = null;
    public static Type PostProcessingProfileType = null;
    public static Type SecondPostProcessingBehaviourType = null;
    void ISupportedPostComponent.InitializeTypes()
    {   /* foreach (var assembly in Params.AssemblyList) {
           if (PostProcessingBehaviourType == null) PostProcessingBehaviourType = assembly.GetTypes().FirstOrDefault(t=>t.Name == "PostProcessingBehaviour" );
           if (PostProcessingProfileType == null) PostProcessingProfileType = assembly.GetType( "PostProcessingProfile", false, true);
        
         }
         Debug.Log( PostProcessingBehaviourType + " " + Params.AssemblyList.Length ); */
        /*Debug.Log( Params.GetTypeFromStringFullName( EFFECTS.AmbientOcclusion ) );
        Debug.Log( Params.AssemblyTypesListFullNames.Keys.First( k => k.Contains( NAMESPACE ) ) );*/
        //var mono = typeof(UnityEngine.Object);
        //  Debug.Log( Params.AssemblyTypesListNames.First( k => !(!mono.IsAssignableFrom( k.Value ) ) ) ) ;
        
        SecondPostProcessingBehaviourType = Params.GetTypeFromStringName( "PostProcessLayer" );
        PostProcessingBehaviourType = Params.GetTypeFromStringName( "PostProcessVolume" );
        PostProcessingProfileType = Params.GetTypeFromStringName( "PostProcessProfile" );
        
        if ( PostProcessingBehaviourType != null && (PostProcessingProfileType == null || SecondPostProcessingBehaviourType == null) )
        {   Debug.LogWarning( "Missing PostProcessProfile or PostProcessLayer" );
            PostProcessingBehaviourType = null;
        }
    }
    
    MonoBehaviour m_MonoComponent { get; set; }
    MonoBehaviour ISupportedPostComponent.MonoComponent
    {   get { return m_MonoComponent; } set
        {   m_MonoComponent = value;
            #if UNITY_POST_PROCESSING_STACK_V2
            if ( value )
            {   var volume = (UnityEngine.Rendering.PostProcessing.PostProcessVolume)value;
                profile = (UnityEngine.Rendering.PostProcessing.PostProcessProfile)volume.profile;
            }
            else profile = null;
            #endif
        }
    }
    MonoBehaviour ISupportedPostComponent.SecondMonoComponent { get; set; }
    
    string ISupportedPostComponent.GetHashString()
    {   Updaterofile();
        var res = "";
        if ( profile ) res += EditorJsonUtility.ToJson( profile );
        if ( ((ISupportedPostComponent)this).MonoComponent ) res += EditorJsonUtility.ToJson( ((ISupportedPostComponent)this).MonoComponent );
        if ( ((ISupportedPostComponent)this).SecondMonoComponent ) res += EditorJsonUtility.ToJson( ((ISupportedPostComponent)this).SecondMonoComponent );
        return  res ;
    }
    void ISupportedPostComponent.CREATE_UNDO( string undoName )
    {   Updaterofile();
        if ( profile) Undo.RecordObject( profile, undoName );
    }
    void ISupportedPostComponent.SET_DIRTY()
    {   Updaterofile();
        if ( profile) EditorUtility.SetDirty( profile );
    }
    bool ISupportedPostComponent.IsAllowToDraw
    {   get
        {   Updaterofile();
            return profile;
        }
    }
    Type ISupportedPostComponent.MonoComponentType { get { return PostProcessingBehaviourType; } }
    Type ISupportedPostComponent.SecondMonoComponentType { get { return SecondPostProcessingBehaviourType; } }
    
    
    
    void CHECK_AA( MonoBehaviour comp )
    {   if ( antialiasingMode_f == null ) antialiasingMode_f = Params.GetField( "antialiasingMode", comp );
        if ( targetAAres == null )
        {   var  targetAA = Enum.GetNames( antialiasingMode_f.FieldType ).First( n => n.ToLower().Contains( "TemporalAntialiasing".ToLower() ) || n.ToLower().Contains( "TAA".ToLower() ) );
            targetAAres = Enum.Parse( antialiasingMode_f.FieldType, targetAA );
            var  targetBB = Enum.GetNames( antialiasingMode_f.FieldType ).First( n => n.ToLower().Contains( "none".ToLower() ) || n.ToLower().Contains( "disable".ToLower() ) );
            targetAAnone = Enum.Parse( antialiasingMode_f.FieldType, targetBB );
        }
    }
    FieldInfo antialiasingMode_f;
    string targetAA;
    object targetAAres;
    object targetAAnone;
    
    bool ISupportedPostComponent.AntiAliasEnable
    {   get
        {   Updaterofile(); var comp = ((ISupportedPostComponent)this).SecondMonoComponent;
            if ( !comp ) return false;
            CHECK_AA( comp );
            if ( targetAAres != null && targetAAnone != null )
            {   return antialiasingMode_f.GetValue( comp ).ToString() == targetAAres.ToString();
            }
            return false;
        }
        set
        {   Updaterofile(); var comp = ((ISupportedPostComponent)this).SecondMonoComponent;
            if ( !comp ) return;
            CHECK_AA( comp );
            if ( targetAAres != null && targetAAnone != null )
            {   if ( antialiasingMode_f.GetValue( comp ).ToString() != targetAAres.ToString() && antialiasingMode_f.GetValue( comp ).ToString() != targetAAnone.ToString() ) return;
                antialiasingMode_f.SetValue( comp, value ? targetAAres : targetAAnone );
            }
        }
    }
    
    bool ISupportedPostComponent.LutEffectExist
    {   get
        {   Updaterofile(); var model = GetPostprocessingModel( EFFECTS.ColorGrading, profile );
            if ( model == null ) return false;
            if ( model.GRADING_MODE != VARIABLES.LowDefinitionRange ) return false;
            if ( !model.enabled ) return false;
            if ( !model.GRADING_MODE_ENABLE ) return false;
            if ( !model.LUT_TEXTURE_ENABLE ) return false;
            if ( !model.LUT_AMOUNT_ENABLE ) return false;
            return true;
        }
        set
        {   Updaterofile();
            if ( value )
            {   AddModel( EFFECTS.ColorGrading, profile ).enabled = true;
                AddModel( EFFECTS.ColorGrading, profile ).GRADING_MODE_ENABLE = true;
                AddModel( EFFECTS.ColorGrading, profile ).LUT_TEXTURE_ENABLE = true;
                AddModel( EFFECTS.ColorGrading, profile ).LUT_AMOUNT_ENABLE = true;
            }
        }
    }
    
    int ISupportedPostComponent.LutEnable
    {   get
        {   Updaterofile();
            if ( !((ISupportedPostComponent)this).LutEffectExist ) return 0;
            var model = GetPostprocessingModel( EFFECTS.ColorGrading, profile );
            if ( model == null ) return 0;
            int result = 0;
            if ( model.enabled ) result |= 1 << 0;
            if ( model.GRADING_MODE_ENABLE ) result |= 1 << 1;
            if ( model.LUT_AMOUNT_ENABLE ) result |= 1 << 2;
            if ( model.LUT_TEXTURE_ENABLE ) result |= 1 << 3;
            return result;
        }
        set
        {   Updaterofile();
            if ( !((ISupportedPostComponent)this).LutEffectExist ) return;
            var model = GetPostprocessingModel( EFFECTS.ColorGrading, profile );
            if ( model == null ) return;
            model.enabled = ((value & 1 << 0) != 0);
            model.GRADING_MODE_ENABLE = ((value & 1 << 1) != 0);
            model.LUT_AMOUNT_ENABLE = ((value & 1 << 2) != 0);
            model.LUT_TEXTURE_ENABLE = ((value & 1 << 3) != 0);
            // GetPostprocessingModel( EFFECTS.ColorGrading, profile ).enabled = value;
        }
    }
    
    void Updaterofile()
    {
        #if UNITY_POST_PROCESSING_STACK_V2
        if ( !((ISupportedPostComponent)this).MonoComponent ) return;
        var volume = (UnityEngine.Rendering.PostProcessing.PostProcessVolume)  ((ISupportedPostComponent)this).MonoComponent;
        profile = (UnityEngine.Rendering.PostProcessing.PostProcessProfile)volume.profile;
        #endif
    }
    Texture2D ISupportedPostComponent.LutTexture
    {   get
        {   Updaterofile();
            var model = GetPostprocessingModel(EFFECTS.ColorGrading, profile);
            if ( model == null ) return null;
            return model.LUT_TEXTURE as Texture2D;
        }
        set
        {   Updaterofile();
            var model = GetPostprocessingModel(EFFECTS.ColorGrading, profile);
            if ( model == null ) return;
            model.LUT_TEXTURE = value;
        }
    }
    
    float ISupportedPostComponent.LutAmount
    {   get
        {   Updaterofile();
            var model = GetPostprocessingModel(EFFECTS.ColorGrading, profile);
            if ( model == null ) return 1;
            return model.LUT_AMOUNT;
        }
        set
        {   Updaterofile();
            var model = GetPostprocessingModel(EFFECTS.ColorGrading, profile);
            if ( model == null ) return;
            model.LUT_AMOUNT = value;
        }
    }
    
    
    /*
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
    NAMESPACE + "." +
      */
    
    
    static class EFFECTS {
        public static string AO           = "AmbientOcclusion"   ;
        public static string AutoExpo               = "AutoExposure"   ;
        public static string Bloom                      = "Bloom"   ;
        public static string ChromaAb       = "ChromaticAberration"   ;
        public static string ColorGrading               = "ColorGrading"   ;
        public static string DepthOfField               = "DepthOfField"   ;
        // public static string Fog                        = "Fog"   ;
        public static string Grain                      = "Grain"   ;
        public static string LensDistor             = "LensDistortion"   ;
        public static string MotionBlur                 = "MotionBlur"   ;
        public static string Reflections     = "ScreenSpaceReflections";
    }
    
    
    static class VARIABLES {
        public static string LowDefinitionRange           = "LowDefinitionRange"   ;
        public static string gradingMode           = "gradingMode"   ;
        public static string ldrLut           = "ldrLut"   ;
        public static string ldrLutContribution           = "ldrLutContribution"   ;
    }
    
    
    
    
    
    ScriptableObject profile;
    Dictionary<int, Editor> p_to_e = new Dictionary<int, Editor>();
    
    const int LAST_COUNT = 5;
    //static Type ModelType;
    
    
    ////////////////////////////
    //! POSTPROCESSING COMPONENT GUI *** //
    
    void SET_PROFILE( MonoBehaviour currentComponent, ScriptableObject __profile )
    {   /*profile = Params.SetFieldValue( PROFILEFIELD, currentComponent, __profile ) as ScriptableObject;
        // var o = new SerializedObject(profile);
        // o.Update();
        Params.SetPropertyValue( "profile", currentComponent, null );*/
        #if UNITY_POST_PROCESSING_STACK_V2
        var volume = (UnityEngine.Rendering.PostProcessing.PostProcessVolume)  ((ISupportedPostComponent)this).MonoComponent;
        profile = (UnityEngine.Rendering.PostProcessing.PostProcessProfile)__profile;
        #endif
        
        
        ResetEditor();
        
        currentComponent.enabled = false;
        
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        
        currentComponent.enabled = true;
        //  o.ApplyModifiedProperties();
    }
    
    void ResetEditor()
    {   p_to_e.Clear();
    
    }
    void SET_DEFAULTPROFILE( MonoBehaviour currentComponent )
    {   var defaultProfile =  AssetDatabase.LoadAssetAtPath( Params.EditorResourcesPath + "/CameraProfile Unity PostProcessing 2.0.asset", PostProcessingProfileType ) as ScriptableObject;
        if ( !defaultProfile ) defaultProfile = CreateProfile( Params.EditorResourcesPath + "/CameraProfile Unity PostProcessing 2.0.asset" );
        Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
        //Params.SetPropertyValue( "profile", currentComponent, defaultProfile );
        SET_PROFILE( currentComponent, defaultProfile );
        SetLast( defaultProfile );
        EditorUtility.SetDirty( currentComponent );
        EditorUtility.SetDirty( Params.camera.gameObject );
    }
    MonoBehaviour lastcurrentComponent;
    UnityEngine.Object lastVolume, lastProfile;
    
    bool ISupportedPostComponent.LeftSideGUI( EditorWindow window, float width )
    {
    
    
        #if UNITY_POST_PROCESSING_STACK_V2
        var volume = (UnityEngine.Rendering.PostProcessing.PostProcessVolume)  ((ISupportedPostComponent)this).MonoComponent;
        var layer = (UnityEngine.Rendering.PostProcessing.PostProcessLayer)  ((ISupportedPostComponent)this).SecondMonoComponent;
        profile = volume.profile;
        // var volume = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
        if ( lastcurrentComponent  != volume )
        {   lastcurrentComponent = volume;
            foreach ( var item in p_to_e )
            {   if ( item.Value ) UnityEngine.Object.DestroyImmediate( item.Value );
            }
            p_to_e.Clear();
        }
        
        
        if ( lastVolume  != volume || lastProfile != profile )
        {   lastVolume = volume; lastProfile = profile;
            p_to_e.Clear();
        }
        
        //profile = currentComponent.profile;
        //profile = Params.GetPropertyValue( "profile", currentComponent ) as ScriptableObject;
        
        
        /*profile = Params.GetFieldValue( PROFILEFIELD, currentComponent ) as ScriptableObject;
        
        var oc = GUI.color;
        if ( !profile ) GUI.color *= Color.red;
        var changedProfile = EditorGUILayout.ObjectField( profile, PostProcessingProfileType, false ) as ScriptableObject;
        GUI.color = oc;
        if ( changedProfile != profile )
        {   Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
        //profile = Params.SetPropertyValue( "profile", currentComponent, changedProfile ) as ScriptableObject;
        SET_PROFILE( currentComponent, changedProfile );
        SetLast( changedProfile );
        EditorUtility.SetDirty( currentComponent );
        EditorUtility.SetDirty( Params.camera.gameObject );
        }
        
        
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button( Params.CONTENT( "Default Profile", "Set Default Profile" ) ) )
        {   SET_DEFAULTPROFILE( currentComponent );
        }
        GUI.enabled = profile;
        if ( GUILayout.Button( Params.CONTENT( "New Copy", "Create and set a copy of current profile" ) ) )
        {   var json = EditorJsonUtility.ToJson(profile);
        var path = AssetDatabase.GenerateUniqueAssetPath( "Assets/NewCameraProfile.asset" );
        var newProfile = CreateProfile(path );
        EditorJsonUtility.FromJsonOverwrite( json, newProfile );
        EditorUtility.SetDirty( newProfile );
        AssetDatabase.ImportAsset( path, ImportAssetOptions.ForceUpdate );
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
        //Params.SetPropertyValue( "profile", currentComponent, newProfile );
        SET_PROFILE( currentComponent, newProfile );
        SetLast( newProfile );
        EditorUtility.SetDirty( currentComponent );
        EditorUtility.SetDirty( Params.camera.gameObject );
        }
        GUI.enabled = true;
        
        
        GUILayout.EndHorizontal();
        
        if ( !profile )
        {   return false;
        }*/
        
        //ModelType = GetField( "fog", profile ).FieldType.BaseType;
        if ( !((ISupportedPostComponent)this).LutEffectExist )
        {   var c = GUI.color;
            GUI.color *= Color.red;
            GUILayout.Label( "LUT texture disabled", Params.Label );
            GUI.color = c;
            var res = GUILayout.Button( "Enable:\nColor Grading\nLow Definition Range\nLookup Texture\nContributions", GUILayout.Height( 80 ));
            if (  res )
            {   Undo.RecordObject( Params.camera.gameObject, "enable LUT texture" );
                ((ISupportedPostComponent)this).LutEffectExist = true;
                EditorUtility.SetDirty( Params.camera.gameObject );
            }
            // return false;
        }
        
        /* if ( !p_to_e.ContainsKey( profile.GetInstanceID() ) )
             p_to_e.Add( profile.GetInstanceID(), Editor.CreateEditor( profile ) );
         var e = p_to_e[profile.GetInstanceID()];
         if ( !e )
         {   GUILayout.Label( "Internal Plugin Error", Params.Label );
             return false;
         }*/
        if ( !p_to_e.ContainsKey( volume.GetInstanceID() ) )
            p_to_e.Add( volume.GetInstanceID(), Editor.CreateEditor( volume ) );
        var e = p_to_e[volume.GetInstanceID()];
        if ( !e )
        {   GUILayout.Label( "Internal Plugin Error", Params.Label );
            return false;
        }
        
        
        GUILayout.Space( 10 );
        
        
        Params.scroll.x = Params.scrollX;
        Params.scroll.y = Params.scrollY;
        Params.scroll = GUILayout.BeginScrollView( Params.scroll, alwaysShowVertical: true, alwaysShowHorizontal: false );
        Params.scrollX.Set( Params.scroll.x );
        Params.scrollY.Set( Params.scroll.y );
        try
        {   e.OnInspectorGUI();
        }
        catch
        {   ResetEditor();
        }
        
        GUILayout.EndScrollView();
        if ( GUILayout.Button( "Recreate Inspector" ) ) ResetEditor();
        #endif
        return true;
        
    } //! POSTPROCESSING COMPONENT GUI
    
    MethodInfo AddSettings;
    
    PostProcessingModelFake AddModel( string name, ScriptableObject profile )
    {   if ( profile == null ) return null;
        var result = GetPostprocessingModel( name, profile );
        if ( result == null )
        {   if ( AddSettings == null ) AddSettings = profile.GetType().GetMethods( (BindingFlags)int.MaxValue ).FirstOrDefault( m => m.Name == "AddSettings" && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType == typeof( Type ) );
            if ( AddSettings == null ) throw new Exception( "Cannot find AddSettings method" );
            AddSettings.Invoke( profile, new[] { Params.GetTypeFromStringName( name ) } );
        }
        result = GetPostprocessingModel( name, profile );
        
        result.enabled = true;
        if ( name == EFFECTS.ColorGrading )
        {   result.GRADING_MODE = VARIABLES.LowDefinitionRange;
        }
        return result;
    }
    
    
    ////////////////////////////
    //! TOP FAST BUTTIONS *** //
    void ISupportedPostComponent.TopFastButtonsGUI( EditorWindow window, Rect rect )
    {   rect.width /= 7;
        rect.height = 40;
        DrawPostProcessingModelButton( "AO", EFFECTS.AO, GetPostprocessingModel( EFFECTS.AO, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Reflections", EFFECTS.Reflections, GetPostprocessingModel( EFFECTS.Reflections, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Bloom", EFFECTS.Bloom, GetPostprocessingModel( EFFECTS.Bloom, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "ChromaAb", EFFECTS.ChromaAb, GetPostprocessingModel( EFFECTS.ChromaAb, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "MotionBlur", EFFECTS.MotionBlur, GetPostprocessingModel( EFFECTS.MotionBlur, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "DepthOfField", EFFECTS.DepthOfField, GetPostprocessingModel( EFFECTS.DepthOfField, profile ), window, rect );
        // rect.x += rect.width;
        // DrawPostProcessingModelButton( EFFECTS.Fog, GetPostprocessingModel( EFFECTS.Fog, profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "Grain", EFFECTS.Grain, GetPostprocessingModel( EFFECTS.Grain, profile ), window, rect );
        rect.x += rect.width;
        
        
        GUILayout.Space( 20 );
    }
    
    ScriptableObject CreateProfile( string path )
    {
    
        var __profile = ScriptableObject.CreateInstance(PostProcessingProfileType);
        __profile.name = Path.GetFileName( path );
        //AddModel( EFFECTS.Fog, profile );
        //GetPostprocessingModel( "fog", profile ).enabled = true;
        AssetDatabase.CreateAsset( __profile, path );
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return __profile;
    }
    
    
    void DrawPostProcessingModelButton( string label, string name, PostProcessingModelFake model, EditorWindow window, Rect rect )         // if ()
    {   if ( model != null && model.enabled ) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
        EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
        if ( Params.TransparentButton( rect, Params.CONTENT( label, "enable/disable " + label.Replace( '\n', ' ' ) ) ) )
        {   Undo.RecordObject( profile, "enable/disable " + label );
        
            if ( model == null ) AddModel( name, profile ).enabled = true;
            else model.enabled = !model.enabled;
            
            EditorUtility.SetDirty( profile );
            
            Params.RepaintImages();
        }
    } //! TOP FAST BUTTIONS
    
    
    
    class ParameterFake {
        public Type VALUE_TYPE { get { return value_field.FieldType; } }
        
        FieldInfo main, value_field, overrideState_field;
        
        public ParameterFake( string name, UnityEngine.Object target )
        {   main = target.GetType().GetField( name, (BindingFlags)int.MaxValue );
            value_field = main.FieldType.BaseType.GetField( "value", (BindingFlags)int.MaxValue );
            overrideState_field = main.FieldType.BaseType.BaseType.GetField( "overrideState", (BindingFlags)int.MaxValue );
            // Debug.Log( "11_" );
            //Debug.Log( main.FieldType.BaseType.GetFields( (BindingFlags)int.MaxValue )[0] );
        }
        public object GetValue( UnityEngine.Object target, object defaultValue )
        {   if ( !target ) return defaultValue;
            var v = main.GetValue(target);
            return value_field.GetValue( v );
        }
        public void SetValue( UnityEngine.Object target, object value )
        {   if ( !target ) return;
            var v = main.GetValue(target);
            Undo.RecordObject( target, target.GetType().Name );
            value_field.SetValue( v, value );
            EditorUtility.SetDirty( target );
        }
        
        
        public bool GetEnableState( UnityEngine.Object target )
        {   if ( !target ) return false;
            var v = main.GetValue(target);
            return (bool)overrideState_field.GetValue( v );
        }
        public void SetEnableState( UnityEngine.Object target, bool value )
        {   if ( !target ) return;
            var v = main.GetValue(target);
            Undo.RecordObject( target, target.GetType().Name );
            overrideState_field.SetValue( v, value );
            EditorUtility.SetDirty( target );
        }
    }
    
    class PostProcessingModelFake {
        static ParameterFake gradingMode;
        static ParameterFake lutLdrTexture;
        static ParameterFake lutAmount;
        
        public string GRADING_MODE
        {   get
            {   return Enum.GetName( gradingMode.VALUE_TYPE, gradingMode.GetValue( target, null ) );
            }
            set
            {   var res = Enum.Parse( gradingMode.VALUE_TYPE, value );
                if ( res != null ) gradingMode.SetValue( target, res );
            }
        }
        public bool GRADING_MODE_ENABLE
        {   get { return (gradingMode.GetEnableState( target )); }
            set { gradingMode.SetEnableState( target, value ); }
        }
        
        public Texture LUT_TEXTURE
        {   get { return (lutLdrTexture.GetValue( target, null )) as Texture; }
            set { lutLdrTexture.SetValue( target, value ); }
        }
        public bool LUT_TEXTURE_ENABLE
        {   get { return (lutLdrTexture.GetEnableState( target )); }
            set { lutLdrTexture.SetEnableState( target, value ); }
        }
        
        public float LUT_AMOUNT
        {   get { return (float)(lutAmount.GetValue( target, 1 )); }
            set { lutAmount.SetValue( target, value ); }
        }
        public bool LUT_AMOUNT_ENABLE
        {   get { return (lutAmount.GetEnableState( target )); }
            set { lutAmount.SetEnableState( target, value ); }
        }
        
        public PostProcessingModelFake( UnityEngine.Object target )
        {   this.target = target;
            if ( target.GetType().Name == EFFECTS.ColorGrading )
            {   if ( lutLdrTexture == null )
                {   gradingMode = new ParameterFake( VARIABLES.gradingMode, target );
                    lutLdrTexture = new ParameterFake( VARIABLES.ldrLut, target );
                    lutAmount = new ParameterFake( VARIABLES.ldrLutContribution, target );
                }
            }
        }
        UnityEngine.Object target;
        /* PropertyInfo _mEnabledProp = null;
         PropertyInfo enabledProp { get {
        
             return _mEnabledProp ?? (_mEnabledProp = ModelType.GetProperty( "enabled", (BindingFlags)int.MaxValue )); } }*/
        
        public bool enabled
        {   get
            {   var serializedObject = new SerializedObject( target );
                var m_Enabled3 = serializedObject.FindProperty( "active" );
                var m_Enabled = serializedObject.FindProperty( "enabled.value" );
                var m_Enabled2 = serializedObject.FindProperty( "enabled.overrideState" );
                return m_Enabled.boolValue && m_Enabled2.boolValue && m_Enabled3.boolValue;
            }
            set
            {   var serializedObject = new SerializedObject( target );
                var m_Enabled3 = serializedObject.FindProperty( "active" );
                var m_Enabled = serializedObject.FindProperty( "enabled.value" );
                var m_Enabled2 = serializedObject.FindProperty( "enabled.overrideState" );
                m_Enabled.boolValue = value;
                if (value)     m_Enabled2.boolValue = value;
                if (value) m_Enabled3.boolValue = value;
                serializedObject.ApplyModifiedProperties();
            }
        }
        /* public object settings {
           get { return GetFieldValue( "m_Settings", target ); }
           set { SetFieldValue( "m_Settings", target, value ); }
         }*/
        
    }
    
    
    
    // Dictionary<ScriptableObject, Dictionary<string, PostProcessingModelFake>> _mPostModelsFields = new Dictionary<ScriptableObject,Dictionary<string, PostProcessingModelFake>>();
    PostProcessingModelFake GetPostprocessingModel( string name,
            ScriptableObject profile )      // if (!_mPostModelsFields.ContainsKey( profile )) _mPostModelsFields.Add( profile, new Dictionary<string, PostProcessingModelFake>() );
    {   // if (!_mPostModelsFields[profile].ContainsKey( name )) {
    
        var p  = profile;
        if ( p == null ) return null;
        
        var type = Params.GetTypeFromStringName(name);
        foreach ( var e in Params.GetFieldValue( "settings", p ) as IList )      //  Debug.Log( e.GetType() );
        {   if ( e.GetType() == type ) return new PostProcessingModelFake( e as UnityEngine.Object );
        
        }
        
        /* var m_SerializedObject = new SerializedObject( p );
         var m_SettingsProperty = m_SerializedObject.FindProperty( "settings" );
         var type = Params.GetTypeFromStringName(name);
         for (int i = 0 ; i < m_SettingsProperty.arraySize ; i++) {
           var set = m_SettingsProperty.GetArrayElementAtIndex( i ). objectReferenceValue ;
           if (set.GetType() == type) return new PostProcessingModelFake( set );
           //_mPostModelsFields[profile].Add( name, new PostProcessingModelFake( set ) );
           // if (set.)
         }*/
        return null;
        /* var target = PostProcessingProfileType.GetField( name, (BindingFlags)int.MaxValue ).GetValue( profile );
         _mPostModelsFields[profile].Add( name, new PostProcessingModelFake( target ) );*/
        // }
        // return _mPostModelsFields[profile][name];
    }
    
    
    
    
    /////////////////
    //! LAST PROFILES *** //
    
    List<ScriptableObject> _mLastList;
    List<ScriptableObject> LastList
    {   get
        {   if ( _mLastList == null )
            {   _mLastList = new List<ScriptableObject>();
                for ( int i = 0 ; i < LAST_COUNT ; i++ )
                {   var guid = EditorPrefs.GetString( "EModules/" + Params.TITLE + "/LastProfilesv2" + i, "" );
                    if ( string.IsNullOrEmpty( guid ) ) continue;
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if ( string.IsNullOrEmpty( path ) ) continue;
                    var pr = AssetDatabase.LoadAssetAtPath(path, PostProcessingProfileType) as ScriptableObject;
                    if ( pr ) _mLastList.Add( pr );
                }
            }
            return _mLastList;
        }
        set
        {   while ( value.Count > LAST_COUNT ) value.RemoveAt( LAST_COUNT );
            for ( int i = 0 ; i < value.Count ; i++ )
            {   if ( !value[i] ) continue;
                var path = AssetDatabase.GetAssetPath( value[i] );
                if ( string.IsNullOrEmpty( path ) ) continue;
                var guid = AssetDatabase.AssetPathToGUID(path);
                if ( string.IsNullOrEmpty( guid ) ) continue;
                EditorPrefs.SetString( "EModules/" + Params.TITLE + "/LastProfilesv2" + i, guid );
            }
            _mLastList = value;
        }
    }
    
    
    
    void SetLast( ScriptableObject newProfile )
    {   if ( !newProfile ) return;
        var list = LastList;
        list.Remove( newProfile );
        if ( list.Count == 0 ) list.Add( newProfile );
        else list.Insert( 0, newProfile );
        LastList = list;
    }
    
    //! LAST PROFILES *** //
    /////////////////
    
    
    
    #if UNITY_POST_PROCESSING_STACK_V2
#pragma warning disable
    MonoBehaviour lastGlobalM;
    bool lastGlobal;
    System.Reflection.MethodInfo updateSettibgs;
    bool wasUpdated;
#pragma warning restore
    #endif
    
    
    ////////////////////////////
    //! PRESETS GRID GUI *** //
    // RenderTexture[][] m_defaultHistory;
#pragma warning disable
    RenderTexture[][] HT = new RenderTexture[2][];
    FieldInfo fieldTaa, fieldm_ResetHistory, fieldm_HistoryTexture;
    bool need_postDraw;
    
#pragma warning restore
    void ISupportedPostComponent.CameraPredrawAction()
    {
    
        var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
        
        
        #if UNITY_POST_PROCESSING_STACK_V2
        if ( lastGlobalM )
        {   ((UnityEngine.Rendering.PostProcessing.PostProcessVolume)lastGlobalM).isGlobal = lastGlobal;
            lastGlobalM = null;
        }
        if ( Params.selectedVolume != -1 )
        {   /*var volum = currentComponent as UnityEngine.Rendering.PostProcessing.PostProcessVolume;
            lastGlobal = volum.isGlobal;
            volum.isGlobal = true;
            lastGlobalM = volum;*/
            
            
            // var layer = component.SecondMonoComponent as UnityEngine.Rendering.PostProcessing.PostProcessLayer;
            
            /* var cont =   layer.GetType().GetField( "m_TargetPool", (System.Reflection.BindingFlags)(-1) ).GetValue(layer);
             cont.GetType().GetMethod( "Reset", (System.Reflection.BindingFlags)(-1) ).Invoke( cont, null );
            
            
             foreach ( var item in UnityEngine.Rendering.PostProcessing.PostProcessManager.instance.GetType() .GetFields())
             {
            
                 if ( item.Name.Contains( "m_SortedVolumes" ) ) Debug.Log( item.Name );
             }*///22
            /*     var cont2 =   UnityEngine.Rendering.PostProcessing.PostProcessManager.instance.GetType().GetField( "m_SortedVolumes", (System.Reflection.BindingFlags)(-1) ).GetValue(layer);
                 cont2.GetType().GetMethod( "Clear", (System.Reflection.BindingFlags)(-1) ).Invoke( cont2, null );*/
            
            /*  PostProcessManager.instance.UpdateSettings( this , cam );
              m_TargetPool.Reset();*/
            
            /*if ( updateSettibgs == null && !wasUpdated )
            {   wasUpdated = true;
                updateSettibgs = UnityEngine.Rendering.PostProcessing.PostProcessManager.instance.GetType().GetMethod( "UpdateSettings", (System.Reflection.BindingFlags)(-1) );
            }
            if ( updateSettibgs != null ) updateSettibgs.Invoke( UnityEngine.Rendering.PostProcessing.PostProcessManager.instance, new object[2]
            {   layer, Params.camera
            } );*///33
            //UnityEngine.Rendering.PostProcessing.RuntimeUtilities.
            /* if ( updateSettibgs == null && !wasUpdated )
             {   wasUpdated = true;
                 updateSettibgs = UnityEngine.Rendering.PostProcessing.PostProcessManager.instance.GetType().GetMethod( "SetLayerDirty", (System.Reflection.BindingFlags)(-1) );
             }
            
             if ( updateSettibgs != null ) updateSettibgs.Invoke( UnityEngine.Rendering.PostProcessing.PostProcessManager.instance, new object[1]
             {   volum.gameObject.layer
             } );*/
            
            
        }
        #endif
        
        need_postDraw = false;
        if ( ((ISupportedPostComponent)this).AntiAliasEnable )
        {
        
        
            if ( fieldTaa == null ) fieldTaa = Params.GetField( "temporalAntialiasing", currentComponent );
            if ( fieldTaa != null && Params.GetFieldValue( "temporalAntialiasing", currentComponent ) != null )
            {   var taa = Params.GetFieldValue("temporalAntialiasing",  currentComponent );
            
                if ( fieldm_HistoryTexture == null )
                {   fieldm_ResetHistory = Params.GetField( "m_ResetHistory", taa );
                    fieldm_HistoryTexture = Params.GetField( "m_HistoryTextures", taa );
                    
                }
                
                
                if ( fieldm_ResetHistory != null && fieldm_HistoryTexture != null )
                {   var  m_HistoryTexture = Params.GetFieldValue( "m_HistoryTextures", taa )as RenderTexture[][];
                
                    if ( m_HistoryTexture != null )
                    {   need_postDraw = true;
                    
                        COPY_RT( ref m_HistoryTexture, ref HT );
                        // if (m_defaultHistory == null) m_defaultHistory                   = new RenderTexture[(int)Params.GetFieldValue( "k_NumEyes", null, taa.GetType() )][];
                        Params.SetFieldValue( "m_HistoryTextures", taa, m_HistoryTexture );
                    }
                }
            }
        }
    }
    
    
    void COPY_RT( ref RenderTexture[][] FROM, ref RenderTexture[][] TO )
    {   if ( TO.Length != FROM.Length ) Array.Resize( ref TO, FROM.Length );
        for ( int i = 0 ; i < FROM.Length ; i++ )
        {   if ( FROM[i] == null )
            {   TO[i] = null;
                continue;
            }
            if ( TO[i] == null ) TO[i] = new RenderTexture[0];
            if ( TO[i].Length != FROM[i].Length ) Array.Resize( ref TO[i], FROM[i].Length );
            for ( int x = 0 ; x < TO[i].Length ; x++ )
            {   TO[i][x] = FROM[i][x];
            }
            FROM[i] = null;
        }
    }
    
    void ISupportedPostComponent.CameraPostDrawAction()
    {
    
        #if UNITY_POST_PROCESSING_STACK_V2
        if ( lastGlobalM )
        {   ((UnityEngine.Rendering.PostProcessing.PostProcessVolume)lastGlobalM).isGlobal = lastGlobal;
            lastGlobalM = null;
        }
        #endif
        
        if ( need_postDraw )
        {   var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).SecondMonoComponent;
            var taa = Params.GetFieldValue("temporalAntialiasing",  currentComponent );
            Params.SetFieldValue( "m_ResetHistory", taa, false );
            
            var  m_HistoryTexture = Params.GetFieldValue( "m_HistoryTextures", taa )as RenderTexture[][];
            
            COPY_RT( ref HT, ref m_HistoryTexture );
            
            Params.SetFieldValue( "m_HistoryTextures", taa, m_HistoryTexture );
        }
    }//! PRESETS GRID GUI
    
    public void SetMonoComponentDefaultParametrs()
    {   var comp = ((ISupportedPostComponent)this).MonoComponent;
        if ( !comp ) return;
        
        Params.SetFieldValue( "isGlobal", comp, true );
        if ( profile ) SET_PROFILE( comp, profile );
        else SET_DEFAULTPROFILE( comp );
    }
    
    public void SetSecondMonoComponentDefaultParametrs()
    {
    
        var comp = ((ISupportedPostComponent)this).SecondMonoComponent;
        if ( !comp ) return;
        
        //AA
        //((ISupportedPostComponent)this).AntiAliasEnable = true;
        CHECK_AA( comp );
        if ( targetAAres != null && targetAAnone != null )
        {   antialiasingMode_f.SetValue( comp, targetAAres );
        }
        
        //FOG
        var fog_f = Params.GetFieldValue( "fog", comp );
        Params.SetFieldValue( "enabled", fog_f, true );
        
        //LAYERS
        Params.SetFieldValue( "volumeLayer", comp, new LayerMask() { value = int.MaxValue } );
    }
    
    
    /* void ISupportedPostComponent.CameraPredrawAction()
     {
       need_postDraw = false;
    
       if (profile.antialiasing.enabled && profile.antialiasing.settings.method == AntialiasingModel.Method.Taa)
       {
         var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
         var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
         var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
         if (m_Taa != null && m_ResetHistory != null && m_HistoryTexture != null)
         {
           var taa = m_Taa.GetValue( currentComponent ) as TaaComponent;
           if (taa != null)
           {
             var his = m_HistoryTexture.GetValue( taa ) as RenderTexture;
             if (his)
             {
               need_postDraw = true;
               m_historyTexture = his;
               m_HistoryTexture.SetValue( taa, null );
             }
           }
         }
       }
     }
    
     void ISupportedPostComponent.CameraPostDrawAction()
     {
       if (need_postDraw)
       {
         var m_Taa = typeof( PostProcessingBehaviour ).GetField( "m_Taa", (BindingFlags)int.MaxValue );
         var  m_ResetHistory = typeof( TaaComponent ).GetField( "m_ResetHistory", (BindingFlags)int.MaxValue );
         var  m_HistoryTexture = typeof( TaaComponent ).GetField( "m_HistoryTexture", (BindingFlags)int.MaxValue );
         var taa = m_Taa.GetValue( currentComponent );
         m_ResetHistory.SetValue( taa, false );
         m_HistoryTexture.SetValue( taa, m_historyTexture );
       }
     }//! PRESETS GRID GUI*/
    
    
    
    
    
    
    
    // ////////////////////
    //! FieldsHelper *** //
    
    
    
    //! FieldsHelper *** //
    // ////////////////////
}
}
#endif
