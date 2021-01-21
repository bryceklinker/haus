using System;

namespace Haus.Core
{
    public static class VersionExtensions
    {
        public static string ToSemanticVersion(this Version version)
        {
            return version.ToString(3);
        }
    }
}