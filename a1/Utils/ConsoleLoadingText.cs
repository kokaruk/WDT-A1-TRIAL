using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wdt.Utils
{
    /// <summary>
    /// heavily modified code from
    /// https://codereview.stackexchange.com/questions/152159/display-loading-text-with-spinner-in-console?rq=1
    /// </summary>
    public class ConsoleLoadingText
    {
        private const string DefaultLoadingText = "Loading...";
        private const int DefaultMillisecondsDelay = 250;
        private static readonly string[] _spinner = {"|", "/", "-", "\\"};
        private bool _continue = true;

        private int _i;

        /// <summary>
        /// Returns a task that, when running, continuously prints the loading text.
        /// </summary>
        public Task Display()
        {
            Console.Clear();
            return Task.Run(() =>
            {
                while (_continue)
                {
                    Console.Write($"\r{DefaultLoadingText} {_spinner[_i]}");
                    _i = (_i + 1) % _spinner.Length;
                    Thread.Sleep(DefaultMillisecondsDelay);
                }
            });
        }

        /// <summary>
        /// Stop this instance from displaying.
        /// </summary>
        public void Stop()
        {
            _continue = false;
        }
    }
}