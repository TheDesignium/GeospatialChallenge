using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class occlusionControls : MonoBehaviour
{

    public AROcclusionManager occlusionManager;
	public bool camOcclusion;
	
	public bool humanOcclusion;
	
    void Start()
    {
        
    }


    void Update()
    {
        if(Input.GetKeyUp(KeyCode.O))
		{
			toggleOcclusion();
		}
#if !UNITY_EDITOR
	  if(Input.touchCount == 1)
      {
          if (Input.GetTouch(0).phase == TouchPhase.Began)
          {
            Touch touch = Input.GetTouch(0);
			var x = (touch.position.x / Screen.width);
			var y = (touch.position.y / Screen.height);
			
			if(x < 0.1f && y > 0.85f)
			{
				toggleOcclusion();
			}
          }
      }
#endif
#if UNITY_EDITOR
      if(Input.GetMouseButtonDown(0))
      {
			var x = (Input.mousePosition .x / Screen.width);
			var y = (Input.mousePosition .y / Screen.height);
			
			if(x < 0.1f && y > 0.85f)
			{
				toggleOcclusion();
			}
      }
#endif
    }
	
	public void toggleOcclusion()
	{
		if(camOcclusion == true)
		{
			camOcclusion = false;
			occlusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
			if(humanOcclusion == true)
			{
				occlusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Best;
				occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Best;
			}
			occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferHumanOcclusion;
		}
		else if(camOcclusion == false)
		{
			camOcclusion = true;
			if(humanOcclusion == true)
			{
				occlusionManager.requestedHumanDepthMode = HumanSegmentationDepthMode.Disabled;
				occlusionManager.requestedHumanStencilMode = HumanSegmentationStencilMode.Disabled;
			}
			occlusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Best;
			occlusionManager.requestedOcclusionPreferenceMode = OcclusionPreferenceMode.PreferEnvironmentOcclusion;
		}
	}
	
}
