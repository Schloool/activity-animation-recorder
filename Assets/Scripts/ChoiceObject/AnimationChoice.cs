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
    public event Action<AnimationClipItem> OnChangeItem;
    
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
        ChooseAnimationByItem(animationItem.clips[0]);
    }

    public void ChooseAnimationByItem(AnimationClipItem item)
    {
        var character = characterParent.GetChild(0);
        SetAnimationClip(character.gameObject, item.clip);
        
        if (_interactionObject != null) Destroy(_interactionObject);
        _boundObjects.ForEach(Destroy);
        _boundObjects.Clear();
        item.bindActions.ForEach((a) =>
        {
            ApplyBindAction(a, item.clip.length);
        });

        if (_interactionCoroutine != null)
        {
            StopCoroutine(_interactionCoroutine);
            _interactionCoroutine = null;
        }

        if (item.interactAction.interactablePrefab != null)
        {
            _interactionCoroutine = StartCoroutine(InteractionRoutine(item, character.gameObject));
        }
        
        OnChangeItem?.Invoke(item);
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

    private IEnumerator InteractionRoutine(AnimationClipItem item, GameObject character)
    {
        var animator = character.GetComponent<Animator>();
        var action = item.interactAction;
        
        while (true)
        {
            _interactionObject = Instantiate(action.interactablePrefab, action.spawnPosition, 
                action.interactablePrefab.transform.rotation);
            if (item.interactAction.moveIntoObject)
            {
                LeanTween.move(character, _interactionObject.transform.position + Vector3.down * 1.3f, 
                    item.clip.length);
            }
            
            if (item.interactAction.turnToObject)
            {
                character.transform.LookAt(_interactionObject.transform);
            }
            
            yield return new WaitForSeconds(item.clip.length);
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

    private void ApplyBindAction(AnimationClipItem.AnimationBindAction action, float clipLength)
    {
        var character = characterParent.GetChild(0);
        var objectParent = RecursiveFindChild(character, action.boneName);
        var bindItem = Instantiate(action.prefab);

        StartCoroutine(BindRoutine(action, bindItem, objectParent, clipLength * action.timePercentage));
    }

    private IEnumerator BindRoutine(AnimationClipItem.AnimationBindAction action, GameObject bindItem, Transform objectParent, float time)
    {
        yield return new WaitForSeconds(time);
        bindItem.transform.parent = objectParent;
        bindItem.transform.localPosition = action.offset;
        bindItem.transform.localScale = action.prefab.transform.localScale;
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
        public List<AnimationClipItem> clips;
    }

    [Serializable]
    public struct AnimationClipItem
    {
        public AnimationClip clip;
        public bool loopable;
        public bool movable;
        public List<AnimationBindAction> bindActions;
        public AnimationInteractAction interactAction;
        

        [Serializable]
        public class AnimationBindAction
        {
            public GameObject prefab;
            [Range(0f, 1f)] public float timePercentage;
            public string boneName;
            public Vector3 offset;
        }
        
        [Serializable]
        public class AnimationInteractAction
        {
            public GameObject interactablePrefab;
            public Vector3 spawnPosition;
            public bool moveIntoObject;
            public bool turnToObject;
            public AnimationClip afterMainAnimationClip;
        }
    }
}