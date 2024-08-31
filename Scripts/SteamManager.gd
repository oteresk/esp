extends Control

static var steamID;
static var steamUsername;

# Called when the node enters the scene tree for the first time.
func _ready():
	Steam.steamInit()
	
	var steamRunning=Steam.isSteamRunning()
	if !steamRunning:
		print("Steam is not running.")
		return
		
	
	print("Your Steam name is: "+GetSteamName())
	Steam.inputInit()
	pass # Replace with function body.


func GetSteamName():
	steamID=Steam.getSteamID()
	steamUsername=Steam.getFriendPersonaName(steamID)
	return steamUsername
	

