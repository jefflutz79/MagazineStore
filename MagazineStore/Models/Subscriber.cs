using System;
using System.Collections.Generic;
using System.Text;

namespace MagazineStore.Models
{
  class Subscriber {
    public string ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<int> MagazineIds { get; set; }
  }
}
