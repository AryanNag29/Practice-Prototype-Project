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

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }
}