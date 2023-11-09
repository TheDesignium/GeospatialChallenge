using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps.Internal
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Object = System.Object;

	class GoogleMapsSceneHelper : MonoBehaviour
	{
		static GoogleMapsSceneHelper _instance;
		static readonly object InitLock = new object();
		readonly object _queueLock = new object();
		readonly List<Action> _queuedActions = new List<Action>();
		readonly List<Action> _executingActions = new List<Action>();

		readonly List<GoogleMapsView> _activeMaps = new List<GoogleMapsView>();

		// Tracking orientation changes
		int _screenWidth;
		int _screenHeight;

		public static GoogleMapsSceneHelper Instance
		{
			get
			{
				if (_instance == null)
				{
					Init();
				}

				return _instance;
			}
		}

		#region pause_resume

		void Start()
		{
			_screenWidth = Screen.width;
			_screenHeight = Screen.height;
		}

		public void Register(GoogleMapsView map)
		{
			if (!_activeMaps.Contains(map))
			{
				_activeMaps.Add(map);
			}
		}

		public void Unregister(GoogleMapsView map)
		{
			if (_activeMaps.Contains(map))
			{
				_activeMaps.Remove(map);
			}
		}

		void OnApplicationPause(bool pauseStatus)
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return; // on iOS we don't need to handle pause/resume
			}

			if (pauseStatus)
			{
				HandleHide();
			}
			else
			{
				HandleShow();
			}
		}

		#endregion

		void HandleShow()
		{
			foreach (var mapView in _activeMaps)
			{
				mapView.GoogleMapViewAJO.Call("onStart");
				mapView.GoogleMapViewAJO.Call("onResume");
			}
		}

		void HandleHide()
		{
			foreach (var mapView in _activeMaps)
			{
				mapView.GoogleMapViewAJO.Call("onPause");
				mapView.GoogleMapViewAJO.Call("onStop");
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		internal static void Init()
		{
			lock (InitLock)
			{
				if (ReferenceEquals(_instance, null))
				{
					var instances = FindObjectsOfType<GoogleMapsSceneHelper>();

					if (instances.Length > 1)
					{
						Debug.LogError(typeof(GoogleMapsSceneHelper) + " Something went really wrong " +
						               " - there should never be more than 1 " + typeof(GoogleMapsSceneHelper) +
						               " Reopening the scene might fix it.");
					}
					else if (instances.Length == 0)
					{
						var singleton = new GameObject {hideFlags = HideFlags.HideAndDontSave};
						_instance = singleton.AddComponent<GoogleMapsSceneHelper>();
						singleton.name = typeof(GoogleMapsSceneHelper).ToString();

						DontDestroyOnLoad(singleton);

						Debug.Log("[Singleton] An _instance of " + typeof(GoogleMapsSceneHelper) +
						          " is needed in the scene, so '" + singleton.name +
						          "' was created with DontDestroyOnLoad.");
					}
					else
					{
						Debug.Log("[Singleton] Using _instance already created: " + _instance.gameObject.name);
					}
				}
			}
		}

		GoogleMapsSceneHelper()
		{
		}

		internal static void Queue(Action action)
		{
			if (action == null)
			{
				Debug.LogWarning("Trying to queue null action");
				return;
			}

			lock (_instance._queueLock)
			{
				_instance._queuedActions.Add(action);
			}
		}

		void Update()
		{
			ListenForOrientationChanges();

			MoveQueuedActionsToExecuting();

			while (_executingActions.Count > 0)
			{
				Action action = _executingActions[0];
				_executingActions.RemoveAt(0);
				action();
			}
		}


		void ListenForOrientationChanges()
		{
			if (_screenWidth != Screen.width || _screenHeight != Screen.height)
			{
				foreach (var mapView in _activeMaps)
				{
					mapView.TriggerOrientationChange();
				}

				_screenWidth = Screen.width;
				_screenHeight = Screen.height;
			}
		}

		void MoveQueuedActionsToExecuting()
		{
			lock (_queueLock)
			{
				while (_queuedActions.Count > 0)
				{
					Action action = _queuedActions[0];
					_executingActions.Add(action);
					_queuedActions.RemoveAt(0);
				}
			}
		}
	}
}