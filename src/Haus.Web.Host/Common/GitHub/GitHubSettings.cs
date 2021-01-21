namespace Haus.Web.Host.Common.GitHub
{
    public class GitHubSettings
    {
        public string Username { get; set; }
        public string PersonalAccessToken { get; set; }
        public string RepositoryOwner { get; set; } = "bryceklinker";
        public string RepositoryName { get; set; } = "haus";
    }
}