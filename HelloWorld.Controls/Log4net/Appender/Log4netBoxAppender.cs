using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util;
using System;

namespace HelloWorld.Controls.Log4net.Appender
{
    public class Log4netBoxAppender : AppenderSkeleton
    {
        public event Action<ColoredLoggingEventArgs>? ColoredLoggingEvent;

        private static readonly Level DEFAULT_LEVEL = Level.Debug;
        protected override void Append(LoggingEvent loggingEvent)
        {
            string? log;
            if (Layout is PatternLayout patternLayout)
            {
                log = patternLayout.Format(loggingEvent);

                if (loggingEvent.ExceptionObject != null)
                {
                    log += loggingEvent.ExceptionObject.ToString();
                }
            }
            else
            {
                log = loggingEvent.RenderedMessage;
            }
            if (log is not null)
            {
                Level level = loggingEvent.Level ?? DEFAULT_LEVEL;
                ColoredLevel? coloredLevel = null;
                if (_levelMapping.Lookup(loggingEvent.Level) is ColoredLevel colored)
                {
                    coloredLevel = colored;
                }
                var args = new ColoredLoggingEventArgs(level, coloredLevel?.ForeColor, coloredLevel?.BackColor, log);
                ColoredLoggingEvent?.Invoke(args);
            }
        }

        protected override bool RequiresLayout => true;

        private readonly LevelMapping _levelMapping = new LevelMapping();

        public void AddMapping(ColoredLevel mapping)
        {
            _levelMapping.Add(mapping);
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            _levelMapping.ActivateOptions();
        }

        public class ColoredLoggingEventArgs
        {
            public Level Level { get; set; }
            public string? ForeColor { get; set; }
            public string? BackColor { get; set; }
            public string Log { get; set; }

            public ColoredLoggingEventArgs(Level level, string? foreColor, string? backColor, string log)
            {
                Level = level;
                ForeColor = foreColor;
                BackColor = backColor;
                Log = log;
            }
        }

        public class ColoredLevel : LevelMappingEntry
        {
            public string? ForeColor { get; set; }
            public string? BackColor { get; set; }

            public ColoredLevel()
            {
            }
        }
    }
}