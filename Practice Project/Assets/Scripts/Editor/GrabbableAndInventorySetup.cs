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
        private bool setTag = true;
        private bool addRigidBody = true;
        private bool addCollider = true;
        private bool addObjectGrabbable = true;
        private bool addItemObject = true;
        private bool applyOutlinerMaterial = true;
        private bool createPrefab = true;
        private bool createScriptableObject = true;

        #endregion
    }
}