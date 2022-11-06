using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class OSILayerScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PCScript PCScript;
    RectTransform OSILayerRect;

    // states
    public bool isInOSISlot { get; set; } = false;
    public int slotIndex { get; set; } = -1; // useless without isInOSISlot; -1 means not in any slot
    bool isDragging = false;
    Vector3 lastPosition;

    private void Awake()
    {
        OSILayerRect = GetComponent<RectTransform>();
    }

    void Start()
    {
        // get pc script
        PCScript = transform.parent.parent.parent.gameObject.GetComponent<PCScript>();
    }

    void Update()
    {
        if (isDragging)
        {
            

            // get mouse position
            var currPosition = Input.mousePosition;
            var mouseWorldPos = Camera.main.ScreenToWorldPoint(currPosition);
            // convert mouse position in screen space to world space to local space
            currPosition = OSILayerRect.parent.worldToLocalMatrix * mouseWorldPos;

            // calculate change in x and y vector
            float dx = currPosition.x - lastPosition.x;
            float dy = currPosition.y - lastPosition.y;
            Vector2 moveVector = new Vector2(dx, dy);

            // move rect position
            OSILayerRect.anchoredPosition += moveVector;
            
            // update last position
            lastPosition = OSILayerRect.anchoredPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDragging)
        {
            // reset states
            slotIndex = -1;
            isDragging = true;
            lastPosition = OSILayerRect.anchoredPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            // get all distance from OSILayer to OSISlot
            List<(float Distance , RectTransform OSISlot)> DistanceByOSISlots = new();
            for (int i = 0; i < PCScript.OSISlots.transform.childCount; i++)
            {
                RectTransform osiSlot = PCScript.OSISlots.transform.GetChild(i).gameObject.GetComponent<RectTransform>();
                float osiSlotDistToOSILayer = Vector2.Distance(OSILayerRect.anchoredPosition, osiSlot.anchoredPosition);
                DistanceByOSISlots.Add( (osiSlotDistToOSILayer, osiSlot) );
            }



            // check 1: get nearest distance
            var minDistByOSISlot = DistanceByOSISlots.First();
            foreach (var distByOSISlot in DistanceByOSISlots)
            {
                if (distByOSISlot.Distance < minDistByOSISlot.Distance)
                    minDistByOSISlot = distByOSISlot;
            }

            // check 2: snap if less than snap radius threshold
            if (minDistByOSISlot.Distance < OSILayerRect.rect.size.x / 2)
            {
                // check 3: if there's already an existing OSI layer in the slot, replace it with new and put the existing back to the default slots
                int newOSISlotIndex = Convert.ToInt32(minDistByOSISlot.OSISlot.name[minDistByOSISlot.OSISlot.name.Length - 1] + "") - 1;
                for (int i = 0; i < PCScript.OSILayers.transform.childCount; i++)
                {
                    OSILayerScript osiSlot = PCScript.OSILayers.transform.GetChild(i).gameObject.GetComponent<OSILayerScript>();
                    if (osiSlot.isInOSISlot && osiSlot.slotIndex == newOSISlotIndex)
                    {
                        osiSlot.PutToDefaultSlot();
                        break;
                    }
                }

                PutToOSISlot(minDistByOSISlot.OSISlot);
            }
            else  // put it in DefaultSlots
                PutToDefaultSlot();
        }
    }

    public void PutToOSISlot(RectTransform osiSlot)
    {
        OSILayerRect.anchoredPosition = osiSlot.anchoredPosition;
        isInOSISlot = true;
        slotIndex = Convert.ToInt32(osiSlot.name[osiSlot.name.Length - 1] + "") - 1;
    }

    public void PutToDefaultSlot()
    {
        // get free slots in DefaultSlots
        HashSet<int> freeSlots = Enumerable.Range(0, 7).ToHashSet();
        for (int i = 0; i < PCScript.OSILayers.transform.childCount; i++)
        {
            OSILayerScript osiSlot = PCScript.OSILayers.transform.GetChild(i).gameObject.GetComponent<OSILayerScript>();
            int slotIndex = osiSlot.slotIndex;
            if (freeSlots.Contains(slotIndex) && !osiSlot.isInOSISlot)
                freeSlots.Remove(slotIndex);
        }

        // put it in first free default slot
        for (int i = 0; i < 7; i++)
        {
            if (freeSlots.Contains(i))
            {
                RectTransform defaultSlot = PCScript.DefaultSlots.transform.GetChild(i).GetComponent<RectTransform>();
                OSILayerRect.anchoredPosition = defaultSlot.anchoredPosition;
                isInOSISlot = false;
                slotIndex = i;
                return;
            }
        }
    }
}
