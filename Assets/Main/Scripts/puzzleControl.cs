using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Google.XR.ARCoreExtensions.Samples.Geospatial;
using GoogleTextToSpeech.Scripts.Example;

using GoogleTextToSpeech.Scripts.Example;

using TMPro;

public class puzzleControl : MonoBehaviour
{

    public VisionAPI vision;
    public GeospatialController geo;
    public TextToSpeechExample speech;
    public SquareAPI square;

    public string inputText;
    public string puzzleTarget;
    public List<string> stringList = new List<string>();

    public int puzzlechoice;
    int lastchoice = 9999;

    public GameObject cameraUI;
    public GameObject instructionsUI;
    public GameObject puzzleUI;
    public GameObject successUI;

    //gameobject game instructions
    //next object ui button

    public TMP_Text introText;
    public TMP_Text targetText;
    public TMP_Text rewardText;

    public Image botImage;
    public Image introImage;
    public Image puzzleButtonImage;
    public Image successImage;
    public Image[] camerasImage;

    public Sprite searchSprite;
    public Sprite thinkingSprite;
    public Sprite waitingSprite;
    public Sprite successSprite;

    public AudioSource audio;
    public AudioClip clip;

    bool gameStarted;

    int userScore;

    List<int> pastChoices = new List<int>();

    void Start()
    {
      instructionsUI.SetActive(true);
      cameraUI.SetActive(false);

      string[] stringArray = inputText.Split(',');
      for (int i = 0; i < stringArray.Length; i++)
      {
          string trimmedString = stringArray[i].Trim();
          if (!string.IsNullOrEmpty(trimmedString))
          {
              stringList.Add(trimmedString);
          }
      }
    }

    void Update()
    {
      if(Input.GetKeyUp(KeyCode.Alpha0))
      {
        startGame();
      }
    }

    public void startGame()
    {
      if(gameStarted == false)
      {
        StartCoroutine(fadeOutIntro());
        gameStarted = true;
      }
      StartCoroutine(startPuzzle());
      StartCoroutine(fadeOutPuzzle());
    }

    IEnumerator startPuzzle()
    {
      int puzzlechoice = 0;
      do
      {
          puzzlechoice = UnityEngine.Random.Range(0, stringList.Count());
      } while (pastChoices.Contains(puzzlechoice) || puzzlechoice == lastchoice);

      yield return new WaitForEndOfFrame();
      puzzleTarget = stringList[puzzlechoice];
      lastchoice = puzzlechoice;
      pastChoices.Add(puzzlechoice);

      if(pastChoices.Count > 20)
      {
          pastChoices.Clear();
      }

      Debug.Log("Take a photo of: " + puzzleTarget);

      string speechtext = "Ok, lets take a picture of a " + puzzleTarget;

      if(puzzleTarget == "insect")
      {
        speechtext = "Ok, lets take a picture of an " + puzzleTarget;
      }
      if(puzzleTarget == "pants" || puzzleTarget == "shoes")
      {
        speechtext = "Ok, lets take a picture of some " + puzzleTarget;
      }

      speech.remoteRequest(speechtext);

      yield return new WaitForEndOfFrame();

      while(speech.talking == true)
      {
        yield return new WaitForEndOfFrame();
      }

      botImage.sprite = searchSprite;
      //cameraUI.SetActive(true);
      targetText.text = puzzleTarget;
      //targetText.gameObject.SetActive(true);
      StartCoroutine("fadeInCamera");
    }

    public void haveResults(List<string> resultList)
    {
      StartCoroutine(checkResults(resultList));
    }

