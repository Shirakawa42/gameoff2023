using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalColor
{
    Blue,
    Orange
}

public class Portal : MonoBehaviour
{
    public Collider2D wall;
    public PortalColor portalColor;

    public string inPortalLayerString;
    public string inOtherPortalLayerString;
    public GameObject otherPortal;

    private void Start()
    {
        inPortalLayerString = "inBluePortal";
        inOtherPortalLayerString = "inOrangePortal";
        otherPortal = Globals.portalManager.orangePortal;
        if (portalColor == PortalColor.Orange)
        {
            inPortalLayerString = "inOrangePortal";
            inOtherPortalLayerString = "inBluePortal";
            otherPortal = Globals.portalManager.bluePortal;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PortalableObject portalableObject = other.GetComponent<PortalableObject>();
        if (portalableObject != null && portalableObject.isClone == false && portalableObject.clone == null)
        {
            other.gameObject.layer = LayerMask.NameToLayer(inPortalLayerString);

            portalableObject.original = other.gameObject;
            portalableObject.clone = Instantiate(other.gameObject, new Vector3(1000, 1000, 0), other.transform.rotation);
            portalableObject.clone.SetActive(false);
            portalableObject.clone.GetComponent<Rigidbody2D>().gravityScale = 0;
            portalableObject.clone.GetComponent<PortalableObject>().Init(true, other.gameObject, portalableObject.clone, other.GetComponent<Rigidbody2D>());
            portalableObject.clone.layer = LayerMask.NameToLayer(inOtherPortalLayerString);
            portalableObject.connectedRigidbody = portalableObject.clone.GetComponent<Rigidbody2D>();

            Vector3 clonePosition = transform.parent.InverseTransformPoint(other.transform.position);
            Debug.Log(clonePosition);
            clonePosition.y = -clonePosition.y;
            clonePosition = otherPortal.transform.position - clonePosition;
            portalableObject.clone.transform.position = clonePosition;
            portalableObject.clone.SetActive(true);
        }
    }
}
