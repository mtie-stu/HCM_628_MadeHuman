using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.User_Task
{
    public enum StatusPartTime
    { 
        PartTime,
        PTCD,
        Banned
    }
    public class PartTime
    {
        [Key]
        public Guid Id { get; set; }

        public string PartTimeId { get; set; }
        public string Name { get; set; }
        public string CCCD { get; set; }
        public string PhoneNumber { get; set; }
        public StatusPartTime StatusPartTimes { get; set; }


        // ✅ FK tới Company
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Part_Time_Company Company { get; set; }

        public ICollection<PartTimeAssignment> Assignments { get; set; } = new List<PartTimeAssignment>();

    }
}
