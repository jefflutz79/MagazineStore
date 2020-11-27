using System;
using System.Collections.Generic;
using System.Text;

namespace MagazineStore.Models
{
  class ApiResponse
  {
    public bool Success { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
    public dynamic Data { get; set; }
  }
}
