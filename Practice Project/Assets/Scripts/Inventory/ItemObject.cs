using PrototypeProject;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    #region References

    public InventoryItemData referenceItem;

    #endregion

    #region Functions

    public void OnHandlePickupItem()
    {
        // Shout into the void: "Someone store me!"
        InteractionEvents.RequestInventoryStore(this);
    }

    #endregion
}