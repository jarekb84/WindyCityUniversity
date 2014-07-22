using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WindyCityUniversity.Models.BaseTypes;

namespace WindyCityUniversity.Models
{
    public class Student : BaseEntity
    {
        // This is a reference field to the id field on the student data file. 
        // Did not use studentID as the name to prevent confusion with the actual PK
        // This is not the primary key since there were duplicates in the data file
        // Leaving this reference to the origioanl ID in the data load incase an outside system 
        // needs to reference student information in the future using that interger
        public int ExternalId { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstName;
            }
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}