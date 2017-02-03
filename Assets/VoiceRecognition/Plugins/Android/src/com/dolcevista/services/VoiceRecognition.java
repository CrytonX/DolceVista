package com.dolcevista.services;

import java.util.ArrayList;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;


import android.app.Activity;
import android.util.Log;
import android.os.Handler;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.nuance.speechkit.Audio;
import com.nuance.speechkit.DetectionType;
import com.nuance.speechkit.Language;
import com.nuance.speechkit.Recognition;
import com.nuance.speechkit.ResultDeliveryType;
import com.nuance.speechkit.RecognitionType;
import com.nuance.speechkit.Session;
import com.nuance.speechkit.Transaction;
import com.nuance.speechkit.TransactionException;

import com.unity3d.player.UnityPlayer;

import android.net.Uri;


public class VoiceRecognition  {
    // Urgh.  These should be enum/ordinal values, but
    // Unity's SendMessage() only allows string arguments
    static final String kListenerStarted = "LISTENER_STARTED";
    static final String kListenerStopped = "LISTENER_STOPPED";
    static final String kListenerComplete = "LISTENER_COMPLETE";
    static final String kListenerResult = "LISTENER_RESULT";
    static final String kListenerProcessing = "LISTENER_PROCESSING";
    static final String kListenerError = "ERROR";
    static final String kListenerMessageDeliminator = "|";


	public static final String TAG = "VoiceRecognition";



    private Session speechSession;
    private Transaction recoTransaction;
    private State state = State.UNAVAILABLE;


    private Activity mUnityActivity;
    private UnityPlayer mUnityPlayer;

	private static VoiceRecognition mInstance = null;

	private String mOnVoiceRecognitionMessageObject = "App";
	private String mOnVoiceRecognitionMessageMethod = "onVoiceRecognitionMessage";
    private String mAccessKey = "";
    private String mAccessHost = "";

	private Boolean mInitialized = false;
	private Boolean mUseProgressiveResults = true;
 
    public static VoiceRecognition getInstance() {
        if (mInstance == null) {
            mInstance = new VoiceRecognition();
        }

        return mInstance;
    }
	

    
    public void initialize(Activity holderActivity) {

        // hold on to the unity activity, we need it to attach our view
        mUnityActivity = holderActivity;
		
        // get the current view (assumed to be the unity view at [0]).
        ViewGroup view = (ViewGroup) mUnityActivity.getWindow().getDecorView();
		FrameLayout content = (FrameLayout) view.findViewById(android.R.id.content);
		mUnityPlayer = (UnityPlayer) content.getChildAt(0);


        state = State.IDLE;

		Log.d(TAG, "initialize() complete!");


	}

	public void setCallback(String callbackObject, String callbackMethod) {
        // hold on to the callback object and method
        mOnVoiceRecognitionMessageObject = callbackObject;
        mOnVoiceRecognitionMessageMethod = callbackMethod;
		Log.d(TAG, "setCallback() complete!");
	}

    public void setCredentials(String host, String accessKey) {
		this.mAccessHost = host; //"nmsps://NMDPTRIAL_rknopf_hotmail_com20161206220629@sslsandbox-nmdp.nuancemobility.net:443";
		this.mAccessKey = accessKey;//"1a79aa102e909afb64d517c8fc589204070c7e4600076f49815bcd762447653ec00f4eee9fe8e903bd01cbba595dbe60693ca66a4415b3c571803b95c7a0d387";
		
		Log.d(TAG, "setCredentials(" + host + ", " + accessKey + ") complete!");
    }



