namespace MusicStore.Entities;

public class AppSettings
{
    public Storageconfiguration StorageConfiguration { get; set; } = default!;
    public Jwt Jwt { get; set; } = default!;
    public SmtpConfiguration? SmtpConfiguration { get; set; }
}

public class SmtpConfiguration
{
    public string Server { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public int PortNumber { get; set; }
    public bool EnableSsl { get; set; }
    public string FromName { get; set; } = default!;
}

public class Storageconfiguration
{
    public string Path { get; set; } = default!;
    public string PublicUrl { get; set; } = default!;
}

public class Jwt
{
    public string SecretKey { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Issuer { get; set; } = default!;
}