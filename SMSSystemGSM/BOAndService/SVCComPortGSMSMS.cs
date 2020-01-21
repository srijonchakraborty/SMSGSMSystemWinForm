/*
|-------------------------------------------------------------------------------|
|	This code was written by Srijon Chakraborty								    |
|	Main source code link on https://github.com/srijonchakro			        |
|	All my source codes are available on http://srijon.softallybd.com           |
|	C# GSM Serial Read Write	                                            |
|	LinkedIn https://bd.linkedin.com/in/srijon-chakraborty-0ab7aba7				|
|-------------------------------------------------------------------------------|
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading;


namespace SMSSystemGSM
{
    public class SVCComPortGSMSMS
    {
        #region cTor
        public SVCComPortGSMSMS()
        {
            _smsGSMPort = new SerialPort();
        } 
        #endregion

        #region Private Field
       
        private SerialPort _smsGSMPort = null;
        #endregion

        #region Properties
        public bool IsConnectedDeviceFound { get; set; } = false;
        public bool IsDeviceConnected { get; set; } = false;
        #endregion

        #region Public Methods
        public List<BOComPortGSMSMS> GetGSMSMSComportList()
        {
            List<BOComPortGSMSMS> BOComPortGSMSMS = new List<BOComPortGSMSMS>();
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Impersonation = ImpersonationLevel.Impersonate;
                options.EnablePrivileges = true;
                string connString = $@"\\{Environment.MachineName}\root\cimv2";
                ManagementScope scope = new ManagementScope(connString, options);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_POTSModem");
                ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection collection = search.Get();

                foreach (ManagementObject obj in collection)
                {
                    string portName = obj["AttachedTo"].ToString();
                    string portDescription = obj["Description"].ToString();

                    if (portName != "")
                    {
                        BOComPortGSMSMS com = new BOComPortGSMSMS();
                        com.ComPortName = portName;
                        com.ComPortDescription = portDescription;
                        BOComPortGSMSMS.Add(com);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Comport Not Found." + e.Message);
            }
            
            return BOComPortGSMSMS;
        }
        public bool ConnectComPortGSMSMS(BOComPortGSMSMS com)
        {
            if (_smsGSMPort == null || !IsDeviceConnected || !_smsGSMPort.IsOpen)
            {
                if (com != null)
                {
                    try
                    {
                        _smsGSMPort.PortName = com.ComPortName;
                        _smsGSMPort.BaudRate = 9600;
                        _smsGSMPort.Parity = Parity.None;
                        _smsGSMPort.DataBits = 8;
                        _smsGSMPort.StopBits = StopBits.One;
                        _smsGSMPort.Handshake = Handshake.RequestToSend;
                        _smsGSMPort.DtrEnable = true;    // Data-terminal-ready
                        _smsGSMPort.RtsEnable = true;    // Request-to-send
                        _smsGSMPort.NewLine = Environment.NewLine;
                        _smsGSMPort.Open();
                        IsDeviceConnected = true;
                    }
                    catch (Exception e)
                    {
                        IsDeviceConnected = false;
                        throw new Exception("Comport Connection Failed." + e.Message);
                    }
                }
                else
                {
                    IsDeviceConnected = false;
                }
            }
            return IsDeviceConnected;
        }

        public void DisconnectComPortGSMSMS()
        {
            try
            {
                if (_smsGSMPort != null || IsDeviceConnected || _smsGSMPort.IsOpen)
                {
                    _smsGSMPort.Close();
                    _smsGSMPort.Dispose();
                    IsDeviceConnected = false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Comport Disconnect Failed." + e.Message);
            }
        }

        public string ReadComPortGSMSMS(out bool isReadComPortSuccess)
        {
            string response = "";
            isReadComPortSuccess = false;
            try
            {
                if (_smsGSMPort == null || ! _smsGSMPort.IsOpen)
                {
                    return "Error:Please Connect";
                }
                _smsGSMPort.WriteLine("AT+CMGF=1"); // Set mode to Text(1) or PDU(0)
                Thread.Sleep(500); // Give half second to write
                _smsGSMPort.WriteLine("AT+CPMS=\"SM\""); // Set storage to SIM(SM)
                Thread.Sleep(500);
                _smsGSMPort.WriteLine("AT+CMGL=\"ALL\""); // What category to read ALL, REC READ, or REC UNREAD
                Thread.Sleep(500);
                _smsGSMPort.Write("\r");
                Thread.Sleep(500);

                response = _smsGSMPort.ReadExisting();

                if (response.EndsWith("\r\nOK\r\n"))
                {
                    isReadComPortSuccess = true;
                }
                else
                {
                    isReadComPortSuccess = false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Read Failed." + e.Message);
            }
            return response;
        }

        public string SendComPortGSMSMS(string toAdress, string message, out bool isWriteComPortSuccess)
        {
            string response = "";
            isWriteComPortSuccess = false;

            try
            {
                _smsGSMPort.WriteLine("AT+CMGF=1"); // Set mode to Text(1) or PDU(0)
                Thread.Sleep(1000);
                _smsGSMPort.WriteLine($"AT+CMGS=\"{toAdress}\"");
                Thread.Sleep(1000);
                _smsGSMPort.WriteLine(message + char.ConvertFromUtf32(26));
                Thread.Sleep(5000);

                response = _smsGSMPort.ReadExisting();

                if (response.EndsWith("\r\nOK\r\n") && response.Contains("+CMGS:")) // IF CMGS IS MISSING IT MEANS THE MESSAGE WAS NOT SENT!
                {
                    isWriteComPortSuccess = true;
                }
                else
                {
                    isWriteComPortSuccess = false;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Send Failed." + e.Message);
            }
            return response;
        }
        #endregion
    }
}