	private void sendMessageToUnity(String msg) {
	   	if (mOnVoiceRecognitionMessageObject == null) {
			return;
		}	
	   	if (mOnVoiceRecognitionMessageMethod == null) {
			return;
		}	

		final String strMessage = msg;
        mUnityActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
				UnityPlayer.UnitySendMessage(mOnVoiceRecognitionMessageObject, mOnVoiceRecognitionMessageMethod, strMessage);
            }
        });
	}

	public int startListener() {
	    if (state == State.LISTENING) {
				Log.i(TAG, "Listener is already started.");
				return -1;   
			} else {
				int errorCode = this.startListening();
				if (errorCode == 0) {
					Log.i(TAG, "Listener started.");
				} else {
					Log.i(TAG, "Listener failed to start.  Error = " + errorCode);
				}

				return errorCode;
			}    
	}
	
	public int stopListener() {
		if (state != State.IDLE ) {
			this.cancel();
			Log.i(TAG, "Listener stopped.");
			return 0;   
		} else {
			Log.i(TAG, "Listener is already stopped.");
			return -1;   
		}    
	}    
	
	
	private int startListening() {	

		if (!this.mInitialized) {
            //Create a session
            speechSession = Session.Factory.session(mUnityActivity.getApplicationContext(), Uri.parse(this.mAccessHost), this.mAccessKey);
            loadEarcons();
            setState(State.IDLE);

			this.mInitialized = true;
		}

	    if (state != State.IDLE) {
            return -1;
        }

        recognize();

		return 0;
	}


    /**
     * Start listening to the user and streaming their voice to the server.
     */
    private void recognize() {
        //Setup our Reco transaction options.
        Transaction.Options options = new Transaction.Options();
        options.setRecognitionType( RecognitionType.DICTATION );
		options.setDetection(DetectionType.Short);
        options.setLanguage(new Language( "eng-USA" ));
        //options.setEarcons(startEarcon, stopEarcon, errorEarcon, null);

        if(mUseProgressiveResults) {
            options.setResultDeliveryType(ResultDeliveryType.PROGRESSIVE);
        }

        //Start listening
        recoTransaction = speechSession.recognize(options, recoListener);
    }

    private Transaction.Listener recoListener = new Transaction.Listener() {
        @Override
        public void onStartedRecording(Transaction transaction) {
            //logs.append("\nonStartedRecording");

            //We have started recording the users voice.
            //We should update our state and start polling their volume.
            setState(State.LISTENING);
            sendMessageToUnity(kListenerStarted);
            startAudioLevelPoll();
        }

        @Override
        public void onFinishedRecording(Transaction transaction) {
            //logs.append("\nonFinishedRecording");

            //We have finished recording the users voice.
            //We should update our state and stop polling their volume.
            setState(State.PROCESSING);
            sendMessageToUnity(kListenerProcessing);
            stopAudioLevelPoll();
        }

        @Override
        public void onRecognition(Transaction transaction, Recognition recognition) {
            //logs.append("\nonRecognition: " + recognition.getText());

            //We have received a transcription of the users voice from the server.
            String result = recognition.getText();
            Log.d(TAG, "Got result: " + result);
            sendMessageToUnity(kListenerResult + kListenerMessageDeliminator + result);
        }

        @Override
        public void onSuccess(Transaction transaction, String s) {
            //logs.append("\nonSuccess");


            //Notification of a successful transaction.
            sendMessageToUnity(kListenerComplete + kListenerMessageDeliminator + s);
            setState(State.IDLE);
        }

        @Override
        public void onError(Transaction transaction, String s, TransactionException e) {
            //logs.append("\nonError: " + e.getMessage() + ". " + s);

            //Something went wrong. Check Configuration.java to ensure that your settings are correct.
            //The user could also be offline, so be sure to handle this case appropriately.
            //We will simply reset to the idle state.
            sendMessageToUnity(kListenerError + kListenerMessageDeliminator + e.getMessage() + ". " + s);
            setState(State.IDLE);
        }
    };

    /**
     * Stop recording the user
     */
    private void stopRecording() {
        recoTransaction.stopRecording();
    }

    /**
     * Cancel the Reco transaction.
     * This will only cancel if we have not received a response from the server yet.
     */
    private void cancel() {
        recoTransaction.cancel();
        sendMessageToUnity(kListenerStopped);
        setState(State.IDLE);
    }

    /* Audio Level Polling */

    private Handler handler = new Handler();

    /**
     * Every 50 milliseconds we should update the volume meter in our UI.
     */
    private Runnable audioPoller = new Runnable() {
        @Override
        public void run() {
            float level = recoTransaction.getAudioLevel();
            //volumeBar.setProgress((int)level);
            handler.postDelayed(audioPoller, 50);
        }
    };

    /**
     * Start polling the users audio level.
     */
    private void startAudioLevelPoll() {
        audioPoller.run();
    }

    /**
     * Stop polling the users audio level.
     */
    private void stopAudioLevelPoll() {
        handler.removeCallbacks(audioPoller);
        //volumeBar.setProgress(0);
    }


    /* State Logic: IDLE -> LISTENING -> PROCESSING -> repeat */

    private enum State {
        IDLE,
        LISTENING,
        PROCESSING,
        UNAVAILABLE
    }

    /**
     * Set the state and update the button text.
     */
    private void setState(State newState) {
        state = newState;
        switch (newState) {
            case IDLE:
                //toggleReco.setText(getResources().getString(R.string.recognize));
                break;
            case LISTENING:
                //toggleReco.setText(getResources().getString(R.string.listening));
                break;
            case PROCESSING:
                //toggleReco.setText(getResources().getString(R.string.processing));
                break;
        }
    }

    public int getState() {
        return state.ordinal();
    }

    /* Earcons */

    private void loadEarcons() {
        //Load all the earcons from disk
        //startEarcon = new Audio(this, R.raw.sk_start, Configuration.PCM_FORMAT);
        ////stopEarcon = new Audio(this, R.raw.sk_stop, Configuration.PCM_FORMAT);
        //errorEarcon = new Audio(this, R.raw.sk_error, Configuration.PCM_FORMAT);
    }

    /* Helpers */
/*
    private RecognitionType resourceIDToRecoType(int id) {
        if(id == R.id.dictation) {
            return RecognitionType.DICTATION;
        }
        if(id == R.id.search) {
            return RecognitionType.SEARCH;
        }
        if(id == R.id.tv) {
            return RecognitionType.TV;
        }
        return null;
    }

    private DetectionType resourceIDToDetectionType(int id) {
        if(id == R.id.long_endpoint) {
            return DetectionType.Long;
        }
        if(id == R.id.short_endpoint) {
            return DetectionType.Short;
        }
        if(id == R.id.none) {
            return DetectionType.None;
        }
        return null;
    }
*/
}
