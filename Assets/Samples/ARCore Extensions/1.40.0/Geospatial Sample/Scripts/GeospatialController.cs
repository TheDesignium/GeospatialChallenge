// <copyright file="GeospatialController.cs" company="Google LLC">
//
// Copyright 2022 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Google.XR.ARCoreExtensions.Samples.Geospatial
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;
#if UNITY_ANDROID

    using UnityEngine.Android;

#endif

using TMPro;

using GoogleTextToSpeech.Scripts.Example;


    /// <summary>
    /// Controller for Geospatial sample.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "Bypass source check.")]
    public class GeospatialController : MonoBehaviour
    {
        public TextToSpeechExample text2speech;
        public SquareAPI square;

        [Header("AR Components")]
		public GameObject loadingObject;

        public bool checkPosition;
        public bool texttospeech;

    	public TMP_Text debugTxt;
        public TMP_Text distanceTxt;
        public getDistance distance;
        public setGPS gps;

        double firstlat;
        double firstlon;
        double oldlat;
        double oldlon;
        double oldlatInput;
        double oldlonInput;
        double lastDistance;

        public bool squareReady;

        public ARSessionOrigin SessionOrigin;
        public ARSession Session;
        public ARAnchorManager AnchorManager;
        public AREarthManager EarthManager;

        public ARStreetscapeGeometryManager StreetscapeGeometryManager;
        public ARCoreExtensions ARCoreExtensions;
        public List<Material> StreetscapeGeometryMaterialBuilding;
        public Material StreetscapeGeometryMaterialTerrain;

        public GameObject GeospatialPrefab;
        public GameObject TerrainPrefab;

        public Text SnackBarText;
        public Text DebugText;

        private const string _localizingMessage = "Localizing your device to set anchor.";
        private const string _localizationInitializingMessage =
            "Initializing Geospatial functionalities.";
        private const string _localizationInstructionMessage =
            "Point your camera at buildings, stores, and signs near you.";
        private const string _localizationFailureMessage =
            "Localization not possible.\n" +
            "Close and open the app to restart the session.";
        private const string _localizationSuccessMessage = "Localization completed.";
        private const string _resolvingTimeoutMessage =
            "Still resolving the terrain anchor.\n" +
            "Please make sure you're in an area that has VPS coverage.";
        private const string _hasDisplayedPrivacyPromptKey = "HasDisplayedGeospatialPrivacyPrompt";
        private const string _persistentGeospatialAnchorsStorageKey = "PersistentGeospatialAnchors";

        private const double _orientationYawAccuracyThreshold = 25;
        private const double _headingAccuracyThreshold = 25;
        private const double _horizontalAccuracyThreshold = 20;

        private bool _streetscapeGeometryVisibility = false;
        private AnchorType _anchorType = AnchorType.Geospatial;
        private int _buildingMatIndex = 0;

        private Dictionary<TrackableId, GameObject> _streetscapegeometryGOs =
            new Dictionary<TrackableId, GameObject>();
        List<ARStreetscapeGeometry> _addedStreetscapeGeometrys = new List<ARStreetscapeGeometry>();
        List<ARStreetscapeGeometry> _updatedStreetscapeGeometrys =
            new List<ARStreetscapeGeometry>();
        List<ARStreetscapeGeometry> _removedStreetscapeGeometrys =
            new List<ARStreetscapeGeometry>();

        private bool _clearStreetscapeGeometryRenderObjects = false;

        private bool _waitingForLocationService = false;
        private bool _isInARView = false;
        private bool _isReturning = false;
        private bool _isLocalizing = false;
        private bool _enablingGeospatial = false;
        private bool _shouldResolvingHistory = false;
        private float _localizationPassedTime = 0f;
        private float _configurePrepareTime = 3f;
        public List<GameObject> _anchorObjects = new List<GameObject>();
        private IEnumerator _startLocationService = null;
        private IEnumerator _asyncCheck = null;

        //public GeospatialAnchorHistory _autosetA;
        public Transform[] mapSize;

        void Start()
        {
			distanceTxt.text = "0m";
        }

        public void OnGetStartedClicked()
        {
            PlayerPrefs.SetInt(_hasDisplayedPrivacyPromptKey, 1);
            PlayerPrefs.Save();
            SwitchToARView(true);
			      loadingObject.SetActive(true);
        }

        public void OnClearAllClicked()
        {
            foreach (var anchor in _anchorObjects)
            {
                Destroy(anchor);
            }

            _anchorObjects.Clear();
        }

        public void OnGeometryToggled(bool enabled)
        {
            _streetscapeGeometryVisibility = enabled;
            if (!_streetscapeGeometryVisibility)
            {
                _clearStreetscapeGeometryRenderObjects = true;
            }
        }

        public void OnGeospatialAnchorToggled(bool enabled)
        {
            // GeospatialAnchorToggle.GetComponent<Toggle>().isOn = true;;
            _anchorType = AnchorType.Geospatial;
        }

        public void OnTerrainAnchorToggled(bool enabled)
        {
            // TerrainAnchorToggle.GetComponent<Toggle>().isOn = true;
            _anchorType = AnchorType.Terrain;
        }

        public void OnRooftopAnchorToggled(bool enabled)
        {
            // RooftopAnchorToggle.GetComponent<Toggle>().isOn = true;
            _anchorType = AnchorType.Rooftop;
        }

        public void Awake()
        {
            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// Unity's OnEnable() method.
        /// </summary>
        public void OnEnable()
        {
            _startLocationService = StartLocationService();
            StartCoroutine(_startLocationService);

            _isReturning = false;
            _enablingGeospatial = false;
            DebugText.gameObject.SetActive(Debug.isDebugBuild && EarthManager != null);

            _localizationPassedTime = 0f;
            _isLocalizing = true;
            SnackBarText.text = _localizingMessage;

            SwitchToARView(PlayerPrefs.HasKey(_hasDisplayedPrivacyPromptKey));

            if (StreetscapeGeometryManager == null)
            {
                Debug.LogWarning("StreetscapeGeometryManager must be set in the " +
                    "GeospatialController Inspector to render StreetscapeGeometry.");
            }

            if (StreetscapeGeometryMaterialBuilding.Count == 0)
            {
                Debug.LogWarning("StreetscapeGeometryMaterialBuilding in the " +
                    "GeospatialController Inspector must contain at least one material " +
                    "to render StreetscapeGeometry.");
                return;
            }

            if (StreetscapeGeometryMaterialTerrain == null)
            {
                Debug.LogWarning("StreetscapeGeometryMaterialTerrain must be set in the " +
                    "GeospatialController Inspector to render StreetscapeGeometry.");
                return;
            }

            OnGetStartedClicked();
        }

        public void OnDisable()
        {
            StopCoroutine(_asyncCheck);
            _asyncCheck = null;
            StopCoroutine(_startLocationService);
            _startLocationService = null;
            Debug.Log("Stop location services.");
            Input.location.Stop();

            foreach (var anchor in _anchorObjects)
            {
                Destroy(anchor);
            }

            _anchorObjects.Clear();
        }

        public void Update()
        {

            if(Input.GetKeyUp(KeyCode.Alpha5))
            {
              int totalGain = 5;
              string message = "You have walked " + "5km" + " and earned " + totalGain + " yen, the total balance on your gift card is " + 33.ToString() + " yen";
              text2speech.remoteRequest(message);
            }
            if (!_isInARView)
            {
                return;
            }

            // Check session error status.
            LifecycleUpdate();
            if (_isReturning)
            {
                return;
            }

            if (ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                return;
            }

            // Check feature support and enable Geospatial API when it's supported.
            var featureSupport = EarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            switch (featureSupport)
            {
                case FeatureSupported.Unknown:
                    return;
                case FeatureSupported.Unsupported:
                    ReturnWithReason("The Geospatial API is not supported by this device.");
                    return;
                case FeatureSupported.Supported:
                    if (ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                        GeospatialMode.Disabled)
                    {
                        Debug.Log("Geospatial sample switched to GeospatialMode.Enabled.");
                        ARCoreExtensions.ARCoreExtensionsConfig.GeospatialMode =
                            GeospatialMode.Enabled;
                        ARCoreExtensions.ARCoreExtensionsConfig.StreetscapeGeometryMode =
                            StreetscapeGeometryMode.Enabled;
                        _configurePrepareTime = 3.0f;
                        _enablingGeospatial = true;
                        return;
                    }

                    break;
            }

            // Waiting for new configuration to take effect.
            if (_enablingGeospatial)
            {
                _configurePrepareTime -= Time.deltaTime;
                if (_configurePrepareTime < 0)
                {
                    _enablingGeospatial = false;
                }
                else
                {
                    return;
                }
            }

            // Check earth state.
            var earthState = EarthManager.EarthState;
            if (earthState == EarthState.ErrorEarthNotReady)
            {
                SnackBarText.text = _localizationInitializingMessage;
                return;
            }
            else if (earthState != EarthState.Enabled)
            {
                string errorMessage =
                    "Geospatial sample encountered an EarthState error: " + earthState;
                Debug.LogWarning(errorMessage);
                SnackBarText.text = errorMessage;
                return;
            }

            // Check earth localization.
            bool isSessionReady = ARSession.state == ARSessionState.SessionTracking &&
                Input.location.status == LocationServiceStatus.Running;
            var earthTrackingState = EarthManager.EarthTrackingState;
            var pose = earthTrackingState == TrackingState.Tracking ?
                EarthManager.CameraGeospatialPose : new GeospatialPose();
            if (!isSessionReady || earthTrackingState != TrackingState.Tracking ||
                pose.OrientationYawAccuracy > _orientationYawAccuracyThreshold ||
                pose.HorizontalAccuracy > _horizontalAccuracyThreshold)
            {
                // Lost localization during the session.
                if (!_isLocalizing)
                {
                    _isLocalizing = true;
                    _localizationPassedTime = 0f;
                    foreach (var go in _anchorObjects)
                    {
                        go.SetActive(false);
                    }
                }

                    _localizationPassedTime += Time.deltaTime;
                    SnackBarText.text = _localizationInstructionMessage;
            }
            else if (_isLocalizing)
            {
                _isLocalizing = false;
                _localizationPassedTime = 0f;
                SnackBarText.text = _localizationSuccessMessage;
                foreach (var go in _anchorObjects)
                {
                    go.SetActive(true);
                }

				        loadingObject.SetActive(false);

                if(checkPosition == true)
                {
                  checkPosition = false;
                  StartCoroutine("positionLoop");
                }
            }
            else
            {
                if (_streetscapeGeometryVisibility)
                {
                    // get access to ARstreetscapeGeometries in ARStreetscapeGeometryManager
                    if (StreetscapeGeometryManager)
                    {
                        StreetscapeGeometryManager.StreetscapeGeometriesChanged
                            += (ARStreetscapeGeometrysChangedEventArgs) =>
                        {
                            _addedStreetscapeGeometrys =
                                ARStreetscapeGeometrysChangedEventArgs.Added;
                            _updatedStreetscapeGeometrys =
                                ARStreetscapeGeometrysChangedEventArgs.Updated;
                            _removedStreetscapeGeometrys =
                                ARStreetscapeGeometrysChangedEventArgs.Removed;
                        };
                    }

                    foreach (
                        ARStreetscapeGeometry streetscapegeometry in _addedStreetscapeGeometrys)
                    {
                        InstantiateRenderObject(streetscapegeometry);
                    }

                    foreach (
                        ARStreetscapeGeometry streetscapegeometry in _updatedStreetscapeGeometrys)
                    {
                        // This second call to instantiate is required if geometry is toggled on
                        // or off after the app has started.
                        InstantiateRenderObject(streetscapegeometry);
                        UpdateRenderObject(streetscapegeometry);
                    }

                    foreach (
                        ARStreetscapeGeometry streetscapegeometry in _removedStreetscapeGeometrys)
                    {
                        DestroyRenderObject(streetscapegeometry);
                    }
                }
                else if (_clearStreetscapeGeometryRenderObjects)
                {
                    DestroyAllRenderObjects();
                    _clearStreetscapeGeometryRenderObjects = false;
                }

            }

            if (earthTrackingState == TrackingState.Tracking)
            {

            }
            else
            {
                //InfoText.text = "GEOSPATIAL POSE: not tracking";
            }
        }

        private void InstantiateRenderObject(ARStreetscapeGeometry streetscapegeometry)
        {
            if (streetscapegeometry.mesh == null)
            {
                return;
            }

            // Check if a render object already exists for this streetscapegeometry and
            // create one if not.
            if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
            {
                return;
            }

            GameObject renderObject = new GameObject(
                "StreetscapeGeometryMesh", typeof(MeshFilter), typeof(MeshRenderer));

            if (renderObject)
            {
                renderObject.transform.position = new Vector3(0, 0.5f, 0);
                renderObject.GetComponent<MeshFilter>().mesh = streetscapegeometry.mesh;

                // Add a material with transparent diffuse shader.
                if (streetscapegeometry.streetscapeGeometryType ==
                    StreetscapeGeometryType.Building)
                {
                    renderObject.GetComponent<MeshRenderer>().material =
                        StreetscapeGeometryMaterialBuilding[_buildingMatIndex];
                    _buildingMatIndex =
                        (_buildingMatIndex + 1) % StreetscapeGeometryMaterialBuilding.Count;
                }
                else
                {
                    renderObject.GetComponent<MeshRenderer>().material =
                        StreetscapeGeometryMaterialTerrain;
                }

                renderObject.transform.position = streetscapegeometry.pose.position;
                renderObject.transform.rotation = streetscapegeometry.pose.rotation;

                _streetscapegeometryGOs.Add(streetscapegeometry.trackableId, renderObject);
            }
        }

        /// <summary>
        /// Updates the render object transform based on this streetscapegeometrys pose.
        /// It must be called every frame to update the mesh.
        /// </summary>
        /// <param name="streetscapegeometry">The <c><see cref="ARStreetscapeGeometry"/></c>
        /// object containing the mesh to be rendered.</param>
        private void UpdateRenderObject(ARStreetscapeGeometry streetscapegeometry)
        {
            if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
            {
                GameObject renderObject = _streetscapegeometryGOs[streetscapegeometry.trackableId];
                renderObject.transform.position = streetscapegeometry.pose.position;
                renderObject.transform.rotation = streetscapegeometry.pose.rotation;
            }
        }

        /// <summary>
        /// Destroys the render object associated with the
        /// <c><see cref="ARStreetscapeGeometry"/></c>.
        /// </summary>
        /// <param name="streetscapegeometry">The <c><see cref="ARStreetscapeGeometry"/></c>
        /// containing the render object to be destroyed.</param>
        private void DestroyRenderObject(ARStreetscapeGeometry streetscapegeometry)
        {
            if (_streetscapegeometryGOs.ContainsKey(streetscapegeometry.trackableId))
            {
                var geometry = _streetscapegeometryGOs[streetscapegeometry.trackableId];
                _streetscapegeometryGOs.Remove(streetscapegeometry.trackableId);
                Destroy(geometry);
            }
        }

        /// <summary>
        /// Destroys all stored <c><see cref="ARStreetscapeGeometry"/></c> render objects.
        /// </summary>
        private void DestroyAllRenderObjects()
        {
            var keys = _streetscapegeometryGOs.Keys;
            foreach (var key in keys)
            {
                var renderObject = _streetscapegeometryGOs[key];
                Destroy(renderObject);
            }

            _streetscapegeometryGOs.Clear();
        }

        private IEnumerator CheckRooftopPromise(ResolveAnchorOnRooftopPromise promise,
            GeospatialAnchorHistory history)
        {
            var retry = 0;
            while (promise.State == PromiseState.Pending)
            {
                if (retry == 100)
                {
                    SnackBarText.text = _resolvingTimeoutMessage;
                }

                yield return new WaitForSeconds(0.1f);
                retry = Math.Min(retry + 1, 100);
            }

            var result = promise.Result;
            if (result.RooftopAnchorState == RooftopAnchorState.Success &&
                result.Anchor != null)
            {
                // Adjust the scale of the prefab anchor object to maintain visibility when it is
                // far away.
                result.Anchor.gameObject.transform.localScale *= GetRooftopAnchorScale(
                    result.Anchor.gameObject.transform.position,
                    Camera.main.transform.position);
                GameObject anchorGO = Instantiate(TerrainPrefab,
                    result.Anchor.gameObject.transform);
                anchorGO.transform.parent = result.Anchor.gameObject.transform;

                _anchorObjects.Add(result.Anchor.gameObject);

                SnackBarText.text = GetDisplayStringForAnchorPlacedSuccess();
            }
            else
            {
                SnackBarText.text = GetDisplayStringForAnchorPlacedFailure();
            }

            yield break;
        }

        private IEnumerator CheckTerrainPromise(ResolveAnchorOnTerrainPromise promise,
            GeospatialAnchorHistory history)
        {
            var retry = 0;
            while (promise.State == PromiseState.Pending)
            {
                if (retry == 100)
                {
                    SnackBarText.text = _resolvingTimeoutMessage;
                }

                yield return new WaitForSeconds(0.1f);
                retry = Math.Min(retry + 1, 100);
            }

            var result = promise.Result;
            if (result.TerrainAnchorState == TerrainAnchorState.Success &&
                result.Anchor != null)
            {
                GameObject anchorGO = Instantiate(TerrainPrefab,
                    result.Anchor.gameObject.transform);
                anchorGO.transform.parent = result.Anchor.gameObject.transform;

                _anchorObjects.Add(result.Anchor.gameObject);

                SnackBarText.text = GetDisplayStringForAnchorPlacedSuccess();
            }
            else
            {
                SnackBarText.text = GetDisplayStringForAnchorPlacedFailure();
            }

            yield break;
        }

        private float GetRooftopAnchorScale(Vector3 anchor, Vector3 camera)
        {
            // Return the scale in range [1, 2] after mapping a distance between camera and anchor
            // to [2, 20].
            float distance =
                Mathf.Sqrt(
                    Mathf.Pow(anchor.x - camera.x, 2.0f)
                    + Mathf.Pow(anchor.y - camera.y, 2.0f)
                    + Mathf.Pow(anchor.z - camera.z, 2.0f));
            float mapDistance = Mathf.Min(Mathf.Max(2.0f, distance), 20.0f);
            return (mapDistance - 2.0f) / (20.0f - 2.0f) + 1.0f;
        }

        private GeospatialAnchorHistory CreateHistory(Pose pose, AnchorType anchorType)
        {
            GeospatialPose geospatialPose = EarthManager.Convert(pose);

            GeospatialAnchorHistory history = new GeospatialAnchorHistory(
                geospatialPose.Latitude, geospatialPose.Longitude, geospatialPose.Altitude,
                anchorType, geospatialPose.EunRotation);
            return history;
        }

        private Quaternion CreateRotation(GeospatialAnchorHistory history)
        {
            Quaternion eunRotation = history.EunRotation;
            if (eunRotation == Quaternion.identity)
            {
                // This history is from a previous app version and EunRotation was not used.
                eunRotation =
                    Quaternion.AngleAxis(180f - (float)history.Heading, Vector3.up);
            }
            return eunRotation;
        }

        private ARAnchor PlaceARAnchor(GeospatialAnchorHistory history, Pose pose = new Pose(),
            TrackableId trackableId = new TrackableId())
        {
            Quaternion eunRotation = CreateRotation(history);
            ARAnchor anchor = null;
            switch (history.AnchorType)
            {
                case AnchorType.Rooftop:
                    ResolveAnchorOnRooftopPromise rooftopPromise =
                        AnchorManager.ResolveAnchorOnRooftopAsync(
                            history.Latitude, history.Longitude,
                            0, eunRotation);

                    StartCoroutine(CheckRooftopPromise(rooftopPromise, history));

                    return null;
                case AnchorType.Terrain:
                    ResolveAnchorOnTerrainPromise terrainPromise =
                        AnchorManager.ResolveAnchorOnTerrainAsync(
                            history.Latitude, history.Longitude,
                            0, eunRotation);

                    StartCoroutine(CheckTerrainPromise(terrainPromise, history));

                    return null;
                case AnchorType.Geospatial:
                    ARStreetscapeGeometry streetscapegeometry =
                        StreetscapeGeometryManager.GetStreetscapeGeometry(trackableId);
                    if (streetscapegeometry != null)
                    {
                        anchor = StreetscapeGeometryManager.AttachAnchor(
                            streetscapegeometry, pose);
                    }

                    if (anchor != null)
                    {
                        SnackBarText.text = GetDisplayStringForAnchorPlacedSuccess();
                    }
                    else
                    {
                        SnackBarText.text = GetDisplayStringForAnchorPlacedFailure();
                    }

                    break;
            }

            return anchor;
        }

        private ARGeospatialAnchor PlaceGeospatialAnchor(
            GeospatialAnchorHistory history)
        {
            bool terrain = history.AnchorType == AnchorType.Terrain;
            Quaternion eunRotation = CreateRotation(history);
            ARGeospatialAnchor anchor = null;

            if (terrain)
            {
                // Anchor returned will be null, the coroutine will handle creating the
                // anchor when the promise is done.
                ResolveAnchorOnTerrainPromise promise =
                    AnchorManager.ResolveAnchorOnTerrainAsync(
                        history.Latitude, history.Longitude,
                        0, eunRotation);

                StartCoroutine(CheckTerrainPromise(promise, history));
                return null;
            }
            else
            {
                anchor = AnchorManager.AddAnchor(
                    history.Latitude, history.Longitude, history.Altitude, eunRotation);
            }

            if (anchor != null)
            {
                GameObject anchorGO = history.AnchorType == AnchorType.Geospatial ?
                    Instantiate(GeospatialPrefab, anchor.transform) :
                    Instantiate(TerrainPrefab, anchor.transform);
                anchor.gameObject.SetActive(!terrain);
                anchorGO.transform.parent = anchor.gameObject.transform;
                _anchorObjects.Add(anchor.gameObject);
            }
            else
            {
                SnackBarText.text = GetDisplayStringForAnchorPlacedFailure();
            }

            return anchor;
        }

        private void SwitchToARView(bool enable)
        {
            _isInARView = enable;
            SessionOrigin.gameObject.SetActive(enable);
            Session.gameObject.SetActive(enable);
            ARCoreExtensions.gameObject.SetActive(enable);
            if (enable && _asyncCheck == null)
            {
                _asyncCheck = AvailabilityCheck();
                StartCoroutine(_asyncCheck);
            }
        }

        private IEnumerator AvailabilityCheck()
        {
            if (ARSession.state == ARSessionState.None)
            {
                yield return ARSession.CheckAvailability();
            }

            // Waiting for ARSessionState.CheckingAvailability.
            yield return null;

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                yield return ARSession.Install();
            }

            // Waiting for ARSessionState.Installing.
            yield return null;
#if UNITY_ANDROID

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                Debug.Log("Requesting camera permission.");
                Permission.RequestUserPermission(Permission.Camera);
                yield return new WaitForSeconds(3.0f);
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                // User has denied the request.
                Debug.LogWarning(
                    "Failed to get the camera permission. VPS availability check isn't available.");
                yield break;
            }
#endif

            while (_waitingForLocationService)
            {
                yield return null;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarning(
                    "Location services aren't running. VPS availability check is not available.");
                yield break;
            }

            // Update event is executed before coroutines so it checks the latest error states.
            if (_isReturning)
            {
                yield break;
            }

            var location = Input.location.lastData;
            var vpsAvailabilityPromise =
                AREarthManager.CheckVpsAvailabilityAsync(location.latitude, location.longitude);
            yield return vpsAvailabilityPromise;

            Debug.LogFormat("VPS Availability at ({0}, {1}): {2}",
                location.latitude, location.longitude, vpsAvailabilityPromise.Result);
            //VPSCheckCanvas.SetActive(vpsAvailabilityPromise.Result != VpsAvailability.Available);
        }

        private IEnumerator StartLocationService()
        {
            _waitingForLocationService = true;
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Requesting the fine location permission.");
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(3.0f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service is disabled by the user.");
                _waitingForLocationService = false;
                yield break;
            }

            Debug.Log("Starting location service.");
            Input.location.Start();

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return null;
            }

            _waitingForLocationService = false;
            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarningFormat(
                    "Location service ended with {0} status.", Input.location.status);
                Input.location.Stop();
            }
        }

        private void LifecycleUpdate()
        {
            // Pressing 'back' button quits the app.
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (_isReturning)
            {
                return;
            }

            // Only allow the screen to sleep when not tracking.
            var sleepTimeout = SleepTimeout.NeverSleep;
            if (ARSession.state != ARSessionState.SessionTracking)
            {
                sleepTimeout = SleepTimeout.SystemSetting;
            }

            Screen.sleepTimeout = sleepTimeout;

            // Quit the app if ARSession is in an error status.
            string returningReason = string.Empty;
            if (ARSession.state != ARSessionState.CheckingAvailability &&
                ARSession.state != ARSessionState.Ready &&
                ARSession.state != ARSessionState.SessionInitializing &&
                ARSession.state != ARSessionState.SessionTracking)
            {
                returningReason = string.Format(
                    "Geospatial sample encountered an ARSession error state {0}.\n" +
                    "Please restart the app.",
                    ARSession.state);
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                returningReason =
                    "Geospatial sample failed to start location service.\n" +
                    "Please restart the app and grant the fine location permission.";
            }

            ReturnWithReason(returningReason);
        }

        private void ReturnWithReason(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return;
            }

            Debug.LogError(reason);
            SnackBarText.text = reason;
            _isReturning = true;
        }

        private void QuitApplication()
        {
            Application.Quit();
        }

        private string GetDisplayStringForAnchorPlacedSuccess()
        {
            return "Anchor(s) Set!";
        }

         private string GetDisplayStringForAnchorPlacedFailure()
        {
            return string.Format(
                    "Failed to set a {0} anchor!", _anchorType);
        }

        public void setAnchorPlusCode(float latpc, float lonpc, int i)
        {
          GeospatialAnchorHistory h = new GeospatialAnchorHistory();
          h.Latitude = latpc;
          h.Longitude = lonpc;
          PlaceGeospatialAnchorPlusCode(h, i);
        }

        private ARGeospatialAnchor PlaceGeospatialAnchorPlusCode(
            GeospatialAnchorHistory history, int t)
        {
            var pose = EarthManager.CameraGeospatialPose;

            Quaternion eunRotation = history.EunRotation;
            if (eunRotation == Quaternion.identity)
            {
                // This history is from a previous app version and EunRotation was not used.
                eunRotation =
                    Quaternion.AngleAxis(180f - (float)history.Heading, Vector3.up);
            }

            var anchor = AnchorManager.AddAnchor(history.Latitude, history.Longitude, pose.Altitude - 1, eunRotation);
            if (anchor != null)
            {
                GameObject anchorGO = Instantiate(GeospatialPrefab, anchor.transform);
                anchor.gameObject.SetActive(true);
                _anchorObjects.Add(anchor.gameObject);
                mapSize[t].position = anchorGO.transform.position;
                mapSize[t].parent = anchorGO.transform;
            }
            else
            {

            }

            return anchor;
        }

    public string returnGeoPose()
    {
      var pose = EarthManager.CameraGeospatialPose;
      string str = pose.Latitude.ToString() + ":" + pose.Longitude.ToString() + ":" + pose.EunRotation.ToString("F2") + ":" + pose.Altitude.ToString();
      return str;
    }

    public List<string> returnLatLon()
    {
      List<string> list = new List<string>();
      var pose = EarthManager.CameraGeospatialPose;
      list.Add(pose.Latitude.ToString());
      list.Add(pose.Longitude.ToString());
      list.Add(pose.EunRotation.ToString("F2"));
      list.Add(pose.Altitude.ToString());

      return list;
    }

    public List<double> returnLatLonDbl()
    {
      List<double> list = new List<double>();
      var pose = EarthManager.CameraGeospatialPose;
      list.Add(pose.Latitude);
      list.Add(pose.Longitude);
      return list;
    }

      public void stopChecking()
      {
        StopCoroutine("positionLoop");
		Debug.Log("stop position loop");
      }
      public void startChecking()
      {
        StartCoroutine("positionLoop");
		Debug.Log("start position loop");
      }

      IEnumerator positionLoop()
      {
		    yield return new WaitForSeconds(3);

			var earthTrackingState = EarthManager.EarthTrackingState;
			float dist = 0;
			if(earthTrackingState == TrackingState.Tracking)
			{
				    GeospatialPose firstpose = EarthManager.CameraGeospatialPose;
					firstlat = firstpose.Latitude;
					firstlon = firstpose.Longitude;
					oldlat = firstlat;
					oldlon = firstlon;
			}
			else
			{
				firstlat = Input.location.lastData.latitude;
				firstlon = Input.location.lastData.longitude;
				oldlat = firstlat;
				oldlon = firstlon;
			}

		StartCoroutine(distanceLoop());


        lastDistance = 0;

        while(true)
        {
          GeospatialPose pose = EarthManager.CameraGeospatialPose;
          dist = distance.CalculateDistance((float)pose.Latitude,(float)oldlat,(float)pose.Longitude,(float)oldlon);
          if(dist > 5)
          {
            oldlat = pose.Latitude;
            oldlon = pose.Longitude;
      			if(squareReady == true)
      			{
      				gps.getPlusCodeVisited(pose.Latitude.ToString(),pose.Longitude.ToString());
      			}
          }
          yield return new WaitForSeconds(5);
    		  if(squareReady == true)
    		  {
    				gps.getPlusCodeVisited(Input.location.lastData.latitude.ToString(),Input.location.lastData.longitude.ToString());
    		  }
          yield return new WaitForSeconds(5);
        }
        yield return new WaitForEndOfFrame();
      }


      IEnumerator distanceLoop()
      {
		 Debug.Log("start distanceLoop");
		 string diststring = "0m";
		 int count = 0;
            int distcount = 1;
            float distLimit = 100;

        while(true)
        {
			yield return new WaitForSeconds(10);

			var earthTrackingState = EarthManager.EarthTrackingState;
			float dist = 0;
			if(earthTrackingState == TrackingState.Tracking)
			{
				GeospatialPose pose = EarthManager.CameraGeospatialPose;
				dist = distance.CalculateDistance((float)pose.Latitude,(float)firstlat,(float)pose.Longitude,(float)firstlon);

				Debug.Log("start distanceLoop");
			}
			else
			{
				dist = distance.CalculateDistance((float)Input.location.lastData.latitude,(float)firstlat,(float)Input.location.lastData.longitude,(float)firstlon);
			}

          float diffdistance = dist - (float)lastDistance;

          if(dist < 1000)
          {
            diststring = dist.ToString("F0") + "m";
          }
          if(dist > 1000)
          {
            diststring = (dist/1000).ToString("F1") + "km";
          }

		   Debug.Log(diststring);

          if(diffdistance > (distLimit * distcount))
          {
            lastDistance = dist;

            if (texttospeech == true)
            {
              string message = "";

              if(square.cardBalance != square.startcardBalance)
              {
                int totalGain = square.cardBalance - square.startcardBalance;
                message = "You have walked " + diststring + " and earned " + totalGain + " yen, the total balance on your gift card is " + square.cardBalance.ToString() + " yen";
              }
              else
              {
                message = "You have walked " + diststring + ", the total balance on your gift card is " + square.cardBalance.ToString() + " yen";
              }

              text2speech.remoteRequest(message);
            }
            distcount += 1;
          }
          distanceTxt.text = diststring;
		  count += 1;
        }
      }

      public void debugVoice()
      {
        GeospatialPose pose = EarthManager.CameraGeospatialPose;
        float dist = distance.CalculateDistance((float)pose.Latitude,(float)firstlat,(float)pose.Longitude,(float)firstlon);
        string diststring = "0m";
        if(dist < 1000)
        {
          diststring = dist.ToString("F0") + "m";
        }
        if(dist > 1000)
        {
          diststring = (dist/1000).ToString("F1") + "km";
        }

        string message = "";

        if(square.cardBalance != square.startcardBalance)
        {
          int totalGain = square.cardBalance - square.startcardBalance;
          message = "You have walked " + diststring + " and earned " + totalGain + " yen, the total balance on your gift card is " + square.cardBalance.ToString() + " yen";
        }
        else
        {
          message = "You have walked " + diststring + ", the total balance on your gift card is " + square.cardBalance.ToString() + " yen";
        }

        text2speech.remoteRequest(message);
      }

    }
}
