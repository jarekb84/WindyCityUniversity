using System;
using WindyCityUniversity.Models.BaseTypes;

namespace WindyCityUniversity.Models
{
    public class Enrollment : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }

        public float GPA { get; set; }
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}