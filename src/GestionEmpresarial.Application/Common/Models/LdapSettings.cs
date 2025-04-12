namespace GestionEmpresarial.Application.Common.Models
{
    public class LdapSettings
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string BindDN { get; set; } = string.Empty;
        public string BindPassword { get; set; } = string.Empty;
        public string SearchBase { get; set; } = string.Empty;
        public string SearchFilter { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string DefaultRole { get; set; } = string.Empty;
        public string EmailAttribute { get; set; } = string.Empty;
        public string DisplayNameAttribute { get; set; } = string.Empty;
        public string UsernameSuffix { get; set; } = string.Empty;
    }
}
