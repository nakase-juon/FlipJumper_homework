using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ============= Audio ================
    // Audio Source component attached on the character
    public AudioSource audioSource;

    // Audio Clip for Start
    public AudioClip startAudio;
    // Audio Clip for Add Force
    public AudioClip addForceAudio;
    // start jump audio
    public AudioClip jumpAudio;
    // Audio Clip for Successful Jump
    public AudioClip successAudio;
    // Audio Clip for failure
    public AudioClip fallAudio;
    // Audio Clip for Explosion
    public AudioClip explosion;
    // Audio Clip for GetCoin
    public AudioClip getCoin;

    // Start is called before the first frame update
    void Start()
    {
        // Play the audio clip for start the game
        PlayAudio(enumAudioClip.Start);
    }

    /// <summary>
    /// play audio clip
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlayAudio(enumAudioClip enumValue)
    {
        if (enumValue == enumAudioClip.Start)
        {
            audioSource.clip = startAudio;
        }
        if (enumValue == enumAudioClip.AddForce)
        {
            audioSource.clip = addForceAudio;
        }
        if (enumValue == enumAudioClip.Jump)
        {
            audioSource.clip = jumpAudio;
        }
        if (enumValue == enumAudioClip.Success)
        {
            audioSource.clip = successAudio;
        }
        if (enumValue == enumAudioClip.Fall)
        {
            audioSource.clip = fallAudio;
        }

        if (enumValue == enumAudioClip.Explosion)
        {
            audioSource.clip = explosion;
        }

        if (enumValue == enumAudioClip.GetCoin)
        {
            audioSource.clip = getCoin;
        }

        audioSource.Play();
    }
}
