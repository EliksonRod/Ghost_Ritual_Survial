using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "PickUps/Gameplay_Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public GameObject prefab;

    public bool hasItem;
    public bool isQuestItem;
}
