﻿namespace Noodle.App.Common;

public interface IJobOptions
{
    public Uri Url { get; }

    public int Concurrency { get; }
}