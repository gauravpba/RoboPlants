using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioTrigger : MonoBehaviour
{
    #region Variables

    public AudioMixer playerAudioMixer;
    public string mixerChannel;

    bool inZone;
    float currentVal = 0f;
    float maxVal = 0f;
    float minVal = -40f;

    #endregion

    #region Unity Methods

    private void Update()
    {
        currentVal = Mathf.Lerp(currentVal, (inZone ? maxVal : minVal), 0.4f * Time.deltaTime);
        playerAudioMixer.SetFloat(mixerChannel, currentVal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(playerAudioMixer == null)
            {
                playerAudioMixer = FindObjectOfType<AudioMixer>();
            }

            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerAudioMixer == null)
            {
                playerAudioMixer = FindObjectOfType<AudioMixer>();
            }

            inZone = false;
        }
    }

    #endregion

    #region Custom Methods



    #endregion
}
