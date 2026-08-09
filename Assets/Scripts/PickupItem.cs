using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private float pickupRange = 2.6f;

    public PickupBehaviour playerPickupBehaviour;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private GameObject pickUpText;


    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange, layerMask))
        {
            if (hit.transform.CompareTag("Item"))
            {
                pickUpText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    playerPickupBehaviour.DoPickup(hit.transform.gameObject.GetComponent<Item>());
                }
            }
        }
        else
        {
            pickUpText.SetActive(false);

        }
    }
}
