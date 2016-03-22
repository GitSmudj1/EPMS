using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPMSAppDemo.Models
{
    //Model that was created to join the record model and work model as it wasn't working with the foreign keys created
    //Not the best way to do this but had to get it working
    public class PiecesOfWork
    {
        public int RecordId { get; set; }
        public IEnumerable<Work> Entries { get; set; }

    }
}