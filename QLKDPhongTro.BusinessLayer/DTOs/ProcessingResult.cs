using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKDPhongTro.BusinessLayer.DTOs
{
    public class ProcessingResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
