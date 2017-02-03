Although the plugin exists in its own directory, you will still need to create an AndroidManifest.xml file inside Assets/Plugins/Android.  This plugin requires the following permissions added to that file:

<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
<uses-permission android:name="android.permission.INTERNET"/>