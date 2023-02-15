using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TMP_Dropdown))]
public abstract class SimulationItemDropdown<T> : MonoBehaviour where T : ChoiceObject
{
    [field:SerializeField] 
    public SimulationItemList<T> ItemList { get; set; }
    
    protected TMP_Dropdown dropdown;

    protected virtual void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        
        // generate the dropdown items based on the character model prefab list
        dropdown.options = ItemList.items
            .Select(item => new TMP_Dropdown.OptionData(item.name))
            .ToList();
    }
}