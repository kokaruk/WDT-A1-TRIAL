using System;

namespace wdt.controller
{
    internal abstract class Controller
    {
        internal virtual void Start()
        {
            Console.Clear();
        }
    }
}