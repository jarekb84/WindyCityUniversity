using WindyCityUniversity.Models.BaseTypes;

namespace WindyCityUniversity.Models
{
    public class Enrollment : BaseEntity
    {
        public float GPA { get; set; }
        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}