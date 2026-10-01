using UnityEngine;

namespace PrototypeProject
{
    public class PickAndDrop : MonoBehaviour
    {
        #region References

        [SerializeField]private FPPTOTPP _cameraChange;
        private StarterAssetsInputs _inputs;

        #endregion

        #region Variables

        

        #endregion

        #region Functions
        
        

        #endregion
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _cameraChange = FindFirstObjectByType<FPPTOTPP>();
            _inputs = GetComponent<StarterAssetsInputs>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
