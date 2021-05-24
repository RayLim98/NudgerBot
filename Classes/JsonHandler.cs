using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EmptyBot.Classes.Models;

namespace EmptyBot.Classes
{
  public class JsonHandler : IJsonHandler
  {

    public string _json { get; private set; }
    public JsonHandler()
    {
    }
    public JsonHandler( string url ) {
      this._json = File.ReadAllText(url);
    }
    public string SerializeJson() => JsonSerializer.Serialize(_json);
    public List<Quote> DeserializeJson() => JsonSerializer.Deserialize<List<Quote>>(this._json);

    public void SetJson( string url ) {
      if( url != null ) {
        this._json = File.ReadAllText(url);
      }
    } 
  }
}