using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPMSAppDemo.Models
{
    public class PiecesOfWork
    {
        public int RecordId { get; set; }
        public IEnumerable<Work> Entries { get; set; }

    }
}