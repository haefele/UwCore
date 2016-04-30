﻿namespace UwCore.Application
{
    public abstract class ApplicationMode
    {
        public IApplication Application { get; set; }

        public abstract void Enter();
        public abstract void Leave();
    }
}