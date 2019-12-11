using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FaceManager : MonoBehaviour
{
    public face[] faces;
    //public face ActiveFace;
    public int currentIndex = -2;
    private int maxIndex;
    private List<AugmentedFace> _tempAugmentedFaces = new List<AugmentedFace>();
    //public GameObject faceMask;
    bool m_IsQuitting;

    //    public ScanningAnimation scanningAnimation;

    //    // Update is called once per frame
    private void Start()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i].gameObject.SetActive(false);
        }
            maxIndex = faces.Length - 1;
        //SwitchFace();
    }
    public void SwitchFace()
    {
        currentIndex += 1;
        if (currentIndex > maxIndex)
            currentIndex = 0;

        ChangeFace();
    }
    private void ChangeFace()
    {
        for (int i = 0; i < faces.Length; i++)
        {
            //faces[i].gameObject.SetActive(false);
            faces[i].gameObject.SetActive(i == currentIndex ? true : false);

            //if (i == currentIndex)
            //{
            //    scanningAnimation.SetIcon(faces[i].faceIcon);
            //}
        }

        //ActiveFace.gameObject.SetActive(true);
    }

    public void Glass1()
    {
        currentIndex = 0;
        ChangeFace();
    }
    public void Glass2()
    {
        currentIndex = 1;
        ChangeFace();
    }
    public void Glass3()
    {
        currentIndex = 2;
        ChangeFace();
    }
    public void Glass4()
    {
        currentIndex = 3;
        ChangeFace();
    }
    void Update()
    {
        //        _UpdateApplicationLifecycle();
        Session.GetTrackables<AugmentedFace>(_tempAugmentedFaces, TrackableQueryFilter.All);

        if (_tempAugmentedFaces.Count == 0)
        {
            //faceMask.SetActive(false);
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            faces[currentIndex].gameObject.SetActive(false);
            //            scanningAnimation.MoveIcon(false);
        }
        else
        {
            //faceMask.SetActive(true);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            faces[currentIndex].gameObject.SetActive(true);
            //            scanningAnimation.MoveIcon(true);
        }
    }
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}