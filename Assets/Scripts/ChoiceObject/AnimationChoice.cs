using System;
using System.Collections;
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
    
    private List<GameObject> _boundObjects;
    private GameObject _interactionObject;
    private Coroutine _interactionCoroutine;

    private void Start()
    {
        _boundObjects = new List<GameObject>();
        ChooseAnimation(0);
    }

    /// <summary>
    /// Selects an animation from all animations based on a given index.
    /// The animation will be started automatically after being selected.
    /// </summary>
    /// <param name="animationIndex">The animation index that will be played.</param>
    public void ChooseAnimation(int animationIndex)
    {
        var animationItem = AnimationList.items[animationIndex];
        ChooseAnimationByItem(animationItem, 0);
    }

    public void ChooseAnimationByItem(AnimationItem animationItem, int clipIndex)
    {
        var character = characterParent.GetChild(0);
        SetAnimationClip(character.gameObject, animationItem.clips[clipIndex]);
        
        if (_interactionObject != null) Destroy(_interactionObject);
        _boundObjects.ForEach(Destroy);
        _boundObjects.Clear();
        animationItem.bindActions.ForEach(ApplyBindAction);

        if (_interactionCoroutine != null)
        {
            StopCoroutine(_interactionCoroutine);
            _interactionCoroutine = null;
        }

        if (animationItem.interactAction.interactablePrefab != null)
        {
            _interactionCoroutine = StartCoroutine(InteractionRoutine(animationItem, character.gameObject, clipIndex));
        }
        
        OnChangeItem?.Invoke(animationItem);
    }

    private void SetAnimationClip(GameObject target, AnimationClip clip)
    {
        var animator = target.GetComponent<Animator>();
        var animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = animatorOverrideController.animationClips
            .Select(a => new KeyValuePair<AnimationClip, AnimationClip>(a, clip))
            .ToList();

        animatorOverrideController.ApplyOverrides(anims);
        animator.runtimeAnimatorController = animatorOverrideController;
    }

    private IEnumerator InteractionRoutine(AnimationItem item, GameObject character, int clipIndex)
    {
        var animator = character.GetComponent<Animator>();
        
        while (true)
        {
            _interactionObject = Instantiate(item.interactAction.interactablePrefab);
            if (item.interactAction.moveIntoObject)
            {
                LeanTween.move(character, _interactionObject.transform.position + Vector3.down * 1.3f, 
                    item.clips[clipIndex].length);
            }
            
            yield return new WaitForSeconds(item.clips[clipIndex].length);
            animator.speed = 0f;
    
            if (item.interactAction.afterMainAnimationClip != null)
            {
                SetAnimationClip(_interactionObject, item.interactAction.afterMainAnimationClip);
                yield return new WaitForSeconds(item.interactAction.afterMainAnimationClip.length);
            }
    
            Destroy(_interactionObject);
            
            animator.speed = 1f;
            character.transform.position = Vector3.zero;
            character.transform.rotation = Quaternion.identity;
        }
    }

    private void ApplyBindAction(AnimationItem.AnimationBindAction action)
    {
        var character = characterParent.GetChild(0);
        var objectParent = RecursiveFindChild(character, action.boneName);
        _boundObjects.Add(Instantiate(action.prefab, objectParent));
    }

    private static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child;
            
            var found = RecursiveFindChild(child, childName);
            if (found != null) return found;
        }
        return null;
    }

    [Serializable]
    public class AnimationItem : ChoiceObject
    {
        public List<AnimationClip> clips;
        public bool movable;
        public List<AnimationBindAction> bindActions;
        public AnimationInteractAction interactAction;

        [Serializable]
        public class AnimationBindAction
        {
            public GameObject prefab;
            public string boneName;
        }
        
        [Serializable]
        public class AnimationInteractAction
        {
            public GameObject interactablePrefab;
            public bool moveIntoObject;
            [FormerlySerializedAs("afterMainAnimation")] public AnimationClip afterMainAnimationClip;
        }
    }
}