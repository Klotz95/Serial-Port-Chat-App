using System.Threading;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
class CSerialPortCOM
{
    private SerialPort port = new SerialPort();
    private Thread getData;
    private string tmpData = "";
    private string status = "Nicht Verbunden!";
    private string modus = "none";
    bool handshake;
    public CSerialPortCOM(bool handshake)
    {
        getData = new Thread(GetData);
        port.BaudRate = 9600;
        port.DataBits = 8;
        port.Parity = Parity.None;
        port.StopBits = StopBits.One;
        this.handshake = handshake;
        //port.ReadTimeout = 500;
        //port.WriteTimeout = 500;
    }
    public bool Connect()
    {
        try
        {
            if (handshake) port.Handshake = Handshake.RequestToSend;

            port.Open();
            if (port.IsOpen)
            {
                getData.Start();
                status = "Verbunden!";
                if (handshake) modus = "Handshake";
                modus = "ohne Handshake";
                return true;
            }
            else
            {
                if (port.IsOpen) status = "Verbunden - Bereits verbunden!";
                status = "Nicht Verbunden - Fehler beim Verbinden!";
                return false;
            }
        }
        catch
        {
            status = "Nicht Verbunden! - Fehler beim Verbinden!";
            return false;
        }
    }
    public bool Disconnect()
    {
        try
        {
            port.Close();
            status = "Nicht Verbunden!";
            modus = "none";
            return true;
        }
        catch
        {
            if (port.IsOpen) status = "Verbunden! - Fehler beim Disconnect!";

            return false;
        }
    }

    private void GetData()
    {
        try
        {
            string line = "";
            while ((line = port.ReadLine()) != null)
            {
                tmpData = line;
            }
        }
        catch(Exception ex)
        {
            tmpData = "FEHLER beim Einlesen" + ex.Message;
            status = "Verbunden! - FEHLER beim Einlesen!";
        }
    }
    public string GetString()
    {
        return tmpData;
    }
    public bool SendString(string message)
    {
        try
        {
            port.WriteLine(message);
            return true;
        }
        catch(Exception ex)
        {
            status = ex.Message;
            return false;
        }
    }
    public string GetStatus()
    {
        return status;
    }
}
