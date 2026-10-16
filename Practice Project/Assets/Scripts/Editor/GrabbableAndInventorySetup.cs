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
            GUILayout.Label("Auto Grabbable Object Setup Tool", EditorStyles.boldLabel);
            //space
            EditorGUILayout.Space(4);

            //1. Identity
            //Label for identity
            GUILayout.Label("Item Identity", EditorStyles.boldLabel);
            //whatever display name will be it will store in propDisplayName
            propDisplayName = EditorGUILayout.TextField("Display Name", propDisplayName);
            //id store in propid
            propId = EditorGUILayout.TextField("Item ID", propId);
            //propicon of gameobject type will change to sprite and then whatever icon will be put it will store in gameobject
            propIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", propIcon, typeof(Sprite), false);

            EditorGUILayout.Space(8);

            //2. Source
            //taking sources and putting it into required variable
            GUILayout.Label("Sources", EditorStyles.boldLabel);
            //Putting 3d model of the gameobject in sourceModel
            sourceModel =
                (GameObject)EditorGUILayout.ObjectField("3D Model/ Mesh", sourceModel, typeof(GameObject), false);
            outlinerMaterial =
                (Material)EditorGUILayout.ObjectField("Outliner Material", outlinerMaterial, typeof(Material), false);

            EditorGUILayout.Space(8);

            //3. Setup Steps
            //Setting Toggle for the given booleans 
            GUILayout.Label("Setup Steps", EditorStyles.boldLabel);
            setTag = EditorGUILayout.Toggle("Tag as 'Grabbable Object'", setTag);
            addRigidBody = EditorGUILayout.Toggle("Add RigidBody", addRigidBody);
            addCollider = EditorGUILayout.Toggle("Add Collider(if missing)", addCollider);
            addObjectGrabbable = EditorGUILayout.Toggle("Add ObjectGrabbable Script", addObjectGrabbable);
            addItemObject = EditorGUILayout.Toggle("Add ItemObject Script(Inventory)", addItemObject);
            applyOutlinerMaterial = EditorGUILayout.Toggle("Add Outliner On Grabbable Object", applyOutlinerMaterial);

            EditorGUILayout.Space(4);

            createPrefab = EditorGUILayout.Toggle("Save to Asset/Prefab", createPrefab);
            createScriptableObject = EditorGUILayout.Toggle("Save to Inventory SO Folder", createScriptableObject);

            EditorGUILayout.Space(14);

            //GuiStyle
            GUIStyle miniWarn = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
            GUILayout.Label("Change will create assets on disk and configue the scene object.", miniWarn);

            EditorGUILayout.Space(4);

            GUI.backgroundColor = new Color(0.25f, 0.75f, 0.35f);
            //GUI apply button
            if (GUILayout.Button("Apply Setup", GUILayout.Height(36)))
            {
                if (sourceModel == null)
                {
                    //If source model is empty it will show this warning
                    EditorUtility.DisplayDialog("Missing Model", "Please assign a 3D Model / Mesh First.", "OK");
                    return;
                }

                ExecuteSetup();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space(4);
            GUILayout.Label("Auto Prop Setup", EditorStyles.centeredGreyMiniLabel);
        }

        /// <summary>
        /// This will execute the setup for prop
        /// </summary>
        private void ExecuteSetup()
        {
            EnsureFolder(Prefab_Save_Folder);
            EnsureFolder(Data_Save_Folder);

            //1. Instantiate the model into the scene
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourceModel);
            // if there instance is null then it will instance the source model in the scene
            if (instance == null) instance = Instantiate(sourceModel);
            //set the name of the instance to the name of prop display name if prop Display name is empty or null
            instance.name = string.IsNullOrEmpty(propDisplayName) ? sourceModel.name : propDisplayName;

            //2. Tag as GrabbableObject
            if (setTag)
            {
                //Adding the tag of the instance 
                instance.tag = TargetTag;
            }

            //3. add Rigidbody
            if (addRigidBody && !instance.TryGetComponent<Rigidbody>(out _))
            {
                //Adding rigidbody component to instance
                instance.AddComponent<Rigidbody>();
            }

            //4. Add BoxCollider if no collider exist in hierarchy
            if (addCollider && instance.GetComponentInChildren<Collider>() == null)
            {
                //adding the box collider to the gameobject if it does not have any collider
                instance.AddComponent<BoxCollider>();
            }

            //5. Add ObjectGrabbable script to the instance
            if (addObjectGrabbable && !instance.TryGetComponent<ObjectGrabbable>(out _))
            {
                //adding grabbable object script to the instance
                instance.AddComponent<ObjectGrabbable>();
            }

            //6. Set material (Element 0 = base material and element 1 = outliner material)
            if (applyOutlinerMaterial && outlinerMaterial != null)
            {
                //getting the component of the instance's meshrender as a child
                MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();
                //for eacch mr in the renderers it will run
                foreach (MeshRenderer mr in renderers)
                {
                    //if there is base material then it will be null in the new prefab
                    Material baseMat = (mr.sharedMaterials.Length > 0 && mr.sharedMaterials[0] != null)
                        ? mr.sharedMaterials[0]
                        : null;
                    //setting the new materials as material[0] = baseMat and material[1] as outliner material
                    mr.sharedMaterials = new Material[] { baseMat, outlinerMaterial };
                    //tells Unity that a target object has changed so the editor knows to save it to disk.
                    EditorUtility.SetDirty(mr);
                }
            }

            //7. Save as Prefab asset into Asset/Prefabs
            //setting the prefab as null first
            GameObject savedPrefab = null;
            if (createPrefab)
            {
                //save prefab to the prefab folder
                string prefabPath =
                    AssetDatabase.GenerateUniqueAssetPath($"{Prefab_Save_Folder}/{instance.name}.prefab");

                //Instance of the prefab in the scene
                savedPrefab =
                    PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.UserAction);
            }

            //8. Create and save inventory ItemData ScriptableObject
            if (createScriptableObject)
            {
                //Storing the data into the scriptable object 
                InventoryItemData newItemData = ScriptableObject.CreateInstance<InventoryItemData>();
                newItemData.id = propId;
                newItemData.displayName = propDisplayName;
                newItemData.icon = propIcon;
                newItemData.prefab = savedPrefab != null ? savedPrefab : instance;

                //save name of the scriptable object
                string dataPath = AssetDatabase.GenerateUniqueAssetPath($"{Data_Save_Folder}/{instance.name}_SO.asset");
                //Create the scriptable object with this function first param (Gameobject, srting name)
                AssetDatabase.CreateAsset(newItemData, dataPath);

                //9. Add ItemObject Script (Direct type reference from asset/script/inventory/)
                ItemObject itemComp = null;

                if (addItemObject)
                {
                    if (!instance.TryGetComponent(out itemComp))
                    {
                        itemComp = instance.AddComponent<ItemObject>();
                    }
                }

                //10. Assign ScriptableObject reference to itemObject's "Reference Item" field
                if (itemComp != null)
                {
                    SerializedObject serializedItemComp = new SerializedObject(itemComp);
                    //This is really cool way to check multiple variable or addresses if you are not confirmed the name
                    SerializedProperty refItemProp = serializedItemComp.FindProperty("referenceItem") ??
                                                     serializedItemComp.FindProperty("ReferenceItem") ??
                                                     serializedItemComp.FindProperty("reference_Item");

                    if (refItemProp != null)
                    {
                        refItemProp.objectReferenceValue = newItemData;
                        //Apply new change in the properties
                        serializedItemComp.ApplyModifiedProperties();
                    }

                    //to save the new change in the prefab
                    PrefabUtility.RecordPrefabInstancePropertyModifications(itemComp);
                }
            }
            //save asset
            AssetDatabase.SaveAssets();
            //refresh assets
            AssetDatabase.Refresh();

            Selection.activeGameObject = instance;
            //end dialog
            EditorUtility.DisplayDialog("Complete",
                $"Successfully configured '{instance.name}'!\n\nPrefab saved to:\n{Prefab_Save_Folder}\n\nSO saved to:\n{Data_Save_Folder}",
                "OK");
        }

        private void EnsureFolder(string relativePath)
        {
        }

        #endregion
    }
}