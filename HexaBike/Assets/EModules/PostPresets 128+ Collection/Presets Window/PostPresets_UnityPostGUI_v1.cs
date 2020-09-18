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
class PostPresets_UnityPostGUI_v1 : ISupportedPostComponent {
    public const string TITLE = "Unity PostProcessing v1.0 [FREE]";
    public const string DOWNLOAD_MESSAGE = "[FREE] Download Unity 'PostProcessing Stack v1.0'";
    public const string DOWNLOAD_LINK = "https://www.assetstore.unity3d.com/#!/content/83912";
    
    string ISupportedPostComponent.GET_TITLE { get { return TITLE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_MESSAGE { get { return DOWNLOAD_MESSAGE; } }
    string ISupportedPostComponent.GET_DOWNLOAD_LINK { get { return DOWNLOAD_LINK; } }
    
    public static Type PostProcessingBehaviourType = null;
    public static Type PostProcessingProfileType = null;
    void ISupportedPostComponent.InitializeTypes()
    {   /* foreach (var assembly in Params.AssemblyList) {
           if (PostProcessingBehaviourType == null) PostProcessingBehaviourType = assembly.GetTypes().FirstOrDefault(t=>t.Name == "PostProcessingBehaviour" );
           if (PostProcessingProfileType == null) PostProcessingProfileType = assembly.GetType( "PostProcessingProfile", false, true);
        
         }
         Debug.Log( PostProcessingBehaviourType + " " + Params.AssemblyList.Length ); */
        
        PostProcessingBehaviourType = Params.GetTypeFromStringName( "PostProcessingBehaviour" );
        PostProcessingProfileType = Params.GetTypeFromStringName( "PostProcessingProfile" );
        
        if ( PostProcessingBehaviourType != null && PostProcessingProfileType == null )
        {   Debug.LogWarning( "Missing PostProcessingProfileType" );
            PostProcessingBehaviourType = null;
        }
    }
    
    MonoBehaviour ISupportedPostComponent.MonoComponent { get; set; } //keep this line
    MonoBehaviour ISupportedPostComponent.SecondMonoComponent { get; set; } //keep this line
    
    string ISupportedPostComponent.GetHashString() { return EditorJsonUtility.ToJson( profile ); }
    void ISupportedPostComponent.CREATE_UNDO( string undoName ) { Undo.RecordObject( profile, undoName ); }
    void ISupportedPostComponent.SET_DIRTY() { EditorUtility.SetDirty( profile ); }
    bool ISupportedPostComponent.IsAllowToDraw { get { return profile; } }
    Type ISupportedPostComponent.MonoComponentType { get { return PostProcessingBehaviourType; } }
    Type ISupportedPostComponent.SecondMonoComponentType { get { return null; } }
    public void SetMonoComponentDefaultParametrs() { }
    public void SetSecondMonoComponentDefaultParametrs() { }
    
    
    
    public bool AntiAliasEnable
    {   get { return GetPostprocessingModel( "antialiasing", profile ).enabled; }
        set { GetPostprocessingModel( "antialiasing", profile ).enabled = value; }
    }
    
    bool ISupportedPostComponent.LutEffectExist { get { return true; } set { } }
    
    int ISupportedPostComponent.LutEnable
    {   get { return GetPostprocessingModel( "userLut", profile ).enabled ? 1 : 0; }
        set { GetPostprocessingModel( "userLut", profile ).enabled = (value & 1) != 0; }
    }
    
    Texture2D ISupportedPostComponent.LutTexture
    {   get { return GetFieldValue( "lut", GetPostprocessingModel( "userLut", profile ).settings ) as Texture2D; }
        set
        {   var s =  GetPostprocessingModel( "userLut", profile ).settings;
            SetFieldValue( "lut", s, value );
            GetPostprocessingModel( "userLut", profile ).settings = s;
        }
    }
    
    float ISupportedPostComponent.LutAmount
    {   get { return (float)GetFieldValue( "contribution", GetPostprocessingModel( "userLut", profile ).settings ); }
        set
        {   var s =  GetPostprocessingModel( "userLut", profile ).settings;
            SetFieldValue( "contribution", s, value );
            GetPostprocessingModel( "userLut", profile ).settings = s;
        }
    }
    
    static ScriptableObject lastProfile;
    ScriptableObject m_profile;
    ScriptableObject profile
    {   get
        {   if ( m_profile == null )
            {   var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
                lastProfile = m_profile = GetFieldValue( "profile", currentComponent ) as ScriptableObject;
            }
            return m_profile;
        }
    }
    Dictionary<ScriptableObject, Editor> p_to_e = new Dictionary<ScriptableObject, Editor>();
    static RenderTexture m_historyTexture;
    
    const int LAST_COUNT = 5;
    static Type m_ModelType;
    static Type ModelType()
    {   if ( m_ModelType == null ) m_ModelType = GetField( "fog", lastProfile ).FieldType.BaseType;
        return m_ModelType;
    }
    
    
    ////////////////////////////
    //! POSTPROCESSING COMPONENT GUI *** //
    
    bool ISupportedPostComponent.LeftSideGUI( EditorWindow window, float width )
    {
    
    
        var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
        //profile = currentComponent.profile;
        //   profile = GetFieldValue( "profile", currentComponent ) as ScriptableObject;
        
        var changedProfile = EditorGUILayout.ObjectField( profile, PostProcessingProfileType, false ) as ScriptableObject;
        if ( changedProfile != profile )
        {   Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
            //profile = SetFieldValue( "profile", currentComponent, changedProfile ) as ScriptableObject;
            SetLast( changedProfile );
            m_profile = changedProfile;
            EditorUtility.SetDirty( currentComponent );
            EditorUtility.SetDirty( Params.camera.gameObject );
        }
        
        //         GUILayout.BeginHorizontal( GUILayout.Width( width ) );
        //         for (int i = 0 ; i < LastList.Count ; i++)
        //         {   if (!LastList[i]) continue;
        //             var al = Params.Button.alignment;
        //             Params.Button.alignment = TextAnchor.MiddleLeft;
        //             var result = GUILayout.Button( Params.CONTENT(LastList[i].name, "Set " + LastList[i].name), Params.Button, GUILayout.Width( width / LastList.Count ), GUILayout.Height( 14 ) );
        //             Params.Button.alignment = al;
        //             if (result)
        //             {   Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
        //                 m_profile = SetFieldValue( "profile", currentComponent, LastList[i] ) as ScriptableObject;
        //                 EditorUtility.SetDirty( currentComponent );
        //                 EditorUtility.SetDirty( Params.camera.gameObject );
        //             }
        //             EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );
        //
        //         }
        //         GUILayout.EndHorizontal();
        
        //GUILayout.Space( 10 );
        GUILayout.BeginHorizontal( GUILayout.Width( width ) );
        if ( GUILayout.Button( Params.CONTENT( "Default Profile", "Set Default Profile" ) ) )
        {   var defaultProfile =  AssetDatabase.LoadAssetAtPath( Params.EditorResourcesPath + "/CameraProfile Unity PostProcessing 1.0.asset", PostProcessingProfileType ) as ScriptableObject;
            if ( !defaultProfile ) defaultProfile = CreateProfile( Params.EditorResourcesPath + "/CameraProfile Unity PostProcessing 1.0.asset" );
            Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
            m_profile = SetFieldValue( "profile", currentComponent, defaultProfile ) as ScriptableObject;
            SetLast( defaultProfile );
            EditorUtility.SetDirty( currentComponent );
            EditorUtility.SetDirty( Params.camera.gameObject );
        }
        GUI.enabled = profile;
        if ( GUILayout.Button( Params.CONTENT( "New Copy", "Create and set a copy of current profile" ) ) )
        {   var json = EditorJsonUtility.ToJson(profile);
            var newProfile = CreateProfile(AssetDatabase.GenerateUniqueAssetPath( "Assets/NewCameraProfile.asset" ) );
            EditorJsonUtility.FromJsonOverwrite( json, newProfile );
            EditorUtility.SetDirty( newProfile );
            Undo.RecordObject( currentComponent, "Change PostProcessing Profile" );
            m_profile = SetFieldValue( "profile", currentComponent, newProfile ) as ScriptableObject;
            SetLast( newProfile );
            EditorUtility.SetDirty( currentComponent );
            EditorUtility.SetDirty( Params.camera.gameObject );
        }
        GUI.enabled = true;
        
        
        GUILayout.EndHorizontal();
        
        if ( !profile )
        {   return false;
        }
        
        //ModelType = GetField( "fog", profile ).FieldType.BaseType;
        
        if ( !p_to_e.ContainsKey( profile ) )
            p_to_e.Add( profile, Editor.CreateEditor( profile ) );
        var e = p_to_e[profile];
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
        e.OnInspectorGUI();
        GUILayout.EndScrollView();
        
        return true;
    } //! POSTPROCESSING COMPONENT GUI
    
    
    
    
    
    
    ////////////////////////////
    //! TOP FAST BUTTIONS *** //
    void ISupportedPostComponent.TopFastButtonsGUI( EditorWindow window, Rect rect )
    {   rect.width /= 7;
        rect.height = 40;
        DrawPostProcessingModelButton( "antialiasing", GetPostprocessingModel( "antialiasing", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "ambient\nOcclusion", GetPostprocessingModel( "ambientOcclusion", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "bloom", GetPostprocessingModel( "bloom", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "screenSpace\nReflection", GetPostprocessingModel( "screenSpaceReflection", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "depthOfField", GetPostprocessingModel( "depthOfField", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "fog", GetPostprocessingModel( "fog", profile ), window, rect );
        rect.x += rect.width;
        DrawPostProcessingModelButton( "color\nGrading", GetPostprocessingModel( "colorGrading", profile ), window, rect );
        
        GUILayout.Space( 20 );
    }
    
    ScriptableObject CreateProfile( string path )
    {   var profile = ScriptableObject.CreateInstance(PostProcessingProfileType);
        profile.name = Path.GetFileName( path );
        GetPostprocessingModel( "fog", profile ).enabled = true;
        AssetDatabase.CreateAsset( profile, path );
        return profile;
    }
    
    
    void DrawPostProcessingModelButton( string name, PostProcessingModelFake model, EditorWindow window, Rect rect )
    {   if ( model.enabled ) EditorGUI.DrawRect( rect, new Color( 0.33f, 0.6f, 0.8f, 0.4f ) );
        EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
        if ( Params.TransparentButton( rect, Params.CONTENT( name, "enable/disable " + name.Replace( '\n', ' ' ) ) ) )
        {   Undo.RecordObject( profile, "enable/disable " + name );
            model.enabled = !model.enabled;
            EditorUtility.SetDirty( profile );
            Params.RepaintImages();
        }
    } //! TOP FAST BUTTIONS
    
    
    
    
    
    class PostProcessingModelFake {
        public PostProcessingModelFake( object target )
        {   this.target = target;
        }
        object target;
        PropertyInfo _mEnabledProp = null;
        PropertyInfo enabledProp { get { return _mEnabledProp ?? (_mEnabledProp = ModelType().GetProperty( "enabled", (BindingFlags)int.MaxValue )); } }
        
        public bool enabled
        {   get { return (bool)enabledProp.GetValue( target, null ); }
            set { enabledProp.SetValue( target, value, null ); }
        }
        public object settings
        {   get { return GetFieldValue( "m_Settings", target ); }
            set { SetFieldValue( "m_Settings", target, value ); }
        }
        
    }
    
    Dictionary<ScriptableObject, Dictionary<string, PostProcessingModelFake>> _mPostModelsFields = new Dictionary<ScriptableObject, Dictionary<string, PostProcessingModelFake>>();
    PostProcessingModelFake GetPostprocessingModel( string name, ScriptableObject profile )
    {   if ( !_mPostModelsFields.ContainsKey( profile ) ) _mPostModelsFields.Add( profile, new Dictionary<string, PostProcessingModelFake>() );
        if ( !_mPostModelsFields[profile].ContainsKey( name ) )
        {   var target = PostProcessingProfileType.GetField( name, (BindingFlags)int.MaxValue ).GetValue( profile );
            _mPostModelsFields[profile].Add( name, new PostProcessingModelFake( target ) );
        }
        return _mPostModelsFields[profile][name];
    }
    
    static Dictionary<Type, Dictionary<string, FieldInfo>> _mGetFieldValue = new Dictionary<Type, Dictionary<string, FieldInfo>>();
    static FieldInfo GetField( string name, object o )
    {   var type = o.GetType();
        if ( !_mGetFieldValue.ContainsKey( type ) ) _mGetFieldValue.Add( type, new Dictionary<string, FieldInfo>() );
        if ( !_mGetFieldValue[type].ContainsKey( name ) )
            _mGetFieldValue[type].Add( name, type.GetField( name, (BindingFlags)int.MaxValue ) );
        return _mGetFieldValue[type][name];
    }
    static object GetFieldValue( string name, object o )
    {   return GetField( name, o ).GetValue( o );
    }
    static object SetFieldValue( string name, object o, object value )
    {   GetField( name, o ).SetValue( o, value );
        return value;
    }
    
    
    
    /////////////////
    //! LAST PROFILES *** //
    
    List<ScriptableObject> _mLastList;
    List<ScriptableObject> LastList
    {   get
        {   if ( _mLastList == null )
            {   _mLastList = new List<ScriptableObject>();
                for ( int i = 0 ; i < LAST_COUNT ; i++ )
                {   var guid = EditorPrefs.GetString( "EModules/" + Params.TITLE + "/LastProfiles" + i, "" );
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
                EditorPrefs.SetString( "EModules/" + Params.TITLE + "/LastProfiles" + i, guid );
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
    
    
    
    
    
    
    
    ////////////////////////////
    //! PRESETS GRID GUI *** //
    
    bool need_postDraw ;
    void ISupportedPostComponent.CameraPredrawAction()
    {   need_postDraw = false;
    
        if ( GetPostprocessingModel( "antialiasing", profile ).enabled )
        {   var method = GetFieldValue( "method", GetPostprocessingModel( "antialiasing", profile ).settings );
        
            if ( method.ToString() == "Taa" )
            {   var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
            
                var fieldTaa =  GetField( "m_Taa", currentComponent );
                if ( fieldTaa != null && GetFieldValue( "m_Taa", currentComponent ) != null )
                {   var taa = GetFieldValue("m_Taa",  currentComponent );
                
                    var fieldm_ResetHistory =  GetField( "m_ResetHistory", taa );
                    var fieldm_HistoryTexture =  GetField( "m_HistoryTexture", taa );
                    
                    
                    if ( fieldm_ResetHistory != null && fieldm_HistoryTexture != null )
                    {   var  m_HistoryTexture = GetFieldValue( "m_HistoryTexture", taa )as RenderTexture;
                    
                        if ( m_HistoryTexture )
                        {   need_postDraw = true;
                            m_historyTexture = m_HistoryTexture;
                            SetFieldValue( "m_HistoryTexture", taa, null );
                        }
                    }
                }
            }
        }
    }
    
    void ISupportedPostComponent.CameraPostDrawAction()
    {   if ( need_postDraw )
        {   var currentComponent = (MonoBehaviour)((ISupportedPostComponent)this).MonoComponent;
            var taa = GetFieldValue("m_Taa",  currentComponent );
            SetFieldValue( "m_ResetHistory", taa, false );
            SetFieldValue( "m_HistoryTexture", taa, m_historyTexture );
        }
    }//! PRESETS GRID GUI
    
    
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
