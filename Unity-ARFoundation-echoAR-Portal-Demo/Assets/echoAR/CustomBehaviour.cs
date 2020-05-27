/**************************************************************************
* Copyright (C) echoAR, Inc. 2018-2020.                                   *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echoar.xyz/terms, or another agreement                      *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;
    public GameObject portalController;

    public GameObject portalPlane;
    private bool portal;

    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    // Use this for initialization
    void Start()
    {
        // Add RemoteTransformations script to object and set its entry
        this.gameObject.AddComponent<RemoteTransformations>().entry = entry;
        portalController = GameObject.Find("portalController"); 

        // Query additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string value = "";

        // Query additional data to get the name
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            gameObject.name = value;

            // Check if object should be treated as a portal
            if (!portal && value == "portal" || value == "Portal")
            {
                gameObject.tag = "portal";
                portal = true;

                // Instantiate portalPlane, skyDome and assign entry
                portalPlane = Instantiate(portalController.GetComponent<PortalController>().portalPlane, gameObject.transform);
                portalPlane.GetComponent<PortalManager>().entry = entry;

                GameObject skyDome = Instantiate(portalController.GetComponent<PortalController>().skyDome, gameObject.transform);

                // Check if GameObject with portalVideo tag exists
                GameObject[] portalVideos = GameObject.FindGameObjectsWithTag("portalVideo");
                if (portalVideos.Length > 0)
                {
                    // Portal video exists independent of portal -> integrate it with portal's skydome
                    skyDome.GetComponent<PortalVideoManager>().videoURL = portalVideos[0].GetComponent<VideoPlayer>().url;
                    skyDome.GetComponent<PortalVideoManager>().entry = portalVideos[0].GetComponent<CustomBehaviour>().entry;
                    // Destroy the independent portal video, now that it's integrated
                    Destroy(portalVideos[0]);
                }
            }
            // Check if video should be treated as a portal video
            if (value == "portalVideo" || value == "PortalVideo")
            {
                gameObject.tag = "portalVideo";
                GameObject[] portals = GameObject.FindGameObjectsWithTag("portal");
                // Check if GameObject with portal tag exists (videoPlane)
                if (portals.Length > 0)
                {
                    // Find portal's skyDome
                    GameObject skyDome = FindGameObjectInChildWithTag(portals[0], "skyDome");
                    // Pass video url to PortalVideoManager
                    skyDome.GetComponent<PortalVideoManager>().videoURL = gameObject.GetComponent<VideoPlayer>().url;
                    skyDome.GetComponent<PortalVideoManager>().entry = entry;

                    // Destroy videoPlane (self)
                    Destroy(gameObject);
                }
            }
        }
    }

    // Returns a Child GameObject by tag
    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }
        }
        return null;
    }

}