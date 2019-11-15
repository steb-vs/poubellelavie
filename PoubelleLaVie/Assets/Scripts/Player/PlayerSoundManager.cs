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
    private PlayerDataComponent _data;
    private PlayerComponent _player;
    private float _stepTimer;

    // Start is called before the first frame update
    private void Start()
    {
        _audioSrc = GetComponent<AudioSource>();
        _player = GetComponent<PlayerComponent>();
        _data = GetComponent<PlayerDataComponent>();

        _player.OnTrashThrown += TrashThrownAction;
        _player.OnObjectTaken += ObjectTakenAction;
        _player.OnObjectDropped += ObjectDroppedAction;
    }

    private void ObjectDroppedAction(GameObject obj, PlayerComponent playerComponent, IUsable usable)
    {
        string typeName = usable.GetType().Name;
        AudioClip clip = null;

        if (typeName == nameof(NPCComponent))
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
        else if (typeName == nameof(NPCComponent))
        {
            clip = npcTakeSound;
        }

        if (clip == null)
            return;

        _audioSrc.clip = clip;
        _audioSrc.Play();
    }

    private void TrashThrownAction(GameObject obj, PlayerComponent playerComponent, int trashCount)
    {
        _audioSrc.clip = trashSound;
        _audioSrc.Play();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_stepTimer <= 0.5f)
            _stepTimer += Time.deltaTime;

        if(_data.moveState == PlayerMoveState.Walk &&
            _stepTimer >= 0.5f &&
            _data.speedModifierObject is PukeComponent)
        {
            _stepTimer = 0.0f;
            _audioSrc.clip = pukeStepSound;
            _audioSrc.Play();
        }
    }
}
