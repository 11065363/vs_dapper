using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1.model
{
    class book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public virtual List<BookReview> Reviews { get; set; }
        public override string ToString()
        {
            return string.Format("[{0}]------《{1}》", Id, Name);
        }
    }
}
