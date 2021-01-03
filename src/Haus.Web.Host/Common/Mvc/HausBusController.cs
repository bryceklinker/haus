using System;
using System.Threading.Tasks;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Queries;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using Haus.Cqrs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Common.Mvc
{
    [Authorize]
    [ApiController]
    public abstract class HausBusController : Controller
    {
        private readonly IHausBus _hausBus;

        protected HausBusController(IHausBus hausBus)
        {
            _hausBus = hausBus;
        }

        protected async Task<IActionResult> QueryAsync<TResult>(IQuery<TResult> query)
        {
            var result = await _hausBus.ExecuteQueryAsync(query).ConfigureAwait(false);
            return OkOrNotFound(result);
        }

        protected async Task<IActionResult> CommandAsync(ICommand command)
        {
            try
            {
                await _hausBus.ExecuteCommandAsync(command).ConfigureAwait(false);
                return NoContent();
            }
            catch (HausValidationException e)
            {
                return BadRequest(e.Errors);
            }
        }

        protected async Task<IActionResult> CreateCommandAsync<TResult>(ICommand<TResult> command, string routeName, Func<TResult, object> createRouteValues)
        {
            try
            {
                var result = await _hausBus.ExecuteCommandAsync(command).ConfigureAwait(false);
                var routeValues = createRouteValues(result);
                return CreatedAtRoute(routeName, routeValues, result);
            }
            catch (HausValidationException e)
            {
                return BadRequest(e.Errors);
            }
        }
        
        protected IActionResult OkOrNotFound<T>(T model)
        {
            if (model == null)
                return NotFound();
            return Ok(model);
        }
    }
}