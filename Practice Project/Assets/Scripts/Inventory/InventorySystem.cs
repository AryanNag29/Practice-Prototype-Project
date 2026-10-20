using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    #region Storage

    //Dictionary use to grab item more easily from reference data
    private Dictionary<InventoryItemData, InventoryItem> m_itemDirectory;

    public List<InventoryItem> inventory { get; private set; }

    // this is to call this class as singleton class during runtime
    public static InventorySystem current { get; private set; }
    //event
    public event Action OnInventoryChangedEvent;
    
    #endregion


    #region Functions
    
    /// <summary>
    /// we are trying to add an item on our inventory which is present in stack already.
    /// if it doesn't we will create new instance of that item and add it to our inventory
    /// </summary>
    /// <param name="referenceData"></param>
    public void Add(InventoryItemData referenceData)
    {
        if (m_itemDirectory.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDirectory.Add(referenceData,newItem);
        }
        OnInventoryChangedEvent?.Invoke();
    }
    
    /// <summary>
    /// Removing is just the inverse of the add.
    /// and when our stack reach 0 we will remove the item from the inventory
    /// </summary>
    /// <param name="referenceData"></param>
    public void Remove(InventoryItemData referenceData)
    {
        if (m_itemDirectory.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDirectory.Remove(referenceData);
            }
            OnInventoryChangedEvent?.Invoke();
        }
    }
    

    #endregion
    
    
    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        inventory = new List<InventoryItem>();
        m_itemDirectory = new Dictionary<InventoryItemData, InventoryItem>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
