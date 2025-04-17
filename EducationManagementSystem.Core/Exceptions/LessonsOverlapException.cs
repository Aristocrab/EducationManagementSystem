namespace EducationManagementSystem.Core.Exceptions;

public class LessonsOverlapException : Exception
{
    private LessonsOverlapException(string message) : base(message) {}

    public static void Throw(TimeOnly startTime, int differenceInMinutes)
    {
        if (differenceInMinutes == 0)
        {
            throw new LessonsOverlapException($"Lesson at this time ({startTime}) already exists");
        }
        
        var plusOrMinusSign = differenceInMinutes > 0 ? '+' : '-';
        var message = $"Lesson at this time {plusOrMinusSign} {Math.Abs(differenceInMinutes)} ({startTime}) already exists";
        throw new LessonsOverlapException(message);
    }
}