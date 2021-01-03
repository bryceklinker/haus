using System.Threading.Tasks;
using Haus.Core.Lighting.Commands;
using Haus.Core.Lighting.Queries;
using Haus.Core.Models.Lighting;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Lighting
{
    [Route("api/lighting-constraints")]
    public class LightingConstraintsController : HausBusController
    {
        public LightingConstraintsController(IHausBus hausBus) 
            : base(hausBus)
        {
        }

        [HttpGet("defaults")]
        public Task<IActionResult> GetDefaults()
        {
            return QueryAsync(new GetDefaultLightingConstraintsQuery());
        }

        [HttpPut("defaults")]
        public Task<IActionResult> UpdateDefaults([FromBody] LightingConstraintsModel model)
        {
            return CommandAsync(new UpdateDefaultLightingConstraintsCommand(model));
        }
    }
}