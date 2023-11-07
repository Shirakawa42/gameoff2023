using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalExit : MonoBehaviour
{
    private Portal portal;

    private void Start() {
        portal = GetComponentInParent<Portal>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PortalableObject>(out var portalableObject))
        {
            if (portalableObject.isClone)
            {
                Vector3 clonePos = portalableObject.transform.position;
                clonePos.z = 0;
                Vector3 localExitPoint = transform.InverseTransformPoint(clonePos);

                // clone is in the portal
                if (localExitPoint.x < 0)
                {
                    portalableObject.clone.SetActive(false);
                    portalableObject.original.transform.SetPositionAndRotation(portalableObject.clone.transform.position, portalableObject.clone.transform.rotation);
                    Destroy(portalableObject.clone);
                }
                // clone is outside the portal
                else
                {
                    portalableObject.clone.SetActive(false);
                    Destroy(portalableObject.clone);
                }
            }
        }
    }
}
