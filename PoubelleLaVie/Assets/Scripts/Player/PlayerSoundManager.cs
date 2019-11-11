using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    public AudioClip trashSound;
    public AudioClip bottleSound;
    public AudioClip pukeSound;
    public AudioClip npcTakeSound;
    public AudioClip npcDropSound;
    public AudioClip pukeStepSound;

    private AudioSource _audioSrc;
    private PlayerComponent _playerComponent;
    private float _stepTimer;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSrc = GetComponent<AudioSource>();
        _playerComponent = GetComponent<PlayerComponent>();

        _playerComponent.OnTrashTrown += TrashThrownAction;
        _playerComponent.OnObjectTaken += ObjectTakenAction;
        _playerComponent.OnObjectDropped += ObjectDroppedAction;
    }

    private void ObjectDroppedAction(GameObject obj, PlayerComponent playerComponent, IUsable usable)
    {
        string typeName = usable.GetType().Name;
        AudioClip clip = null;

        if (typeName == nameof(NPCBehaviour))
        {
            clip = npcDropSound;
        }

        if (clip == null)
            return;

        _audioSrc.clip = clip;
        _audioSrc.Play();
    }

    private void ObjectTakenAction(GameObject obj, PlayerComponent playerComponent, IUsable usable)
    {
        string typeName = usable.GetType().Name;
        AudioClip clip = null;

        if (typeName == nameof(BottleComponent))
        {
            clip = bottleSound;
        }
        else if (typeName == nameof(PukeComponent))
        {
            clip = pukeSound;
        }
        else if (typeName == nameof(NPCBehaviour))
        {
            clip = npcTakeSound;
        }

        if (clip == null)
            return;

        _audioSrc.clip = clip;
        _audioSrc.Play();
    }

    private void TrashThrownAction(GameObject obj, PlayerComponent playerComponent, int garbageCount)
    {
        _audioSrc.clip = trashSound;
        _audioSrc.Play();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_stepTimer <= 0.5f)
            _stepTimer += Time.deltaTime;

        if(_playerComponent.Data.MoveState == PlayerMoveState.Run &&
            _stepTimer >= 0.5f &&
            _playerComponent.SpeedModObj is PukeComponent)
        {
            _stepTimer = 0.0f;
            _audioSrc.clip = pukeStepSound;
            _audioSrc.Play();
        }
    }
}
