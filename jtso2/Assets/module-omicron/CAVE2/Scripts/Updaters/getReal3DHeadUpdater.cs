﻿using UnityEngine;
using System.Collections;

public class getReal3DHeadUpdater : MonoBehaviour
{

    // Offset to tracking data (ex. object pivot vs tracking marker center)
    [SerializeField]
    Vector3 posOffset;
    [SerializeField]
    Vector3 rotOffset;

    [SerializeField]
    bool useLateUpdate;

    // Use this for initialization
    void Start()
    {

    }
#if USING_GETREAL3D
    // Update is called once per frame
    void Update()
    {
        if (!useLateUpdate)
        {
            transform.localPosition = getReal3D.Input.head.position + posOffset;
            transform.localRotation = getReal3D.Input.head.rotation;
            transform.Rotate(rotOffset);
        }
    }

    void LateUpdate()
    {
        if (useLateUpdate)
        {
            transform.localPosition = getReal3D.Input.head.position + posOffset;
            transform.localRotation = getReal3D.Input.head.rotation;
            transform.Rotate(rotOffset);
        }
    }
#endif

    public void SetPositionOffset(object[] data)
    {
        float x = posOffset.x;
        float y = posOffset.y;
        float z = posOffset.z;

        float.TryParse((string)data[0], out x);
        float.TryParse((string)data[1], out y);
        float.TryParse((string)data[2], out z);

        posOffset = new Vector3(x, y, z);
    }

    public void SetRotationOffset(object[] data)
    {
        float x = posOffset.x;
        float y = posOffset.y;
        float z = posOffset.z;

        float.TryParse((string)data[0], out x);
        float.TryParse((string)data[1], out y);
        float.TryParse((string)data[2], out z);

        rotOffset = new Vector3(x, y, z);
    }

}
