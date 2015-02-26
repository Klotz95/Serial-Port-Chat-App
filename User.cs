using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
class User
{
  //Attribute
  string Name;
  string comment;
  Image ProfilePic;

  public User(string Name, string comment, Image ProfilePic)
  {
    this.Name = Name;
    this.comment = comment;
    this.ProfilePic = ProfilePic;
  }
  public Iamge getImage()
  {
    return ProfilePic;
  }
  public string getName()
  {
    return Name;
  }
  public string getComment()
  {
    return comment;
  }
}
