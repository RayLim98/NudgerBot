using System.Collections.Generic;
using EmptyBot.Classes.Models;

namespace EmptyBot.Classes
{
  public interface IJsonHandler 
  {
    // Desciprtion: Reads Json 
    // Input String url
    // @returns String json
    void SetJson(string url);
    // Description: Gets .NET OBJECT and converts it to a string
    // @Returns List<Qoute>
    string SerializeJson();
    // Description: Gets Json string and converts it to a .NET OBJECT
    // @Returns List<Qoute>
    List<Quote> DeserializeJson();
  }
}