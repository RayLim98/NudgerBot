using System.Collections.Generic;
using EmptyBot.Classes.Models;

namespace EmptyBot.Classes
{
  public interface IJobHandler
  {
    List<Job> GetJobsList();
    float GetTimeSpan();
  } 
}