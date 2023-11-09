using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chattingBot : MonoBehaviour
{
  public List<Sprite> spritesList; // List of sprites to cycle through
  public Image imageObject; // Image object to set the sprites to
  public float delayTime = 1f; // Time to wait before changing to the next sprite

  private int currentIndex = 0; // Current index in the spritesList

  void Start()
  {
      // Set the initial sprite
      imageObject.sprite = spritesList[currentIndex];

      // Start the coroutine to cycle through the sprites
      StartCoroutine(CycleSprites());
  }

  IEnumerator CycleSprites()
  {
      while (true)
      {
          // Wait for delayTime before changing to the next sprite
          yield return new WaitForSeconds(delayTime);

          // Change to the next sprite
          int randomIndex = Random.Range(0, spritesList.Count);
          imageObject.sprite = spritesList[randomIndex];
      }
  }
}
