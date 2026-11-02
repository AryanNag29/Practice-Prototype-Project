using UnityEngine;

public class ItemObject : MonoBehaviour
{
    #region References

    public InventoryItemData referenceItem;

    #endregion

    #region Functions

    public void OnHandlePickupItem()
    {
        InventorySystem.current.Add(referenceItem);
        Destroy(gameObject);
    }

    #endregion
    
}