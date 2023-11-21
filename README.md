# PlusCode Collector

This app is designed to encourage exploration and activity by rewarding users for walking around their environment. For each PlusCode a user visits, a small bonus is added to their Square gift card.

## Description

This app is designed to encourage activity and also exploration, encouraging people to take alternative routes and visit spaces they would not otherwise travel through.

The app uses [Google Plus Codes](https://maps.google.com/pluscodes/), an open-source digital addressing system, to divide the world into a grid. Users can collect each Plus Code in the grid by walking through it and for each Plus Code collected they will receive a small bonus added to their Square gift card.

## Getting Started

### Installing

There is a working APK [HERE](https://drive.google.com/file/d/1dM-4XXmpMviFivdRLV4_dr20D6tNsdCO/view?usp=sharing)

The APK was built with a custom keystore so the ARCore Extensions API key will only work with the built APK above.

Please enter your own ARCore Extensions API key to build and run the project.

A valid Google Places/Pluscode API key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/squareV3 find the "GPS" object in the hierarchy and add the API key to the "Api" variable in the inspector of the "setGPS" script.

A valid Google Text-to-Speech API key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/squareV3 find the "Google Text to Speech" and "Google Text to Speech2" objects in the hierarchy and add the API key to the "Api Key" variable in the inspector of the "TextToSpeech" script.

This project uses the "Google Maps View" Unity asset so a valid Google Maps API key needs to be added to the Unity project.

To add the key go Window -> Google Maps View -> Edit Settings in the Unity Editor. More information can be found [here](https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=_3-set-the-api-key-in-unity-project-settings).

For the Square API part of the app to work a valid [Square API](https://developer.squareup.com/) key needs to be added to the Unity scene.

In the scene /Assets/Main/Scenes/squareV3 find the "Square" object in the hierarchy and add the API key to the "Access Token" variable in the inspector of the "SquareAPI" script. However if you want to try the app without this functionality you can disable it by checking the "Debug Mode" option on the inspector of the "SquareAPI" script.

## Authors

[@mechpil0t](https://twitter.com/mechpil0t)

## License

This project is licensed under the MIT License - see the LICENSE.md file for details
