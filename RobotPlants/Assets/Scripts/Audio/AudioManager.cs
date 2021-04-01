using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Variables

    [Header("AudioSource Refrences")]
    AudioSource bassAudioSource;
    AudioSource leadAudioSource;
    AudioSource percFXAudioSource;
    AudioSource stringsAudioSource;
    AudioSource synthAudioSource;
    AudioSource sfxAudioSource;

    [Header("Object/Component References and Prefabs")]
    [HideInInspector] public PlayerBody playerBody; //Reference to the playerBody <--- Set by the playerBody in instantiation

    #endregion

    #region Unity Methods

    private void Awake()
    {
        //Get AudioSource refs
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].gameObject.name.Equals("BassChannel")) bassAudioSource = audioSources[i];
            else if (audioSources[i].gameObject.name.Equals("LeadChannel")) leadAudioSource = audioSources[i];
            else if (audioSources[i].gameObject.name.Equals("PercFXChannel")) percFXAudioSource = audioSources[i];
            else if (audioSources[i].gameObject.name.Equals("StringsChannel")) stringsAudioSource = audioSources[i];
            else if (audioSources[i].gameObject.name.Equals("SynthChannel")) synthAudioSource = audioSources[i];
            else if (audioSources[i].gameObject.name.Equals("SFXChannel")) sfxAudioSource = audioSources[i];
        }
    }

    #endregion

    #region Custom Methods

    public void PlaySFX(AudioClip sfxClip, bool randomPitchShift = false, float pitchShiftSpread = 0.2f)
    {
        //Plays a passed in sfx clip
        if (randomPitchShift) sfxAudioSource.pitch = Random.Range(1f - pitchShiftSpread, 1f + pitchShiftSpread);
        else sfxAudioSource.pitch = 1f;

        sfxAudioSource.PlayOneShot(sfxClip);
    }

    #endregion
}
