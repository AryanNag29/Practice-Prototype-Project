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
        

        #endregion
    }
}