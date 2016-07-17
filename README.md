# TrafficCamBot
Microsoft Bot Framework bot for viewing traffic cameras.

# Example dialog:
![alt tag](https://raw.githubusercontent.com/scottmcmaster/TrafficCamBot/master/ExampleDialog.PNG)

# Supported Interactions
## view
* Selects a city/set of traffic cameras to view. Sets this as state in the conversation.  
* Examples:
  * `view bay area`
  * `View the traffic cameras for Seattle`

## list
* Lists the names of all of the cameras for the currently-selected city.
* Examples:
  * `list`

## _(enter a camera name)_
* This is how you view a camera image. The bot keeps an index of a number of alternative names (e.g. variations on "st", "street", etc.) where possible, and uses some heuristics (including a Lucene index) to try and
figure out what the user meant.  
* The most reliable way to get a result, obviously, is to enter an exact camera name (case-insensitive).
* But certain more-conversational styles will often work as well, e.g. "Show me the traffic camera at Sunset Blvd".
* These are based on the phrase-search query scoring in the Lucene index, so ymmv.  
* If multiple cameras may match, the user is presented with a numbered menu...

## _enter a camera number from a choice list_
* If the user was presented with a choice list by the bot, they can enter the appropriate integer to view a given camera.  
* The choice list stays in conversational state until another one needs to be presented.

# Adding a New CameraDataService
If you have a new set of traffic cameras you would like the bot to support, follow these steps:  
* Look at the [existing](https://github.com/scottmcmaster/TrafficCamBot/blob/master/TrafficCamBot/Data/SeattleCameraDataService.cs) [examples](https://github.com/scottmcmaster/TrafficCamBot/blob/master/TrafficCamBot/Data/BayAreaCameraDataService.cs).
* Create a new class in the [Data folder](https://github.com/scottmcmaster/TrafficCamBot/tree/master/TrafficCamBot/Data), TrafficCamBot.Data namespace.
* Extend [CameraDataServiceBase](https://github.com/scottmcmaster/TrafficCamBot/blob/master/TrafficCamBot/Bot/CameraDataServiceBase.cs). (You may also just implement ICameraDataService, but you'll lose lots of nice stuff like the Lucene index by default).
  * Return a unique name from the Name property. Note that this name should be displayable to the user (i.e. user-friendly and meaningful).
  * In the constructor, call SetCameraNames with a list of all of the camera names your service will understand/support.
  * Implement GetImageUrlForCamera (basically, given the name of one of your cameras, return the appropriate CameraImage structure).
  * Send a pull request.

