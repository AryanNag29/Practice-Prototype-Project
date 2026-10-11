using UnityEngine;

public class ItemObject : MonoBehaviour
{
    #region References

    public InventoryItemData referenceItem;
    public InventorySystem _inventorySystem;

    #endregion

    #region Functions

    public void OnHandlePickupItem()
    {
        _inventorySystem.Add(referenceItem);
        Destroy(gameObject);
    }

    #endregion

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inventorySystem = GetComponent<InventorySystem>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}