using E_TutorApp.Domain.Entities.Abstracts;
using E_TutorApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_TutorApp.Domain.Entities.Concretes
{
    public class DetailStudent : BaseEntity
    {
        // Foreign key
        public string StudentId { get; set; }

        //   Update zamani elave olunmalilar. !!!     21.08.2024 Meet qerari
        //public List<string>  CardCoursesId { get; set; }
        //public List<string > WistListCourseId { get; set; }

        // Nav Props
        [ForeignKey(nameof(StudentId))]
        public virtual User Student { get; set; }
        public virtual IEnumerable<CourseBasicInfos>? EnrolledCoursesOfStudent { get; set; }
    }
}
