namespace Noodle.App.Common;

public interface IJobStats
{
    string Status { get; set; }

    int Successful { get; set; }

    int Failed { get; set; }
}