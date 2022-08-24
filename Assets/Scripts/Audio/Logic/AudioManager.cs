
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsList;
    public SceneSoundList_SO sceneSoundList;

    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;
    public float MusicStartSecond => UnityEngine.Random.Range(5f,25f);
    [Header("AudioMixter")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot ambientOnlySnapshot;
    public AudioMixerSnapshot muteSnapshot;

    private float MusicTransitionSecond = 8f;

    private void OnEnable()
    {
        //Debug.Log("OnEnable================>");
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent += OnPlayerSoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.PlaySoundEvent -= OnPlayerSoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }

    private void OnEndGameEvent()
    {
        muteSnapshot.TransitionTo(1f);
        StopCoroutine(soundRoutine);
    }

    private void OnAfterSceneLoadEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        SceneSoundItem sceneSound = sceneSoundList.GetSceneSoundItem(currentScene);

        if (sceneSound == null)
        {
            return;
        }
        
        SoundDetails ambient = soundDetailsList.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsList.GetSoundDetails(sceneSound.music);

        //Debug.Log("OnAfterSceneLoadEvent================>"+ (soundRoutine != null));
        //切换场景音乐转换有问题
        if (soundRoutine !=null)
        {
            StopCoroutine(soundRoutine);
            //muteSnapshot.TransitionTo(2.0f);
        }
        else
        {
            soundRoutine = StartCoroutine(PlaySoundRoutine(ambient, music));
        }
  
    }

    private void OnPlayerSoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailsList.GetSoundDetails(soundName);
        if (soundDetails !=null)
        {
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }

    private IEnumerator PlaySoundRoutine(SoundDetails ambient , SoundDetails music)
    {
        //Debug.Log("PlaySoundRoutine================>ambient");
        if (ambient !=null)
        {
            PlayAmbientClip(ambient,1f);
        }

        yield return new WaitForSeconds(MusicStartSecond);
        //Debug.Log("PlaySoundRoutine================>music");
        if (music != null)
        {
            PlayMusicClip(music, MusicTransitionSecond);
        }


    }

    private void PlayMusicClip(SoundDetails sound ,float transitionTime)
    {
        //Debug.Log("PlayMusicClip================>"+ sound.soundName);
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(sound.soundVolume));
        gameSource.clip = sound.soundClip;
        if (gameSource.isActiveAndEnabled) 
        {
            gameSource.Play();
        }

        normalSnapshot.TransitionTo(transitionTime);
    }

    private void PlayAmbientClip(SoundDetails sound, float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(sound.soundVolume));
        ambientSource.clip = sound.soundClip;
        if (ambientSource.isActiveAndEnabled)
        {
            ambientSource.Play();
        }
        ambientOnlySnapshot.TransitionTo(transitionTime);
    }

    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value * 100 - 80);
    }
}
