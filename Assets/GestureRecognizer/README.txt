Gesture Recognizer

Steps:
1 - Create gesture files on menu "Assets/Create/Gestures/Pattern"
2 - Select the gesture file created and draw a gesture pattern by clicking on grid
3 - You can give an "id" to the gesture pattern to identify it later
4 - On your scene, attach a "Recognizer" component on a GameObject
5 - Fill the "Patterns" list with your gesture pattern assets
6 - Capture player gesture on a List<Vector2> (keep in mind that y-axis must point up)
7 - Use the "Recognize" method on "Recognizer" component to find the user gesture

Take a look at example scene at "GestureRecognizer/Example/Test"