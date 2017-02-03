using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoiceRecognitionExample : MonoBehaviour 
{
	public string host = "nmsps://NMDPTRIAL_rknopf_hotmail_com20161206220629@sslsandbox-nmdp.nuancemobility.net:443";
	public string key = "1a79aa102e909afb64d517c8fc589204070c7e4600076f49815bcd762447653ec00f4eee9fe8e903bd01cbba595dbe60693ca66a4415b3c571803b95c7a0d387";
	public Button listenButton;
    public RenderHeads.Media.AVProVideo.MediaPlayer mediaPlayer;
	public Text outputText;
    public string trigger;

	private string speechResult = "";

	void Start () 
	{
        outputText.text = "Test";
        Debug.Log(outputText.text);
		VoiceRecognition.Init(this.gameObject.name, "OnVoiceRecognitionMessage");
		VoiceRecognition.SetCredentials (host, key);
        Invoke("CheckListener", 1);
	}


	void Update () 
	{
		
	}

    public void CheckListener()
    {
        if (VoiceRecognition.GetState() == 0)
            VoiceRecognition.StartListener();
        else
            Invoke("CheckListener", 5);
    }

	public void StopListening() {
		VoiceRecognition.StopListener ();
	}

	public void OnVoiceRecognitionMessage(string msg)
	{
		Debug.Log("VoiceRecognition: " + msg);
		string[] parts = msg.Split (VoiceRecognition.kListenerMessageDelminators);
		switch (parts [0]) {
		case VoiceRecognition.kListenerResult:
			Debug.Log ("Result: " + parts [1]);
                // this is a changing result as data gets changed based on context.
                // save it until we get a complete so we can use the final result
                speechResult = parts[1];
                if (outputText.text != null)
                    outputText.text = speechResult.ToLower();
                string[] words = speechResult.Split((char[])null);
                foreach (string word in words)
                {
                    if (word.ToLower().Equals(trigger) == true)
                        TriggerVideo();
                }
                break;
		case VoiceRecognition.kListenerError:
			Debug.Log ("Error: " + parts [1]);
			break;
		case VoiceRecognition.kListenerComplete:
			Debug.Log ("Complete: " + speechResult);
			if(outputText.text!= null)
                    outputText.text = speechResult.ToLower();
            string[] words2 = speechResult.Split((char[])null);
            foreach (string word in words2)
            {
                if (word.ToLower().Equals(trigger) == true)
                   TriggerVideo();
            }
                // kick it off again
            if (speechResult.ToLower ().Equals ("stop listening") == false) {
			    VoiceRecognition.StartListener ();
			}
			break;
	
		default:
			Debug.Log (msg);
			break;

		}
	}

    public void TriggerVideo()
    {
        if (outputText.text != null)
            outputText.text = "Video Triggered";
        mediaPlayer.Play();
    }

}
