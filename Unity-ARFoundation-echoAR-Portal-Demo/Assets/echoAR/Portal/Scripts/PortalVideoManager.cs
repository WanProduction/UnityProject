using UnityEngine;
using UnityEngine.Video;

public class PortalVideoManager : MonoBehaviour
{
    public string videoURL = null;
    public Entry entry = null;
    public VideoPlayer videoPlayer;

    void Start() {
    }
    // Update is called once per frame
    void Update()
    {
        videoPlayer = gameObject.GetComponent<VideoPlayer>();

        if (videoURL != null && videoURL.Length > 0) 
        {
            // Create VideoPlayer, if it doesn't already exist
            if (videoPlayer == null) 
            {
                videoPlayer = gameObject.AddComponent<VideoPlayer>();
                videoPlayer.url = videoURL;
            }
        }       
        else {
            if (videoPlayer != null) 
            {
                Destroy(videoPlayer);
            }
        }
        string value = null;
        // Check if name "portalVideo" is removed from the video entry at any time
        if (entry != null && entry.getAdditionalData() != null && !entry.getAdditionalData().TryGetValue("name", out value)) {
            if (value == null) {
                videoURL = null;
                // if skyDome contains VideoPlayer, remove it
                if (videoPlayer != null) 
                {
                    Destroy(videoPlayer);
                }
            }
            else if (value == "portalVideo") 
            {
                // Create VideoPlayer, if it doesn't already exist
                if (videoPlayer == null && videoURL != null) 
                {
                    videoPlayer = gameObject.AddComponent<VideoPlayer>();
                    videoPlayer.url = videoURL;
                }                
            }
        }

    }
}
