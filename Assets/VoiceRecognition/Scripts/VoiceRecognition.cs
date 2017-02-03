using UnityEngine;
using System.Runtime.InteropServices;

public static class VoiceRecognition
{

	// These should be enum values, not strings.  
	// Unity does not allow SendMessage() from Java to pass ints, only strings, 
	public const string kListenerStarted = "LISTENER_STARTED";
	public const string kListenerStopped = "LISTENER_STOPPED";
	public const string kListenerComplete = "LISTENER_COMPLETE";
	public const string kListenerResult = "LISTENER_RESULT";
	public const string kListenerProcessing = "LISTENER_PROCESSING";
	public const string kListenerError = "ERROR";
	public static readonly char[] kListenerMessageDelminators = { '|' };

	#if UNITY_EDITOR

	public static void Init(string voiceRecognitionMessageObject = "", string voiceRecognitionMessageCallback = "") {
		Debug.Log ("Called Init()");
		return;
	}

	public static int StartListener() {
		Debug.Log ("Called StartListener()");
		return 0;
	}

	public static int StopListener() {
		Debug.Log ("Called StopListener()");
		return 0;
	}

	public static void SetCredentials(string host, string accessKey) {
		Debug.Log ("Called SetCredentials()");
		return;
	}

	public static int GetState() {
		//Debug.Log ("Called GetState()");
		return 0;
	}

	#elif UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void _VoiceRecognitionInit(string objName, string msg);

	[DllImport("__Internal")]
	private static extern int _VoiceRecognitionStartListener();

	[DllImport("__Internal")]
	private static extern int _VoiceRecognitionStopListener();

	[DllImport("__Internal")]
	private static extern void _VoiceRecognitionSetCredentials(string host, string accessKey);

	[DllImport("__Internal")]
	private static extern int _VoiceRecognitionGetState();

	public static void Init(string voiceRecognitionMessageObject = "", string voiceRecognitionMessageCallback = "") {
	{
		//Give the activity instance to my own static method to launch an activity from there
		Debug.Log ("VoiceRecognition: Calling Init from Unity");
		_VoiceRecognitionInit();
		_VoiceRecognitionSetCallback(voiceRecognitionMessageObject, voiceRecognitionMessageCallback);
	}

	public static int StartListener()
	{
		Debug.Log ("VoiceRecognition: Calling StartListener from Unity");
		return _VoiceRecognitionStartListener();
	}

	public static int StopListener()
	{
		Debug.Log ("VoiceRecognition: Calling StopListener from Unity");
		return _VoiceRecognitionStopListener();
	}


	public static void SetCredentials(string host, string accessKey) 
	{
		Debug.Log (string.Format("VoiceRecognition: Calling SetServerInfo from Unity ({0}. {1})", host, accessKey));
		_VoiceRecognitionSetCredentials(host, accessKey);
	}

	public static int GetState() {
		Debug.Log ("VoiceRecognition: Calling GetState()");
		return _VoiceRecognitionGetState();
	}

	#elif UNITY_ANDROID

	private static AndroidJavaObject voiceRecognition;

	public static void Init(string voiceRecognitionMessageObject = "", string voiceRecognitionMessageCallback = "") {
		// Get the current activity and save it
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

		//Give the activity instance to my own static method to launch an activity from there
		Debug.Log ("VoiceRecognition: Calling Init from Unity");
		AndroidJavaClass voiceRecognitionClass = new AndroidJavaClass("com.dolcevista.services.VoiceRecognition");
		voiceRecognition = voiceRecognitionClass.CallStatic<AndroidJavaObject> ("getInstance");
		voiceRecognition.Call("initialize", unityActivity);
		voiceRecognition.Call("setCallback", voiceRecognitionMessageObject, voiceRecognitionMessageCallback);

	}
	 
	public static int StartListener()
	{
		Debug.Log ("VoiceRecognition: Calling StartListener from Unity");
		return voiceRecognition.Call<int>("startListener");
		//return result;
	}
	
	public static int StopListener()
	{
		Debug.Log ("VoiceRecognition: Calling StopListener from Unity");
		return voiceRecognition.Call<int>("stopListener");
	}
	
	public static void SetCredentials(string host, string accessKey) 
	{
		Debug.Log (string.Format("VoiceRecognition: Calling SetServerInfo from Unity ({0}, {1})", host, accessKey));
		voiceRecognition.Call("setCredentials", host, accessKey);
	}

	public static int GetState() 
	{
		//Debug.Log ("Called GetState()");
		return voiceRecognition.Call<int>("getState");
	}

	#else
	// Windows/Mac/etc (not editor)
	// TODO:  Not a current target, implementation required)
	#endif
}

