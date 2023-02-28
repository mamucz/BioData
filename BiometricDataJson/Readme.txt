Biometric data reader

Wrapper BiometricDataJson reads Json data from three servers.
	- CameraTracker Server
	- BreathingServer
	- HearRateServer

The addresses of the individual servers are specified in the properties of the wrapper.
The server address must be entered as the full path in the Url field of each server. 
(for example: http://192.168.168.4:5000/face-tracker)
For FaceTrackerServer it is still necessary to enter the face index in the Face field in the range [0 ... x], 0 is the first face found

Reading of individual items from servers takes place via the BiometricElement element. 
It is bound to the item on the server by the PropertyName field.
It is necessary to fill in this field with a text string corresponding to the name of the variable on server.

Name of variables for servers:

For all servers, there is a status variable that has values of 0 or 1, depending on whether the server can read data:
	0 - data not read
	1 - data read

Names of individual variables for individual servers.
	- FaceTrackerServerStatus
	- BreathingServerStatus
	- HearRateServerStatus

Note: For specific variables, a value of -1 represents an error condition.

Names of individual variables for the given server:
CameraFaceTrackerServer:
	- facetracker_status
	- faces
	- left_eye_closed
	- right_eye_closed
	- major_emotion
	- angry
	- disgust
	- fear
	- happy
	- sad
	- surprise
	- neutral

BreathingServer:
	- RR_UTS
	- RR
	- RR_avg
	- RR_min
	- RR_max

HearRateServer
	- HR_UTS
	- HR_avg
	- HR_min
	- HR_max
	- pNN30
	- pNN50
	- RMSSD