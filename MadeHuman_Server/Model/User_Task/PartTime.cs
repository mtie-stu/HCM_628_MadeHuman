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
        public Guid PartTimeId { get; set; } = default!;

        public string Name { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public StatusPartTime StatusPartTimes { get; set; }


        // ✅ FK tới Company
        public Guid CompanyId { get; set; } = default!;

        [ForeignKey(nameof(CompanyId))]
        public Part_Time_Company Company { get; set; }

        public ICollection<PartTimeAssignment> Assignments { get; set; } = new List<PartTimeAssignment>();

    }
}
