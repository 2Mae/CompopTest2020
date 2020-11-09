namespace Compopulate
{

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class Compop : System.Attribute
    {
        //using string flags since only primitives can be used as attribute parameters.
        public string[] flags;
        public Compop()
        {
            flags = new string[0];
        }
        public Compop(params string[] flags)
        {
            this.flags = flags;
        }
    }

    public static class Flags
    {
        public const string allowNull = "allowNull";
    }
}
