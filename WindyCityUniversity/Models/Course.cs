using System.Collections.Generic;
using WindyCityUniversity.Models.BaseTypes;

namespace WindyCityUniversity.Models
{
    // class named Course instead of Class to make it easier to use as a variable name
    // if class were used, names would need to be prefixed with @
    public class Course : BaseEntity
    {   
        // this string based code can potentially be updated, so it will not be made primary key
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}