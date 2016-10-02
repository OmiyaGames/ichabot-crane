using UnityEngine;
using System.Collections;

public class PlayerSync : MonoBehaviour
{
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = GetComponent<Rigidbody>().position;
            stream.Serialize(ref syncPosition);

            syncPosition = GetComponent<Rigidbody>().velocity;
            stream.Serialize(ref syncVelocity);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = GetComponent<Rigidbody>().position;
        }
    }

    void Awake()
    {
        lastSynchronizationTime = Time.time;
    }

    void Update()
    {
        if (GetComponent<NetworkView>().isMine)
        {
        }
        else
        {
            SyncedMovement();
        }
    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;

        GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }
}
