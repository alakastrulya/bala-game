using UnityEngine;

[CreateAssetMenu(fileName = "Item_Stats", menuName = "Game/Item Stats")]
public class ItemStats : ScriptableObject
{
    public int candyHeal = 1;
    public int hazardDamage = 1;
    public int spikesDamage = 1;
    public int bombDamage = 2;
    public float bombFuse = 2f;
}
