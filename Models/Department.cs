using System.ComponentModel.DataAnnotations;
namespace RingoMediaTask.Models
{
    public class Department
    {
        public Department()
        {
            SubDepartments = new List<Department>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public byte[] Logo { get; set; }
        public int? ParentDepartmentId { get; set; }
        public Department ParentDepartment { get; set; }
        public List<Department> SubDepartments { get; set; }
    }
}
