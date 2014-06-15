namespace MakeSharp
{
    public class Make
    {
        internal static MakeSharpConfiguration config= new MakeSharpConfiguration();

        public static IConfigureMakeSharp Configure
        {
            get { return config; }
        }

        public static MakeSharpConfiguration ConfiguratorInstance
        {
            get { return config; }
        }
    }
}