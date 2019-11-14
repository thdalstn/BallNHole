using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerObj : MonoBehaviour
{
    [SerializeField] AudioClip[] _BGMClips;
    [SerializeField] AudioClip[] _ESClips;
    List<AudioSource> _ltESPlayer; // lt = list
    AudioSource _BGMPlayer;
    static SoundManagerObj _uniqueInstance;

    public static SoundManagerObj _instance
    {
        get
        {
            return _uniqueInstance;
        }
    }

    public enum eBGMTYPE
    {
        MENU,
        STAGE1
    }

    public enum eESTYPE
    {
        BTNCLICK,
        BNB, // = BALL AND BALL
        BNW,
        GOAL,
        NOGOAL
    }

    void Awake()
    {
        _uniqueInstance = this;
        _BGMPlayer = GetComponent<AudioSource>();
        _ltESPlayer = new List<AudioSource>();
    }

    void LateUpdate()
    {
        for (int n = 0; n < _ltESPlayer.Count; n++)
        {
            if (!_ltESPlayer[n].isPlaying)
            {
                AudioSource AS = _ltESPlayer[n];

                _ltESPlayer.Remove(AS);
                Destroy(AS.gameObject);
            }
        }
    }

    public void PlayBGM (eBGMTYPE type, float vol = 0.3f, bool isLoop = true)
    {
        _BGMPlayer.clip = _BGMClips[(int)type];
        _BGMPlayer.volume = vol;
        _BGMPlayer.loop = isLoop;
        _BGMPlayer.Play();
    }

    public void PlayES (eESTYPE type, float vol = 1.0f, bool isLoop = false)
    {
        GameObject go = new GameObject("EffSounds");
        AudioSource AS = go.AddComponent<AudioSource>();

        go.transform.SetParent(transform);
        AS.clip = _ESClips[(int)type];
        AS.volume = vol;
        AS.loop = isLoop;
        AS.Play();
        _ltESPlayer.Add(AS);
    }
}