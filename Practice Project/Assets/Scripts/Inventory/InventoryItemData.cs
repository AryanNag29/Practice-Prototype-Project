using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    #region Variable

    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;

    #endregion
}
