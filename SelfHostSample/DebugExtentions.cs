using System.Text;
using Microsoft.Owin;

namespace SelfHostSample
{
    public static class DebugExtentions
    {
        public static string DumpEnvironment(this IOwinContext context, string linesep)
        {
            var sb = new StringBuilder();
            foreach (var item in context.Environment)
            {
                sb.AppendFormat("{0}: {1}{2}", item.Key, item.Value == null ? "(null)" : item.Value.ToString(), linesep);
            }

            return sb.ToString();
        }
    }
}
