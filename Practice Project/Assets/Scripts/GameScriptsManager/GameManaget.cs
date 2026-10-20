using UnityEngine;

public class GameManaget : MonoBehaviour
{
    #region Reference
    
    [SerializeField]private GameObject itemslot;
    [SerializeField] private Transform inventoryPanel; 
    
    #endregion
    
    #region Regions
    

    #endregion

    #region Variables

    

    #endregion

    #region Functions

    public void OnUpdateInventory()
    {
        foreach (Transform t in inventoryPanel)
        {
          Destroy(t.gameObject);   
        }

        DrawInventory();
    }

    public void DrawInventory()
    {
        foreach (InventoryItem item in InventorySystem.current.inventory)
        {
            AddInventorySlot(item);
        }
    }

    public void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(itemslot);
        obj.transform.SetParent(inventoryPanel, false);

        InventorySlot slot = obj.GetComponent<InventorySlot>();
        // 3. Set the item data
        if (slot != null)
        {
            slot.Set(item);
        }
        else
        {
            Debug.LogError("The itemslot prefab is missing the InventorySlot script!");
        }
    }

    #endregion
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        //it's a event not a function
        InventorySystem.current.OnInventoryChangedEvent += OnUpdateInventory;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
