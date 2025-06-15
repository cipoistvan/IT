using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace ITAssets
{

    public class ITLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new ITLogger(categoryName);
        }
        public void Dispose()
        {
        }
    }
    public class ITLogger: ILogger
    {
        private readonly string _categoryName;
        
        public ITLogger(string categoryName)
        {
            _categoryName = categoryName;
        
        }

        public IDisposable BeginScope<TState>(TState state) => null!;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = $"{DateTime.Now.ToString("yy-MM-dd HH:mm:ss")} - [{logLevel}] - {_categoryName}: {formatter(state, exception)}";
            if (exception != null)
                message += $"\n{exception}";

            if(App.MainVM is not null)
                App.MainVM.LogText += "\n" + message;

            //MessageBox.Show(message, "Log üzenet", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    

    public class ITFileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public ITFileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger(string categoryName)
            => new ITFileLogger(_filePath, categoryName);

        public void Dispose() { }
    }

    public class ITFileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly string _category;

        public ITFileLogger(string filePath, string category)
        {
            _filePath = filePath;
            _category = category;
        }

        public IDisposable? BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{_category}] {logLevel}: {formatter(state, exception)}";
            try
            {
                File.AppendAllText(_filePath, message + Environment.NewLine);
            }
            catch
            {
            }
        }
    }



}
