namespace LLTC.Utils;

public sealed class TimeHandle
{
    private bool isPaused;
    private int timePassed = 0;

    public int TimePassed { get { return timePassed; } }

    public TimeHandle(int timePassed = 0)
    {
        this.timePassed = timePassed;
        isPaused = true;
    }

    public void SwitchState() => isPaused = !isPaused;

    public void Pass(int sec)
    {
        if (!isPaused)
            timePassed += sec;
    }

    public void CorrectTimePassed(int coefficient) 
    {
        if (timePassed != 0)
            timePassed = (int)((double)timePassed / 100 * coefficient);
    }

    public string Info() => $"{ProcessSeconds(timePassed)}" + 
        $"{(isPaused ? "LLTC is paused, waiting for resume..." : "LLTC is running, press 'space' to pause.")}";

    private string ProcessSeconds(int sec)
    {
        if (sec == 0)
            return "";

        int hours = sec / 3600;
        int minutes = sec % 3600 / 60;
        int seconds = sec % 60;

        return $"{(hours <= 0 ? string.Empty : $"{hours}h ")}" +
            $"{(minutes <= 0 ? string.Empty : $"{minutes}m ")}" +
            $"{(seconds <= 0 ? string.Empty : $"{seconds}s")} passed!\n";
    }
}