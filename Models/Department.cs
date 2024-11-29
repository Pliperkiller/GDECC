namespace MigrationAPI.Models
{
    public class Department
    {
        public double id;
        public string department;

        
        public Department(int _id, string _dep)
        {
            id = _id;
            department = _dep;
        }
    }
}
