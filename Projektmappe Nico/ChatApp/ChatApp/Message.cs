using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
class Message
{
  //Attribute
  User ChatPartner;
  string[] ChatHistory;
  Image[] receivedImages;

  //Konstruktor
  public Message(User ChatPartner)
  {
    this.ChatPartner = ChatPartner;
  }
  public void AddMessage(string content)
  {
    //add to array
    string[] saved = ChatHistory;
    if(ChatHistory == null)
    {
      //make an object and fill it with content
      ChatHistory = new string[1];
      ChatHistory[0] = content;
    }
    else
    {
      for(int i = 0;i<saved.Length;i++)
      {
        ChatHistory[i] = saved[i];
      }
      ChatHistory[saved.Length] = content;
    }
  }
  public void AddPicture(Image content)
  {
    //add to array
    Image[] saved = receivedImages;
    if(receivedImages == null)
    {
      //make an object and fill it with content
      receivedImages = new Image[1];
      receivedImages[0] = content;
    }
    else
    {
      for(int i = 0;i<saved.Length;i++)
      {
        receivedImages[i] = saved[i];
      }
      receivedImages[saved.Length] = content;
    }
  }
  public string[] getMessages()
  {
    return ChatHistory;
  }
  public Image[] getImages()
  {
    return receivedImages;
  }
  public User getChatPartner()
  {
    return ChatPartner;
  }
}
