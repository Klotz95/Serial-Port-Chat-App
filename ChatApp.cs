using System.Threading;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

class ChatApp
{
  //Attribute
  CSerialPortCom port;
  string[] ChatHistrory;
  public ChatApp(CSerialPortCOM configured())
  {
        port = configured;
  }


}
