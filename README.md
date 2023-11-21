# Geospatial Route Check

This app provides a user with a route to a destination but also checks for additional requested locations along the way such as convenience stores, bus stops, or toilets.

## Description

The app uses Google Maps to allow users to tap on a destination they wish to walk to. The app then uses the Google Directions API to generate a route that is visualised on the Google Map. Then the app checks for POIs along the route using the Geoapify API. The resulting POIs are then displayed on the Google Map and in AR using the Google Geospatial API.

## Getting Started

### Installing

There is a working APK [HERE](https://drive.google.com/file/d/1cdp4y3fEac3n5wU3warYKu6LpXC8odwe/view?usp=sharing)

The APK was built with a custom keystore so the ARCore Extensions API key will only work with the built APK above.

Please enter your own ARCore Extensions API key to build and run the project.

A valid Google Directions API key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/V2routeCheck find the RouteCheck object in the hierarchy and add the API key to the "Api" variable in the inspector of the "directionsPOI" script.

This project uses the "Google Maps View" Unity asset so a valid Google Maps API key needs to be added to the Unity project.

To add the key go Window -> Google Maps View -> Edit Settings in the Unity Editor. More information can be found [here](https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=_3-set-the-api-key-in-unity-project-settings).

A valid [Geoapify API](https://apidocs.geoapify.com/) key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/V2routeCheck find the Travel object in the hierarchy and add the Geoapify API key to the "Api" variable in the inspector of the "travelAPI" script.

## Authors

[@mechpil0t](https://twitter.com/mechpil0t)

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
