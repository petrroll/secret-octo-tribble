# Petrrollus

###About:
A simple 1v1 [robocode](http://robocode.sourceforge.net/) robot with a basic radar, movement, and fire functionality. It was written in a few hours so it's neither very complex nor too well written. 

Despite being quite simple it incorporates some interesting concepts such as absolute modularity (all systems are independent interfaced modules that can be changed on the fly), extensive enemy information tracking (with history), and two factor enemy movement prediction (that doesn't work very well, frankly). 

It also includes a nice generic circular array class that is used as history information storage and a debug drawer class. 

###Current status:
The code could very easily be expanded apon and probably finished to a state in which the robot would actually be quite competetive but as it is now it just doesn't work very well. Espacially the movement module is broken (it follows other robots on straight line making itself an easy target) and the fire-prediction module actually yields worse results than simple linear prediction would. 

###License:
Do whatever you wish with it :).
