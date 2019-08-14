using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _source;

    private Dictionary<string, AudioClip> pool_dict = new Dictionary<string, AudioClip>();

    private WaitForEndOfFrame _wait;

    public bool sound_off = true;

    public void PlaySound(string soundName)
    {
        if (!sound_off)
        {
            return;
        }

        if (!pool_dict.ContainsKey(soundName))
        {
            pool_dict.Add(soundName, Resources.Load<AudioClip>("Sound/" + soundName));
        }

        StartCoroutine(DoPlaySound(pool_dict[soundName]));
    }

    int sound_count = 0;

    public IEnumerator DoPlaySound(AudioClip clip)
    {
        if (sound_count > 0) yield break;
        sound_count++;
        clip.LoadAudioData();
        while (clip.loadState == AudioDataLoadState.Loading || clip.loadState == AudioDataLoadState.Unloaded)
        {
            yield return _wait;
        }

        sound_count--;
        if (clip.loadState == AudioDataLoadState.Loaded)
        {
            StartCoroutine(PlayOneShot(clip));
        }
    }

    private void Start()
    {
        //PlayBackGround("MusicBackground");
        //if (DataMangaer.GetLocalDataInt(LocalKey.KEY_BACKGROUND_MUSIC_STATUS) == 0)
        //{
        //    StartBackGround();
        //}
    }


    public void PlayBackGround(string backgroundName)
    {
        if (_source == null)
        {
            var go = new GameObject("bg");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            _source = go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        _source.clip = Resources.Load<AudioClip>("BGM/" + backgroundName);
        _source.loop = true;
    }

    public void StopBackGround()
    {
        _source.Stop();
    }

    public void StartBackGround()
    {
        //if (DataMangaer.GetLocalDataInt(LocalKey.KEY_BACKGROUND_MUSIC_STATUS) == 0)
        //{
        //    if (!_source.isPlaying)
        //    {
        //        _source.Play();
        //    }
        //}
    }

    public IEnumerator PlayOneShot(AudioClip clip)
    {
        var go = new GameObject("one_shot");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        var s = go.AddComponent<AudioSource>();
        s.loop = false;
        s.clip = clip;
        s.Play();
        yield return new WaitForSeconds(clip.length + .1f);
        Destroy(go);
    }

    private void Awake()
    {
        _wait = new WaitForEndOfFrame();
        DontDestroyOnLoad(gameObject);
        if (_source == null)
        {
            var go = new GameObject("bg");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            _source = go.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }
    }

}