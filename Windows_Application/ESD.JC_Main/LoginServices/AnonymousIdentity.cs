namespace ESD.JC_Main.LoginServices
{
    public class AnonymousIdentity : CustomIdentity
    {
        public AnonymousIdentity() 
            : base(string.Empty, string.Empty, string.Empty)
        { }
    }
}
