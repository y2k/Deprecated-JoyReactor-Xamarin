using System;

namespace JoyReactor.Core.Model
{
    public abstract class MessageService
    {
        public static MessageService Instance { get; set; }

        public abstract void Show(string message);
    }
}