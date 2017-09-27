using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressButtonDemo
{
   public class ExtendPropertyChangingEventArgs: PropertyChangingEventArgs
    {
        public bool Cancel { get; set; }

        public ExtendPropertyChangingEventArgs(string propertyName) : base(propertyName)
        {
        }
    }
}
