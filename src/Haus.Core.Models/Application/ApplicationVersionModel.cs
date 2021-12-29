using System;

namespace Haus.Core.Models.Application;

public record ApplicationVersionModel(
    string Version,
    bool IsOfficialRelease,
    bool IsNewer,
    DateTimeOffset CreationDate,
    string Description
);