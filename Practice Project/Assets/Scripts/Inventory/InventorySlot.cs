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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
