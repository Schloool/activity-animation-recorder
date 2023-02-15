using UnityEngine;

public class AnimationItemDropdown : SimulationItemDropdown<AnimationChoice.AnimationItem>
{
    private CharacterChoice _characterChoice;
    
    protected override void Start()
    {
        base.Start();
        _characterChoice = FindObjectOfType<CharacterChoice>();
        _characterChoice.OnChangeCharacter += HandleCharacterChange;
    }
    
    private void OnDestroy()
    {
        _characterChoice.OnChangeCharacter -= HandleCharacterChange;
    }

    private void HandleCharacterChange(GameObject character)
    {
        dropdown.value = 0;
    }
}