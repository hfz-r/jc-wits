using System.Security.Principal;

namespace ESD.JC_Main.LoginServices
{
    public class CustomIdentity : IIdentity
    {
        public string Name { get; private set; }
        public string Roles { get; private set; }
        public string Email { get; private set; }

        public CustomIdentity(string name, string roles, string email = "")
        {
            Name = name;
            Roles = roles;
            Email = email ?? string.Empty;
        }

        #region IIdentity Members
        public string AuthenticationType { get { return "Custom authentication"; } }

        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}
