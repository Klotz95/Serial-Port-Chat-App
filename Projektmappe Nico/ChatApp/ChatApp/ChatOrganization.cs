using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace ChatApp
{
class ChatOrganization
{
  //Attribute
  User currentUser;
  User LastOnline;
  Message[] allMessages;
  bool waitAnswer;
  bool sendLock;
  Image oderableImage;
  string orderbalemessage;
  Thread RefreshContent;
  Thread waitForUser;
  CSerialPortCOM configured;
  //Konstruktor
  public ChatOrganization(string UserName, Image ProfilePic,string Comment, CSerialPortCOM current, ref bool check)
  {
    configured = current;
    //create the current User
    currentUser = new User(UserName,Comment, ProfilePic);
    waitAnswer = false;
    sendLock = false;
    //initialize the connection
    if (configured.Connect())
    {
        check = true;
        //start Thread
        RefreshContent = new Thread(WaitForMessage);
        RefreshContent.Start();
    }
    else
    {
        check = false;
    }

  }
  public Message[] receiveMessages()
  {
    return allMessages;
  }
  public bool sendMessageWithContent(string content, Image pic)
  {
    if(waitAnswer != true && sendLock != true)
    {
    orderbalemessage = content;
    oderableImage = pic;
    //now create the content for transimission
    bool picture = true;
    BinaryFormatter formatter = new BinaryFormatter();
    MemoryStream ms = new MemoryStream();
    formatter.Serialize(ms,currentUser);
    byte[] cuUser = ms.ToArray();
    ms.Close();
    byte[] trastring = ConvertMessageToAscii(content);
    ms = new MemoryStream();
    formatter.Serialize(ms,pic);
    byte[] serializedImage = ms.ToArray();
    //now create the controlbits
    byte UserLength = Convert.ToByte(cuUser.Length);
    byte trastringLength = Convert.ToByte(trastring.Length);
    byte ImageLenght = Convert.ToByte(serializedImage.Length);
    //now create the final array
    byte[] finishForSending = new byte[4 + UserLength + trastringLength + ImageLenght];
    finishForSending[0] = Convert.ToByte(picture);
    finishForSending[1] = UserLength;
    finishForSending[2] = trastringLength;
    finishForSending[3] = ImageLenght;
    for(int i = 0; i<UserLength;i++)
    {
      finishForSending[i + 4] = cuUser[i];
    }
    for(int i = 0; i<trastringLength;i++)
    {
      finishForSending[i+4+UserLength] = trastring[i];
    }
    for(int  i = 0; i<ImageLenght;i++)
    {
      finishForSending[i + 4 + UserLength + trastringLength] = serializedImage[i];
    }
    //now send
    if(configured.SendData(finishForSending))
    {
      //start a thread which waits for the UserAnswer
      if(waitForUser != null)
      {
        waitForUser.Interrupt();
      }
      if(RefreshContent != null)
      {
        RefreshContent.Interrupt();
      }
      waitForUser = new Thread(waitForAnswer);
      waitAnswer = true;
      waitForUser.Start();
      return true;
    }
    return false;
  }
  else
  {
    return false;
  }
  }
  public bool sendMessage(string content)
  {
    if(waitAnswer != true && sendLock != true)
    {
    orderbalemessage = content;
    //now create the content for transimission
    bool pic = false;
    BinaryFormatter formatter = new BinaryFormatter();
    MemoryStream ms = new MemoryStream();
    formatter.Serialize(ms,currentUser);
    byte[] cuUser = ms.ToArray();
    ms.Close();
    byte[] trastring = ConvertMessageToAscii(content);
    //now create the controlbits
    byte UserLength = Convert.ToByte(cuUser.Length);
    byte trastringLength = Convert.ToByte(trastring.Length);
    //now create the final array
    byte[] finishForSending = new byte[3 + UserLength + trastringLength];
    finishForSending[0] = Convert.ToByte(pic);
    finishForSending[1] = UserLength;
    finishForSending[2] = trastringLength;
    for(int i = 0; i<UserLength;i++)
    {
      finishForSending[i + 3] = cuUser[i];
    }
    for(int i = 0; i<trastringLength;i++)
    {
      finishForSending[i+3+UserLength] = trastring[i];
    }
    //now send
    if(configured.SendData(finishForSending))
    {
      //start a thread which waits for the UserAnswer
      if(waitForUser != null)
      {
        waitForUser.Interrupt();
      }
      if(RefreshContent != null)
      {
        RefreshContent.Interrupt();
      }
      waitForUser = new Thread(waitForAnswer);
      waitAnswer = true;
      waitForUser.Start();
      return true;
    }
    return false;
  }
  else
  {
    return false;
  }
}
  private void waitForAnswer()
  {
    while(waitAnswer)
    {
      Thread.Sleep(100);
      byte[] receivedPackage = new byte[0];
      if(configured.receiveData(ref receivedPackage))
      {
        if(receivedPackage.Length != 0)
        {
          //try to desirealize
          MemoryStream ms = new MemoryStream();
          ms.Write(receivedPackage,0,receivedPackage.Length);
          BinaryFormatter formatter = new BinaryFormatter();
          ms.Seek(0,SeekOrigin.Begin);
          try
          {
            LastOnline = (User)formatter.Deserialize(ms);
            waitAnswer = false;
          }
          catch
          {
            waitAnswer = true;
          }
        }
      }

    }
    //now look if there is already a Message Storage for that User
    bool found = false;
    for(int i = 0; i< allMessages.Length;i++)
    {
      if(found != true)
      {
      Message currentMessage = allMessages[i];
      User messageUser = currentMessage.getChatPartner();
      if(messageUser.getName() == LastOnline.getName())
      {
        found = true;
        allMessages[i].AddMessage("{A}"+ orderbalemessage);
        if(oderableImage != null)
        {
          allMessages[i].AddPicture(oderableImage);
        }
        oderableImage = null;
        orderbalemessage = null;
      }
      }
    }
    if(found != true)
    {
      //create new Message
      Message newMessage = new Message(LastOnline);
      newMessage.AddMessage("{A}" + orderbalemessage);
      if(oderableImage != null)
      {
        newMessage.AddPicture(oderableImage);
      }
      oderableImage = null;
      orderbalemessage = null;
      //now save it in the array
      Message[] saved = allMessages;
      allMessages = new Message[saved.Length +1];
      for(int i = 0;i < saved.Length;i++)
      {
        allMessages[i] = saved[i];
      }
      allMessages[saved.Length] = newMessage;
    }
    //now start the receive Message Thread
    RefreshContent = new Thread(WaitForMessage);
    RefreshContent.Start();
  }
  private void WaitForMessage()
  {
    
    while(true)
    {
      byte[] receivedMessage = new byte[0];
      if(configured.receiveData(ref receivedMessage))
      {
        if(receivedMessage.Length != 0)
        {
          //now check if the message has content, how long the parts are and who has send the message
         
          if(Convert.ToBoolean(receivedMessage[0]))
          {
            byte UserLenght = receivedMessage[1];
            byte MessageLenght = receivedMessage[2];
            byte contentLenght = receivedMessage[3];

            //now make the User Object
            byte[] messageUser = new byte[UserLenght];
            for(int i = 4;i < UserLenght + 4;i++)
            {
              messageUser[i-4] = receivedMessage[i];
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(messageUser,0,messageUser.Length);
            ms.Seek(0,SeekOrigin.Begin);
            LastOnline = (User)formatter.Deserialize(ms);
            ms.Close();
            //now get the message string
            byte[] messageContentInByte = new byte[MessageLenght];
            for(int i = 4 + UserLenght;i< 4+UserLenght + MessageLenght;i++)
            {
              messageContentInByte[i - (4+UserLenght)] = receivedMessage[i];
            }
            //now create the string
            string messagecontent = ConvertAsciiToMessage(messageContentInByte);

            //now get the image
            byte[] ImageInByte = new byte[contentLenght];
            for(int i = 4+ UserLenght + MessageLenght;i < 4+MessageLenght +UserLenght+contentLenght;i++)
            {
               ImageInByte[i - (4+ UserLenght + MessageLenght)] = receivedMessage[i];
            }
            //now make the Image
            ms = new MemoryStream();
            ms.Write(ImageInByte,0,ImageInByte.Length);
            ms.Seek(0,SeekOrigin.Begin);
            Image receivedImage = (Image)formatter.Deserialize(ms);

            //now look if that user had already send a message
            bool alreadyInList = false;
            for(int i = 0;i< allMessages.Length;i++)
            {
              if(alreadyInList != true)
              {
              User check = allMessages[i].getChatPartner();
              if(check.getName() == LastOnline.getName())
              {
                allMessages[i].AddMessage("{B}" + messagecontent);
                allMessages[i].AddPicture(receivedImage);
                alreadyInList = true;
              }
            }
            }
            if(alreadyInList != true)
            {
              //create a new Message
              Message newMessage = new Message(LastOnline);
              newMessage.AddMessage("{B}" + messagecontent);
              newMessage.AddPicture(receivedImage);
              Message[] saveOldMessages = allMessages;
              allMessages = new Message[saveOldMessages.Length + 1];

              for(int i = 0;i < saveOldMessages.Length; i++)
              {
                allMessages[i] = saveOldMessages[i];
              }
              allMessages[saveOldMessages.Length] = newMessage;

            }


          }
          else
          {
            byte UserLenght = receivedMessage[1];
            byte MessageLenght = receivedMessage[2];

            //now make the User Object
            byte[] messageUser = new byte[UserLenght];
            for(int i = 3;i < UserLenght + 3;i++)
            {
              messageUser[i-3] = receivedMessage[i];
            }
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(messageUser,0,messageUser.Length);
            ms.Seek(0,SeekOrigin.Begin);
            LastOnline = (User)formatter.Deserialize(ms);
            ms.Close();
            //now get the message string
            byte[] messageContentInByte = new byte[MessageLenght];
            for(int i = 3 + UserLenght;i< 3+UserLenght + MessageLenght;i++)
            {
              messageContentInByte[i - (3+UserLenght)] = receivedMessage[i];
            }
            //now create the string
            string messagecontent = ConvertAsciiToMessage(messageContentInByte);

            //now look if that user had already send a message
            bool alreadyInList = false;
            for(int i = 0;i< allMessages.Length;i++)
            {
              if(alreadyInList != true)
              {
              User check = allMessages[i].getChatPartner();
              if(check.getName() == LastOnline.getName())
              {
                allMessages[i].AddMessage("{B}" + messagecontent);
                alreadyInList = true;
              }
            }
            }
            if(alreadyInList != true)
            {
              //create a new Message
              Message newMessage = new Message(LastOnline);
              newMessage.AddMessage("{B}" + messagecontent);
              Message[] saveOldMessages = allMessages;
              allMessages = new Message[saveOldMessages.Length + 1];

              for(int i = 0;i < saveOldMessages.Length; i++)
              {
                allMessages[i] = saveOldMessages[i];
              }
              allMessages[saveOldMessages.Length] = newMessage;

            }

          }
          //now send information about yourself
          sendLock = true;
          MemoryStream sm = new MemoryStream();
          BinaryFormatter formater = new BinaryFormatter();
          formater.Serialize(sm,currentUser);
          byte[] currentUserinByte = sm.ToArray();
          configured.SendData(currentUserinByte);
          Thread.Sleep(100);
          sendLock = false;
        }
      }
    }
  }
  private byte[] ConvertMessageToAscii(string content)
  {
    ASCIIEncoding enc = new ASCIIEncoding();
    return enc.GetBytes(content);
  }
  private string ConvertAsciiToMessage(byte[] content)
  {
    ASCIIEncoding enc = new ASCIIEncoding();
    return enc.GetString(content);
  }
}
}
