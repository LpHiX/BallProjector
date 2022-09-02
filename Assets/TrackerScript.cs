using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class TrackerScript : MonoBehaviour
{
    UdpClient udp;
    Thread thread;
    string returnData = "";

    // Start is called before the first frame update
    void Start()
    {
        thread = new Thread(new ThreadStart(UDPThreadMethod));
        thread.Start();
    }
    private void OnDestroy()
    {
        if (udp != null)
        {
            udp.Close();
            udp = null;
        }
        if (thread != null)
        {
            if (thread.Join(100))
                Debug.Log("UDP thread has terminated successfully");
            else
            {
                Debug.Log("UDP thread did not terminate within 100ms, forcefully aborting");
                thread.Abort();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(returnData);
        String[] substrings = returnData.Split();
        if(substrings.Length > 3)
        {
            transform.position = new Vector3(-float.Parse(substrings[0]) / 100, float.Parse(substrings[1]) / 100, float.Parse(substrings[2]) / 100);
        }
    }

    private void UDPThreadMethod()
    {
        udp = new UdpClient(54322);
        Byte[] recievedBytes = new byte[0];
        string recievedText = "";
        while(udp != null)
        {
            try
            {
                recievedText = "";
                IPEndPoint RemoteIpEndpoint = new IPEndPoint(IPAddress.Any, 0);
                recievedBytes = udp.Receive(ref RemoteIpEndpoint);
                recievedText = Encoding.UTF8.GetString(recievedBytes);
                returnData = recievedText;
            }
            catch (Exception err)
            {
                if (udp != null)
                    Debug.Log("UDP Client Socket Exception Error: " + err);
                else
                    Debug.Log("thread is about to terminate ...");
            }
        }
    }
}
