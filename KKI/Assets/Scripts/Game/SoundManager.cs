using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<SFXClipsTypes,List<AudioClip>> _SFXClips;
    [SerializeField] private AudioSource _source;
    // Start is called before the first frame update
    public void Init()
    {
        EventBus.Instance.SFXPlay.AddListener(PlaySFX);
    }

    private void PlaySFX(SFXClipsTypes SFXType)
    {
        if (_source == null) return;
        List<AudioClip> defaultClipList = new();
        List<AudioClip> clipList = _SFXClips.GetValueOrDefault(SFXType, defaultClipList);
        if (clipList.Count == 0) return;
        int rnd = Random.Range(0, clipList.Count);
        AudioClip clip = clipList[rnd];
        _source.PlayOneShot(clip);
    }
}

public enum SFXClipsTypes{
    Slash,
    Impact,
    Death,
    Cast,
    Heal,
    ArrowShoot,
    Bless,
    Activation
}