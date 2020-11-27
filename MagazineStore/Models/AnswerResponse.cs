using System;
using System.Collections.Generic;
using System.Text;

namespace MagazineStore.Models
{
  class AnswerResponse
  {
    public string TotalTime { get; set; }
    public bool AnswerCorrect { get; set; }
    public List<string> ShouldBe { get; set; }
  }
}
