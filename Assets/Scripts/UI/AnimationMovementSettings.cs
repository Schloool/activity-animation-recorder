using UnityEngine;

public class AnimationMovementSettings : MonoBehaviour
{
    [SerializeField] private float minSpeedFactor = 0.5f;
    [SerializeField] private float maxSpeedFactor = 2f;
    [SerializeField] private float maxDistance = 5f;
    
    [SerializeField] [Tooltip("The parent for all UI-elements related to movement settings")]
    private GameObject movementAdjustmentParent;
    
    private CharacterChoice _characterChoice;
    private AnimationChoice _animationChoice;
    private Animator _currentAnimator;
    private bool _inPlace;
    private float _movementSpeed;

    private void Start()
    {
        _characterChoice = FindObjectOfType<CharacterChoice>();
        _characterChoice.OnChangeCharacter += HandleCharacterChange;

        _animationChoice = FindObjectOfType<AnimationChoice>();
        _animationChoice.OnChangeItem += HandleAnimationChange;
        
        _inPlace = true;
        _movementSpeed = 1f;
    }

    private void OnDestroy()
    {
        _characterChoice.OnChangeCharacter -= HandleCharacterChange;
        _animationChoice.OnChangeItem -= HandleAnimationChange;
    }

    private void Update()
    {
        if (_inPlace) return;
        
        var characterTransform = _currentAnimator.transform;
        characterTransform.Translate(Vector3.forward * _movementSpeed * Time.deltaTime);
        
        var distanceFromSpawn = Vector3.Magnitude(characterTransform.position);
        if (distanceFromSpawn > maxDistance) characterTransform.position = Vector3.zero;
    }

    private void HandleCharacterChange(GameObject newCharacter)
    {
        _currentAnimator = newCharacter.GetComponent<Animator>();
    }
    
    private void HandleAnimationChange(AnimationChoice.AnimationItem animation)
    {
        movementAdjustmentParent.SetActive(animation.movable);
    }

    public void HandleSpeedChange(float value)
    {
       _movementSpeed = Mathf.Lerp(minSpeedFactor, maxSpeedFactor, value);
       _currentAnimator.speed = _movementSpeed;
    }

    public void HandleInPlace(bool inPlace)
    {
        _currentAnimator.transform.position = Vector3.zero;
        _inPlace = inPlace;
    }

    public void HandleRotation(float value)
    {
        var characterTransform = _currentAnimator.transform;
        characterTransform.position = Vector3.zero;
        
        var rotation = Mathf.Lerp(0f, 360f, value);
        characterTransform.parent.rotation = Quaternion.Euler(Vector3.up * rotation);
    }
}