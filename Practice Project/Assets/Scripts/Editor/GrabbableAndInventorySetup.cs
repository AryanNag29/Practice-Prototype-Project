using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace PrototypeProject
{
    public class GrabbableAndInventorySetup : EditorWindow
    {
        #region Variables

        //These will be store in object inventory scriptable object 
        //Identity
        private string propDisplayName = "";
        private string propId = "";
        private Sprite propIcon;

        //This will be model and it's outliner shader material for that object
        //Inputs
        private GameObject sourceModel;
        private Material outlinerMaterial;

        //These are the checklist that you can add in your grabbable object prefab setup
        //Checklist Toggles

        //set object tag to "GrabbableObject"
        private bool setTag = true;

        //Add rigid body into it
        private bool addRigidBody = true;

        //add colliders in that object;
        private bool addCollider = true;

        //add object grabbable script
        private bool addObjectGrabbable = true;

        //add the itemobject script
        private bool addItemObject = true;

        //apply the outliner material by putting it into mesh renderer
        private bool applyOutlinerMaterial = true;

        //create prefab and saving it into prefab folder
        private bool createPrefab = true;

        //create scriptable object of that object and saving it into SO folder
        private bool createScriptableObject = true;

        //Paths for those folder and names for tag
        //Paths And Names
        private const string TargetTag = "GrabbableObject";
        private const string Prefab_Save_Folder = "Assets/Prefabs";
        private const string Data_Save_Folder = "Assets/Scripts/Inventory/Inventory Scriptable Object";

        #endregion

        #region Functions

        //This function is to show the tool window in the scene under tools
        [MenuItem("Tools/Grabbable & Inventory Object Setup")]
        public static void ShowWindow()
        {
            GrabbablePropSetupWindow window =
                GetWindow<GrabbablePropSetupWindow>("Grabbable & Inventory Object Setup");
            //This is window size for that tool window
            window.minSize = new Vector2(320, 560);
        }

        private void OnEnable()
        {
            //Auto-locate outlinerMat in the project if empty
            if (outlinerMaterial == null)
            {
                //This is to find the asset outliner of type material
                string[] guide = AssetDatabase.FindAssets("OutlinerMat, t:Material");
                //if it find more then one material of same type it will select the first one
                if (guide.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guide[0]);
                    outlinerMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
        }

        private void OnGUI()
        {
            //Title
            //layout space between gui
            EditorGUILayout.Space(6);
            //Lebel for the tool
            GUILayout.Label("Auto Grabbable Object Setup Tool",EditorStyles.boldLabel);
            //space
            EditorGUILayout.Space(4);
            
            //1. Identity
            //Label for identity
            GUILayout.Label("Item Identity",EditorStyles.boldLabel);
            //whatever display name will be it will store in propDisplayName
            propDisplayName = EditorGUILayout.TextField("Display Name", propDisplayName);
            //id store in propid
            propId = EditorGUILayout.TextField("Item ID", propId);
            //propicon of gameobject type will change to sprite and then whatever icon will be put it will store in gameobject
            propIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", propIcon, typeof(Sprite), false);
        }

        #endregion
    }
}