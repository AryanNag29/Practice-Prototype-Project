using UnityEngine;
using UnityEditor;
using System.IO;

namespace PrototypeProject
{
//Unity GUI programming is pretty sick i will learn it 
    public class GrabbablePropSetupWindow : EditorWindow
    {
        // --- Identity ---
        private string propDisplayName = "";
        private string propId = "";
        private Sprite propIcon;

        // --- Inputs ---
        private GameObject sourceModel;
        private Material outlinerMaterial;

        // --- Checklist Toggles ---
        private bool setTag = true;
        private bool addRigidbody = true;
        private bool addColliders = true;
        private bool addObjectGrabbable = true;
        private bool addItemObject = true;
        private bool applyOutlinerMaterial = true;
        private bool createPrefab = true;
        private bool createScriptableObject = true;

        // --- Paths & Names ---
        private const string TARGET_TAG = "GrabbableObject";
        private const string PREFAB_SAVE_FOLDER = "Assets/Prefabs";
        private const string DATA_SAVE_FOLDER = "Assets/Scripts/Inventory/Inventory Scriptable Object";

        [MenuItem("Tools/Grabbable And Inventory Object Setup")]
        public static void ShowWindow()
        {
            GrabbablePropSetupWindow window =
                GetWindow<GrabbablePropSetupWindow>("Grabbable And Inventory Object Setup ");
            window.minSize = new Vector2(320, 560);
        }

