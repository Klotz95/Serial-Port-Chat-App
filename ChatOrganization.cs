using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
class ChatOrganization
{
  //Attribute
  User currentUser;
  User LastOnline;
  Message[] allMessages;
  Thread RefreshThread;
  CSerialPortCom configured;
  //Konstruktor
  public ChatOrganization(string UserName, Image ProfilePic, string comment,CSerialPortCom configured)
  {
    this.configured = configured;
    currentUser = new User(UserName,comment,ProfilePic);
    RefreshThread = new Thread(refresh);
    RefreshThread.Start();
  }

  private Message[] receiveMessage()
  {
    return allMessages;
  }
  private void InitializeConnection()
  {
    //send information about the new user and other important aspects
    if(!configured.Connect())
    {
      //stop all running Process and inform the user about the problem
      Message Error = new Message()
    }
  }
  private void refresh()
  {
    while(true)
    {
      Thread.Sleep(1000);

    }
  }
}