    IEnumerator checkResults(List<string> resultList)
    {
      bool match = resultList.Exists(item =>
          string.Equals(
              RemovePluralS(item).ToLowerInvariant(),
              RemovePluralS(puzzleTarget).ToLowerInvariant(),
              StringComparison.OrdinalIgnoreCase
          )
      );

      if (match == true)
      {
          Debug.Log("Good, I can see " + puzzleTarget);
          userScore += 1;

/*
          List<string> ll = geo.returnLatLon();
          string eul = ll[2].Replace(",","/");
          eul = eul.Replace("(","");
          eul = eul.Replace(")","");
          string imgname = ll[0] + "-" + ll[1] + "-" + eul + "-" + ll[3];
          Debug.Log(imgname);
          //vision.saveImage(imgname); //0-0-0.00/ 0.00/ 0.00/ 0.00-0
*/

          StartCoroutine("fadeOutCamera");

          int reward = userScore * 100;

          string speechtext = "Wow! what a great picture of a " + puzzleTarget + ". I have added " + reward + " yen to your gift card as a reward!";

          if(puzzleTarget == "insect")
          {
            speechtext = "Wow! what a great picture of an " + puzzleTarget + ". I have added " + reward + " yen to your gift card as a reward!";
          }
          if(puzzleTarget == "pants" || puzzleTarget == "shoes")
          {
            speechtext = "Wow! what a great picture of some " + puzzleTarget + ". I have added " + reward + " yen to your gift card as a reward!";
          }
          speech.remoteRequest(speechtext);
          yield return new WaitForEndOfFrame();
          while(speech.talking == true)
          {
            yield return new WaitForEndOfFrame();
          }

          square.addBonus(reward);

          //do some more celebration, trigger gift card thing
          botImage.sprite = successSprite;
          Sprite sprite = vision.makeSuccessSprite();
          yield return new WaitForEndOfFrame();
          successImage.sprite = sprite;
          rewardText.text = "¥" + reward.ToString() + "+";
          successUI.SetActive(false);
          successUI.SetActive(true);
          //

          yield return new WaitForSeconds(4);

          botImage.sprite = waitingSprite;
          //cameraUI.SetActive(false);
          puzzleUI.SetActive(true);
          puzzleButtonImage.color = Color.white;
          //targetText.gameObject.SetActive(false);
          StartCoroutine("fadeOutCamera");
      }
      else
      {
        foreach (string name in resultList)
        {
            Debug.Log("Object Name: " + name);
        }

        if(resultList.Count > 0)
        {
          Debug.Log("No Match Try Again");

          string speechtext = "Hmmmm, I could see ";
          foreach (string name in resultList)
          {
              if(!speechtext.Contains(name))
              {
                speechtext = speechtext + name + ",";
              }
          }
          speechtext += " but not " + puzzleTarget + ". please try again!";
          speech.remoteRequest(speechtext);
          yield return new WaitForEndOfFrame();
          while(speech.talking == true)
          {
            yield return new WaitForEndOfFrame();
          }
          botImage.sprite = searchSprite;
          //cameraUI.SetActive(true);
          StartCoroutine("fadeInCamera");

        }
        else
        {
          noResults();
        }
      }
      yield return new WaitForEndOfFrame();
    }

    public static string RemovePluralS(string input)
    {
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase))
        {
            return input.Substring(0, input.Length - 1);
        }
        return input;
    }

    public void noResults()
    {
      StartCoroutine(noResultsLoop());
    }

    IEnumerator noResultsLoop()
    {
      Debug.Log("No Result Try Again");
      speech.remoteRequest("I could not see anything, please try again!");
      yield return new WaitForEndOfFrame();
      while(speech.talking == true)
      {
        yield return new WaitForEndOfFrame();
      }
      botImage.sprite = searchSprite;
      //cameraUI.SetActive(true);
      StartCoroutine("fadeInCamera");
    }

    IEnumerator fadeOutIntro()
    {
      var spin = Color.white;
      var coin = introText.color;

      float alph = 1;

      yield return new WaitForEndOfFrame();
      while(alph > 0)
      {
        alph = spin.a;
        alph -= 0.006f;
        spin.a = alph;
        coin.a = alph;
        introImage.color = spin;
        introText.color = coin;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      instructionsUI.SetActive(false);
    }

    IEnumerator fadeOutPuzzle()
    {
      var spin = Color.white;

      float alph = 1;

      yield return new WaitForEndOfFrame();
      while(alph > 0)
      {
        alph = spin.a;
        alph -= 0.006f;
        spin.a = alph;
        puzzleButtonImage.color = spin;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      puzzleUI.SetActive(false);
    }

    IEnumerator fadeInCamera()
    {
      var spin = Color.white;
      float alph = 0;
      spin.a = alph;

      foreach(Image img in camerasImage)
      {
        img.color = spin;
      }
      targetText.color = spin;

      cameraUI.SetActive(true);
      targetText.gameObject.SetActive(true);

      yield return new WaitForEndOfFrame();
      while(alph < 0.8f)
      {
        alph = spin.a;
        alph += 0.006f;
        spin.a = alph;
        foreach(Image img in camerasImage)
        {
          img.color = spin;
        }
        targetText.color = spin;

        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
    }

    IEnumerator fadeOutCamera()
    {
      var spin = Color.white;
      float alph = 1;
      spin.a = alph;

      foreach(Image img in camerasImage)
      {
        img.color = spin;
      }
      targetText.color = spin;

      yield return new WaitForEndOfFrame();
      while(alph > 0)
      {
        alph = spin.a;
        alph -= 0.006f;
        spin.a = alph;
        foreach(Image img in camerasImage)
        {
          img.color = spin;
        }
        targetText.color = spin;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();

      cameraUI.SetActive(false);

    }

    /*camerasImage

   dog, cat, bird, insect ,car, truck, bus, train,
   fruit, vegetable, table, chair, pants, dress,
   shoes, hat, tree, flower, person, bicycle, motorcycle,
   ball, bottle, building,

    */
}
