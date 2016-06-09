# FitzWorld_Template
This is a Unity template project to allow the easy creation of FitzWorld environments

## Steps to do
1. Open this project in Unity
1. Create an empty GameObject, call it WorldManager
  1. Add script component ErgManager to WorldManager
  1. Add script component TrackManager to WorldManager
  1. Add script component StatDisplayManager to WorldManager
  1. Add script component CameraManager to WorldManager
1. Create an empty GameObject, call it BoatType
  1. Add script component Boat to BoatType
  1. Add some geometry, this will be the boat the user sees
  1. Add a RigidBody component, this will be used to collide with the trackparts. Use constraints to disable it from moving on its own.
  1. Optional: Add a TextMesh to display the boats name, put it into the Boat-Script configuration
  1. Optional: Add a TextMesh to display the boats distance, put it into the Boat-Script configuration
  1. Turn this GameObject into a Prefab
1. Create an empty GameObject, call it TrackPartType
  1. Add some geometry, this will be a part of the endless track the user sees
  1. Add an empty GameObject, call it FrontDockingPoint. It should be on the front end of the track (where the boat rows to, Z=0.0, Y=0.0, X=0.0)
  1. Add an empty GameObject, call it BackDockingPoint. It should be on the back end of the track, (where the boat comes from, Z=0.0, Y=0.0)
  1. Add an invisible collision cube to the end of the part (this is where the boat collides and new parts are generated)
    1. Add script component CollNotifierTrackpart to the collision cube
1. Create an empty GameObject, call it GoalType
  1. TODO
1. Create an empty GameObject, call it StatDisplay
  1. TODO
1. Create empty GameObjects as Child for WorldManager. These will be our cameras. Place them where you want the cmas to be
1. Now fill in the configuration for the WorldManager and its scripts
  1. ErgManager    
    1. BoatType: This is where your BoatType-Prefab is used
    1. NumLanes: How many lanes should be supported?
    1. Player Lane: Which lane should the player go?
    1. Lane Distance: Width of each lane
    1. Address: The ZMQ address string to connect to
  1. TrackManager
    1. PartType: This is where your TrackPartType is used
    1. GoalType: This is where your GoalType is used
    1. Trackparts per Goal: How many trackparts should be done until a goal is instantiated?
    1. MaxParts: How many parts should be instantiated at once?
  1. StatDisplayManager
    1. Slots: Define the slots where stats should be displayed onto
  1. CameraManager
    1. TransitionFactor: How fast should the camera switch between two positions

## TODO (first version, will be turned into issues later on)
* This is a very first version... I'd do a lot of things in another way if I'd redo everything...
* This is just for boats, it could easily be more abstract and support bicycles and runners too
* I'm not happy with the Dead Reckoning (it jitters when Ergs are received from different sources)
