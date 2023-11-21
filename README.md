# Geospatial Solar

Combining the Google Geospatial API with the Google Solar API for instant analysis of a building's solar potential by simply tapping it.

## Description

The app used the Google Geospatial Geometry to allow the user to tap on a building. The app then uses the Google Geospatial API to get the location of the tapped building and then queries the Google Solar API using that location. Once the app receives the data it visualises the potential area and number of solar panels in AR.

## Getting Started

### Installing

There is a working APK [HERE](https://drive.google.com/file/d/19tRh82fHF7GFrva5uZe83ZIqYzipyJ0z/view?usp=sharing)

The APK was built with a custom keystore so the ARCore Extensions API key will only work with the built APK above.

Please enter your own ARCore Extensions API key to build and run the project.

A valid Google Solar API key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/solar2 find the GeospatialController object in the hierarchy and add the API key to the "Solar Key" variable in the inspector.

## Authors

[@mechpil0t](https://twitter.com/mechpil0t)

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
