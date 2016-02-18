//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPMSAppDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Work
    {
        public int Id { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateCompleted { get; set; }
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateDue { get; set; }
        public string WorkItem { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> HoursWorked { get; set; }
        public Nullable<decimal> HoursRemaining { get; set; }
        [DataType(DataType.Date)]
        public System.DateTime SubmittedDate { get; set; }
        public byte[] RowVersion { get; set; }
        public int Work_Record { get; set; }
        public int Work_Project { get; set; }
        public int Work_Category { get; set; }
        public Nullable<int> Late { get; set; }
        public DateTime tempTimeBegin { get; set; }
        public DateTime tempTimeEnd { get; set; }


        public virtual Category Category { get; set; }
        public virtual Project Project { get; set; }
        public virtual Record Record { get; set; }
    }
}
