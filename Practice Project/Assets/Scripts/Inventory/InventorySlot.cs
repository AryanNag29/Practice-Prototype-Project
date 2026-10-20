using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
//make sure to have it in assembly file
using TMPro;
using Image = UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour
{
    #region Reference

    [SerializeField] private TextMeshProUGUI m_label;
    [SerializeField] private Image m_Icon;
    [SerializeField] private GameObject m_StackObj;
    [SerializeField] private TextMeshProUGUI m_StackLabel;

    #endregion

    #region Functions

    public void Set(InventoryItem item)
    {
        //setting the inventory item icon
        m_Icon.sprite = item.data.icon;
        m_label.text = item.data.displayName;
        if (item.stackSize <= 1)
        {
            m_StackObj.SetActive(false);
            return;
        }

        m_StackLabel.text = item.stackSize.ToString();
        
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
