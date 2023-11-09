using System;
using System.Collections;
using System.Collections.Generic;

using GoogleTextToSpeech.Scripts.Data;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

//using NinevaStudios.GoogleMaps;
//using NinevaStudios.GoogleMaps.Internal;

namespace GoogleTextToSpeech.Scripts.Example
{
    public class TextToSpeechExample : MonoBehaviour
    {
        [SerializeField] private VoiceScriptableObject voice;
        [SerializeField] private TextToSpeech textToSpeech;
        [SerializeField] private AudioSource audioSource;
        //[SerializeField] private TextMeshProUGUI inputField;

        private Action<AudioClip> _audioClipReceived;
        private Action<BadRequestData> _errorReceived;

        //public GoogleMapsDemo google;
        public Image imageObject;
        //public Image imageObjectSmall;
        //public Image bgImageObject;

        public List<Sprite> spritesList; // List of sprites to cycle through
        public float delayTime = 1f; // Time to wait before changing to the next sprite
        private int currentIndex = 0; // Current index in the spritesList

		    public string debugString;
        //public TMP_Text responseTxt;
        public bool talking;
        public bool muted;
        public bool keepVisible;

        public void PressBtn()
        {
            _errorReceived += ErrorReceived;
            _audioClipReceived += AudioClipReceived;
            //textToSpeech.GetSpeechAudioFromGoogle(inputField.text, voice, _audioClipReceived, _errorReceived);
        }

        private void ErrorReceived(BadRequestData badRequestData)
        {
            Debug.Log($"Error {badRequestData.error.code} : {badRequestData.error.message}");
            talking = false;
        }

        public void muteControl(bool b)
        {
          muted = b;
          if(muted == true && talking == true)
          {
            imageObject.gameObject.SetActive(false);
            audioSource.Stop();
            talking = false;
          }
        }

    		void Update()
    		{

    		}

        public void remoteRequest(string s)
        {
          if(talking == false && muted == false)
          {
            _errorReceived += ErrorReceived;
            _audioClipReceived += AudioClipReceived;
            textToSpeech.GetSpeechAudioFromGoogle(s, voice, _audioClipReceived, _errorReceived);
            talking = true;
          }
        }

        private void AudioClipReceived(AudioClip clip)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
            StartCoroutine(controlUI());
        }

        IEnumerator controlUI()
        {
          talking = true;

          float alph = 0;
          var spin = Color.white;
          var txt =  Color.black;
          
          if(keepVisible == false)
          {
            spin.a = alph;
            txt.a = alph;
            imageObject.color = spin;
            imageObject.gameObject.SetActive(true);

            while(alph < 1)
            {
              alph = spin.a;
              alph += 0.05f;
              spin.a = alph;
              txt.a = alph;
              imageObject.color = spin;
              yield return new WaitForEndOfFrame();
            }
          }

          yield return new WaitForEndOfFrame();

          while (audioSource.isPlaying)
          {
            yield return new WaitForSeconds(delayTime);
            int randomIndex = UnityEngine.Random.Range(0, spritesList.Count);
            imageObject.sprite = spritesList[randomIndex];
          }

          if(keepVisible == false)
          {
            while(alph > 0)
            {
              alph = spin.a;
              alph -= 0.01f;
              spin.a = alph;
              txt.a = alph;
              imageObject.color = spin;
              yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();

            imageObject.gameObject.SetActive(false);
          }

        yield return new WaitForSeconds(1);

          talking = false;
        }
    }
}
