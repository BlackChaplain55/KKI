using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это музыкальный проигрыватель, играет плейлисты по кругу

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _ambientSource;
    [SerializeField] private List<AudioClip> _musicCombat = new();
    [SerializeField] private List<AudioClip> _musicMenu = new();
    [SerializeField] private List<AudioClip> _musicDeckBuild = new();
    [SerializeField] private List<AudioClip> _ambient = new();
    [SerializeField] private Game _game;
    private bool _soundEnabled = true;
    private Coroutine _musicRoutine;
    private Coroutine _ambientRoutine;

    private void OnValidate()
    {
        if(!_game) _game = FindObjectOfType<Game>();
    }

    public void SetMenuPlaylist()
    {
        Play(_musicMenu);
    }

    public void SetCombatPlaylist()
    {
        Play(_musicCombat, _ambient);
    }

    public void SetDeckbuildPlaylist()
    {
        Play(_musicDeckBuild);
    }

    private void Play(List<AudioClip> playlist, List<AudioClip> ambient=null)
    {
        if (_musicRoutine!=null) StopCoroutine(_musicRoutine);
        if (playlist.Count>0)
        {
            _musicRoutine = StartCoroutine(PlayMusic(playlist));
        };
        if (_ambientRoutine != null) StopCoroutine(_ambientRoutine);
        if (ambient!=null)
        {
            if (ambient.Count > 0)
            {
                _ambientRoutine = StartCoroutine(PlayAmbient(playlist));
            }
        }
    }

    private IEnumerator PlayMusic(List<AudioClip> playlist)
    {
        int index = -1;
        while (_soundEnabled)
        {
            if (!_musicSource.isPlaying)
            {
                index++;
                if (index == playlist.Count) index = 0;
                _musicSource.clip = playlist[index];
                _musicSource.Play();
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    private IEnumerator PlayAmbient(List<AudioClip> playlist)
    {
        int index = -1;
        while (_soundEnabled)
        {
            if (!_ambientSource.isPlaying)
            {
                index++;
                if (index == playlist.Count) index = 0;
                _ambientSource.clip = playlist[index];
                _ambientSource.Play();
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
