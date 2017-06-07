using UnityEngine;
using System;
using System.Collections;

public class SerialCommunicator : MonoBehaviour // can hinerit from DataStream
{
   /* public string Port = "COM4";
    public int BaudRate = 9600;

    private SerialPort SerialIOStream;

    public void Open()
    {
        SerialIOStream = new SerialPort(Port, BaudRate);
        SerialIOStream.Open();
    } 

    public string Read()
    {
        try
        {
            return SerialIOStream.ReadLine();
        }
        catch(Exception e)
        {
            return null;
        }
    }

    public void Write(string message)
    {
        // Send the request
        SerialIOStream.WriteLine(message);
        SerialIOStream.BaseStream.Flush();
    }

    public IEnumerator ReadAsync(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;

        do
        {
            // A single read attempt
            try
            {
                dataString = SerialIOStream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void Close()
    {
        SerialIOStream.Close();
    }*/
}
