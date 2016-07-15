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
