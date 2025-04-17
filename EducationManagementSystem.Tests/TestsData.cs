namespace EducationManagementSystem.Tests;

public static class TestsData
{
    public class OverlappingTimesTheoryData : TheoryData<string, string, string>
    {
        public OverlappingTimesTheoryData()
        {
            // Existing lesson start time, new lesson start time, expected error message
            Add("10:00", "9:30", "Lesson at this time + 30 (10:00) already exists");
            Add("10:00", "10:00", "Lesson at this time (10:00) already exists");
            Add("10:00", "10:30", "Lesson at this time - 30 (10:00) already exists");
            Add("11:20", "11:55", "Lesson at this time - 35 (11:20) already exists");
            Add("11:55", "11:20", "Lesson at this time + 35 (11:55) already exists");
        }
    }
    
    public class OverlappingTimesWithDurationsTheoryData : TheoryData<string, int, string, int>
    {
        public OverlappingTimesWithDurationsTheoryData()
        {
            // Existing lesson start time, new lesson duration, new lesson start time, new lesson duration
            Add("10:00", 90, "11:00", 60);
            Add("10:00", 60, "9:00", 90);
            Add("09:00", 120, "10:30", 60);
            Add("14:00", 45, "14:30", 30);
            Add("13:00", 90, "12:00", 120);
            Add("08:00", 60, "08:30", 45);
            Add("15:00", 75, "14:45", 60);
            Add("11:00", 120, "12:00", 90);
            Add("17:00", 30, "16:30", 60);
            Add("09:30", 60, "09:00", 45);
        }
    }
    
    public class NonOverlappingTimesTheoryData : TheoryData<string, string>
    {
        public NonOverlappingTimesTheoryData()
        {
            // Existing lesson start time, new lesson start time
            Add("10:00", "11:00");
            Add("10:00", "9:00");
            Add("7:00", "9:00");
            Add("9:00", "7:00");
            Add("7:00", "19:00");
            Add("19:00", "7:00");
        }
    }
    
    public class NonOverlappingTimesWithDurationsTheoryData : TheoryData<string, int, string, int>
    {
        public NonOverlappingTimesWithDurationsTheoryData()
        {
            // Existing lesson start time, new lesson duration, new lesson start time, new lesson duration
            Add("10:00", 90, "11:30", 60);
            Add("10:00", 60, "9:30", 30);
            Add("9:00", 120, "11:00", 60);
            Add("12:00", 60, "10:00", 120);
            Add("14:00", 45, "15:00", 30);
            Add("14:00", 45, "15:45", 30);
        }
    }
}