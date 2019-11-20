//using System;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Orbit.Models
//{
//    public abstract class ReportBase : IModel
//    {
//        protected ReportBase(DateTimeOffset reportDateTime)
//        {
//            this.ReportDateTime = reportDateTime;
//        }

//        [Column]
//        public virtual DateTimeOffset ReportDateTime { get; private set; }
        
//        [Column]
//        public abstract string ComponentName { get; protected set; }
        
//    }
//}