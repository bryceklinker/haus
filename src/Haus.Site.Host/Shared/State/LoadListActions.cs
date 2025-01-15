using System;
using Haus.Core.Models.Common;

namespace Haus.Site.Host.Shared.State;

public record LoadListRequestAction<T>;

public record LoadListSuccessAction<T>(ListResult<T> Result);

public record LoadListFailedAction<T>(Exception Error);