using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProvider.Models
{
    class FileGenerationResponse
    {
        string FileUrl { get; set; }
        string Message { get; set; }

        bool IsSuccess { get; set; }
    }
}