        private void OnEnable()
        {
            // Auto-locate OutlinerMat in the project if empty
            if (outlinerMaterial == null)
            {
                string[] guids = AssetDatabase.FindAssets("OutlinerMat t:Material");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    outlinerMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(6);
            GUILayout.Label("Auto Prop Setup Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            // 1. Identity
            GUILayout.Label("Item Identity", EditorStyles.boldLabel);
            propDisplayName = EditorGUILayout.TextField("Display Name", propDisplayName);
            propId = EditorGUILayout.TextField("Item ID", propId);
            propIcon = (Sprite)EditorGUILayout.ObjectField("Item Icon", propIcon, typeof(Sprite), false);

            EditorGUILayout.Space(8);

            // 2. Sources
            GUILayout.Label("Sources", EditorStyles.boldLabel);
            sourceModel =
                (GameObject)EditorGUILayout.ObjectField("3D Model / Mesh", sourceModel, typeof(GameObject), false);
            outlinerMaterial =
                (Material)EditorGUILayout.ObjectField("Outliner Material", outlinerMaterial, typeof(Material), false);

            EditorGUILayout.Space(8);

            // 3. Setup Steps
            GUILayout.Label("Setup Steps", EditorStyles.boldLabel);
            setTag = EditorGUILayout.Toggle("Tag as 'GrabbableObject'", setTag);
            addRigidbody = EditorGUILayout.Toggle("Add Rigidbody", addRigidbody);
            addColliders = EditorGUILayout.Toggle("Add Collider (if missing)", addColliders);
            addObjectGrabbable = EditorGUILayout.Toggle("Add ObjectGrabbable Script", addObjectGrabbable);
            addItemObject = EditorGUILayout.Toggle("Add ItemObject Script", addItemObject);
            applyOutlinerMaterial = EditorGUILayout.Toggle("Set Element 1 = OutlinerMat", applyOutlinerMaterial);

            EditorGUILayout.Space(4);
            createPrefab = EditorGUILayout.Toggle("Save to Assets/Prefabs", createPrefab);
            createScriptableObject = EditorGUILayout.Toggle("Save to Inventory SO Folder", createScriptableObject);

            EditorGUILayout.Space(14);

            GUIStyle miniWarn = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
            GUILayout.Label("Changes will create assets on disk and configure the scene object.", miniWarn);

            EditorGUILayout.Space(4);

            GUI.backgroundColor = new Color(0.25f, 0.75f, 0.35f);
            if (GUILayout.Button("Apply Setup", GUILayout.Height(36)))
            {
                if (sourceModel == null)
                {
                    EditorUtility.DisplayDialog("Missing Model", "Please assign a 3D Model / Mesh first.", "OK");
                    return;
                }

                ExecuteSetup();
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(4);
            GUILayout.Label("Auto Prop Setup v1.2", EditorStyles.centeredGreyMiniLabel);
        }

        private void ExecuteSetup()
        {
            EnsureFolder(PREFAB_SAVE_FOLDER);
            EnsureFolder(DATA_SAVE_FOLDER);

            // 1. Instantiate the model into the scene
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourceModel);
            if (instance == null) instance = Instantiate(sourceModel);
            instance.name = string.IsNullOrEmpty(propDisplayName) ? sourceModel.name : propDisplayName;

            // 2. Tag as GrabbableObject
            if (setTag)
            {
                instance.tag = TARGET_TAG;
            }

            // 3. Add Rigidbody
            if (addRigidbody && !instance.TryGetComponent<Rigidbody>(out _))
            {
                instance.AddComponent<Rigidbody>();
            }

            // Add BoxCollider if no collider exists in hierarchy
            if (addColliders && instance.GetComponentInChildren<Collider>() == null)
            {
                instance.AddComponent<BoxCollider>();
            }

            // 4. Add ObjectGrabbable script (Direct type reference from Assets/Scripts/PickAndDrop/)
            if (addObjectGrabbable && !instance.TryGetComponent<ObjectGrabbable>(out _))
            {
                instance.AddComponent<ObjectGrabbable>();
            }

            // 5. Add ItemObject script (Direct type reference from Assets/Scripts/Inventory/)
            ItemObject itemComp = null;
            if (addItemObject)
            {
                if (!instance.TryGetComponent(out itemComp))
                {
                    itemComp = instance.AddComponent<ItemObject>();
                }
            }

            // 6. Set Materials (Element 0 = Base Material, Element 1 = OutlinerMat)[cite: 4]
            if (applyOutlinerMaterial && outlinerMaterial != null)
            {
                MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mr in renderers)
                {
                    Material baseMat = (mr.sharedMaterials.Length > 0 && mr.sharedMaterials[0] != null)
                        ? mr.sharedMaterials[0]
                        : null;

                    mr.sharedMaterials = new Material[] { baseMat, outlinerMaterial }; //[cite: 4]
                    EditorUtility.SetDirty(mr);
                }
            }

            // 7. Save as Prefab asset into Assets/Prefabs
            GameObject savedPrefab = null;
            if (createPrefab)
            {
                string prefabPath =
                    AssetDatabase.GenerateUniqueAssetPath($"{PREFAB_SAVE_FOLDER}/{instance.name}.prefab");
                savedPrefab =
                    PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.UserAction);
            }

            // 8. Create and save InventoryItemData ScriptableObject
            if (createScriptableObject)
            {
                InventoryItemData newItemData = ScriptableObject.CreateInstance<InventoryItemData>();
                newItemData.id = propId;
                newItemData.displayName = propDisplayName;
                newItemData.icon = propIcon;
                newItemData.prefab = savedPrefab != null ? savedPrefab : instance;

                string dataPath = AssetDatabase.GenerateUniqueAssetPath($"{DATA_SAVE_FOLDER}/{instance.name}_SO.asset");
                AssetDatabase.CreateAsset(newItemData, dataPath);

                // 9. Assign ScriptableObject reference to ItemObject's "Reference Item" field
                if (itemComp != null)
                {
                    SerializedObject serializedItemComp = new SerializedObject(itemComp);
                    SerializedProperty refItemProp = serializedItemComp.FindProperty("referenceItem")
                                                     ?? serializedItemComp.FindProperty("ReferenceItem")
                                                     ?? serializedItemComp.FindProperty("reference_Item");

                    if (refItemProp != null)
                    {
                        refItemProp.objectReferenceValue = newItemData;
                        serializedItemComp.ApplyModifiedProperties();
                    }

                    PrefabUtility.RecordPrefabInstancePropertyModifications(itemComp);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeGameObject = instance;
            EditorUtility.DisplayDialog("Complete",
                $"Successfully configured '{instance.name}'!\n\nPrefab saved to:\n{PREFAB_SAVE_FOLDER}\n\nSO saved to:\n{DATA_SAVE_FOLDER}",
                "OK");
        }

        private void EnsureFolder(string relativePath)
        {
            if (!Directory.Exists(relativePath))
            {
                Directory.CreateDirectory(relativePath);
            }
        }
    }
}