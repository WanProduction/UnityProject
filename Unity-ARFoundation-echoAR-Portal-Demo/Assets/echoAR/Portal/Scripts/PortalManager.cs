using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// PortalManager determines the interaction between the shaders and main camera -> the main portal "effect".
/// </summary>

public class PortalManager : MonoBehaviour
{
    public Entry entry;
    public GameObject portalController;
    private Material[] skyDomeMaterials;
    private Material PortalPlaneMaterial;
    private Vector3 initialWorldSpacePosition;
    private Quaternion initialWorldSpaceRotation;
    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        portalController = GameObject.Find("portalController");
        skyDomeMaterials = portalController.GetComponent<PortalController>().skyDome.GetComponent<Renderer>().sharedMaterials;
        PortalPlaneMaterial = GetComponent<Renderer>().sharedMaterial;

        // Set initial transformation
        initialWorldSpacePosition = gameObject.transform.position;
        initialWorldSpaceRotation = gameObject.transform.rotation * Quaternion.AngleAxis(90, transform.worldToLocalMatrix * Camera.main.transform.right) * Quaternion.AngleAxis(-180, transform.worldToLocalMatrix * Camera.main.transform.forward);
        initialScale = gameObject.transform.localScale;
    }

    // PortalPlane shader behavior
    void OnTriggerStay(Collider collider)
    {
        Vector3 camPositionInPortalSpace = transform.InverseTransformPoint(Camera.main.transform.position);

        if (camPositionInPortalSpace.y <= 0.0f)
        {
            for (int i = 0; i < skyDomeMaterials.Length; ++i)
            {
                skyDomeMaterials[i].SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Front);
        }
        else if(camPositionInPortalSpace.y < 0.5f)
        {
            // Disable Stencil test
            for (int i = 0; i < skyDomeMaterials.Length; ++i)
            {
                skyDomeMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Always);
            }
            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Off);
        }
        else
        {
            // Enable stencil test
            for (int i = 0; i < skyDomeMaterials.Length; ++i)
            {
                skyDomeMaterials[i].SetInt("_StencilComp", (int)CompareFunction.Equal);
            }
            PortalPlaneMaterial.SetInt("_CullMode", (int)CullMode.Back);
        }
    }

    void Update()
    {
        string value = "";

        // Handle translation for portalPlane
        Vector3 positionOffset = Vector3.zero;
        if (entry.getAdditionalData().TryGetValue("portalPlaneX", out value))
        {
            positionOffset.x = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("portalPlaneY", out value))
        {
            positionOffset.y = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("portalPlaneZ", out value))
        {
            positionOffset.z = float.Parse(value, CultureInfo.InvariantCulture);
        }
        gameObject.transform.position = gameObject.transform.parent.position + positionOffset;

        // Handle rotation
        Quaternion targetQuaternion = initialWorldSpaceRotation;
        if (entry.getAdditionalData().TryGetValue("portalPlaneXAngle", out value))
        {
            targetQuaternion *= Quaternion.AngleAxis(float.Parse(value, CultureInfo.InvariantCulture), transform.worldToLocalMatrix *
                Camera.main.transform.right);
            gameObject.transform.rotation = targetQuaternion;
        }
        if (entry.getAdditionalData().TryGetValue("portalPlaneYAngle", out value))
        {
            targetQuaternion *= Quaternion.AngleAxis(float.Parse(value, CultureInfo.InvariantCulture), transform.worldToLocalMatrix *
                Camera.main.transform.up);
            gameObject.transform.rotation = targetQuaternion;
        }
        if (entry.getAdditionalData().TryGetValue("portalPlaneZAngle", out value))
        {
            targetQuaternion *= Quaternion.AngleAxis(float.Parse(value, CultureInfo.InvariantCulture), transform.worldToLocalMatrix *
                Camera.main.transform.forward);
            gameObject.transform.rotation = targetQuaternion;
        }

        // Handle Scale
        float scaleFactor = 1f;
        if (entry.getAdditionalData().TryGetValue("portalPlaneScale", out value))
        {
            scaleFactor = float.Parse(value, CultureInfo.InvariantCulture);
        }
        gameObject.transform.localScale = initialScale * scaleFactor;
    }
}
