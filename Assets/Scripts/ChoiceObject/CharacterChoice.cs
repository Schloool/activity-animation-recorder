using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Component used to realize the selection of the active character model of the scene.
/// </summary>
public class CharacterChoice : MonoBehaviour
{
    [SerializeField] [Tooltip("All character model prefabs which can be chosen.")]
    public CharacterList characterList;
    
    [SerializeField] [Tooltip("The parent of the character model object.")] 
    private Transform characterParent;

    [SerializeField] [Tooltip("Whether the first character will be instantiated automatically on start or not.")]
    private bool instantiateFirstCharacterOnStart = true;
    
    [SerializeField] private RuntimeAnimatorController animatorController;

    [SerializeField] private float scaleCoefficient =  0.15f;

    /// <summary>
    /// Event called when a new character has been selected.
    /// </summary>
    public event Action<GameObject> OnChangeCharacter;
    
    private void Start()
    {
        if (instantiateFirstCharacterOnStart) ChooseCharacter(0);
    }

    /// <summary>
    /// Selects a new character model from all character model prefabs based on a given index.
    /// </summary>
    /// <param name="characterIndex">The index used to identify the new character model prefab in the list.</param>
    public void ChooseCharacter(int characterIndex)
    {
        if (characterParent.childCount > 0) Destroy(characterParent.GetChild(0).gameObject);
        var newCharacter = Instantiate(characterList.items[characterIndex].prefab, characterParent);
        
        var characterAnimator = newCharacter.GetComponent<Animator>();
        if (characterAnimator.runtimeAnimatorController == null)
        {
            characterAnimator.runtimeAnimatorController = animatorController;
        }

        newCharacter.transform.localScale = Vector3.one * scaleCoefficient;
        
        OnChangeCharacter?.Invoke(newCharacter);
    }

    [Serializable]
    public class CharacterItem : ChoiceObject
    {
        public GameObject prefab;
    }
}