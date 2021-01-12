using System.Threading.Tasks;
using Haus.Core.Devices.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Devices
{
    [Route("api/light-types")]
    public class LightTypesController : HausBusController
    {
        public LightTypesController(IHausBus hausBus) 
            : base(hausBus)
        {
        }

        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            return QueryAsync(new GetLightTypesQuery());
        }
    }
}