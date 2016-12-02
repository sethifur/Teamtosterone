using Scheddy.Models;


namespace Scheddy.ViewModels
{
    public class SectionScheduleType
    {

        public Section section { get; set; }
        public int scheduleType { get; set; }

        public SectionScheduleType()
        {
            section = new Section();
        }

    }
}