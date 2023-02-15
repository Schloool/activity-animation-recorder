using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Component responsible for handling the choice of the animation played by the active model.
/// </summary>
public class AnimationChoice : MonoBehaviour
{
    public event Action<AnimationItem> OnChangeItem;
    
    [field:SerializeField]
    public AnimationList AnimationList { get; set; }
    
    [SerializeField] [Tooltip("The parent transform of the active model.")]
    private Transform characterParent;
    
    /// <summary>
    /// Selects an animation from all animations based on a given index.
    /// The animation will be started automatically after being selected.
    /// </summary>
    /// <param name="animationIndex">The animation index that will be played.</param>
    public void ChooseAnimation(int animationIndex)
    {
        var animationItem = AnimationList.items[animationIndex];
        var character = characterParent.GetChild(0);
        var animator = character.GetComponent<Animator>();
        var animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = animatorOverrideController.animationClips
            .Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, animationItem.clip))
            .ToList();

        animatorOverrideController.ApplyOverrides(anims);
        animator.runtimeAnimatorController = animatorOverrideController;
        
        OnChangeItem?.Invoke(animationItem);
    }

    [Serializable]
    public class AnimationItem : ChoiceObject
    {
        public AnimationClip clip;
        public bool movable;
    }
}