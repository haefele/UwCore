using System;
using System.Text;

namespace UwCore.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetFullMessage(this Exception self)
        {
            var result = new StringBuilder();

            do
            {
                result.AppendLine(self.Message);
            } while ((self = self.InnerException) != null);

            var basicMessage = result.ToString().TrimEnd(Environment.NewLine.ToCharArray());

            if (basicMessage.EndsWith(".") == false)
                basicMessage += ".";

            basicMessage = basicMessage.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            return basicMessage;
        }
    }
}