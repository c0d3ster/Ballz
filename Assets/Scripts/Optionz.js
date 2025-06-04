#pragma strict

//stores difficulty multiplier (1.2 = easy, 1 = medium, .8 = hard)
static var diff : double = 1.0; // Initialize with default value

// Control options - can have multiple enabled
static var useTarget : boolean = true;
static var useAccelerometer : boolean = true;  // Mobile only
static var useJoystick : boolean = true;
static var useKeyboard : boolean = true;       // Desktop only

static function ChangeDifficulty() {
	if (!diff) diff = 1.0; // Safety check in case static var is reset
	
	switch (diff){
		case 1:
			diff = .8;
			break;
		case .8:
			diff = 1.2;
			break;
		case 1.2:
			diff = 1;
			break;
		default:
			diff = 1.0; // Reset to medium if unknown state
			break;
	}
}

static function DisplayDifficulty() {
	var diffString : String;

	switch (diff){
		case 1:
			diffString = "Medium";
			break;
		case .8:
			diffString = "Hard";
			break;
		case 1.2:
			diffString = "Easy";
			break;
		default:
			diffString = "Unknown";
			break;
	}
	return diffString;
}

static function SetDifficulty(difficulty : String) {
	switch(difficulty.ToLower()) {
		case "easy":
			diff = 1.2;
			break;
		case "medium":
			diff = 1.0;
			break;
		case "hard":
			diff = 0.8;
			break;
		default:
			Debug.LogWarning("Invalid difficulty: " + difficulty);
			diff = 1.0; // Default to medium
			break;
	}
}