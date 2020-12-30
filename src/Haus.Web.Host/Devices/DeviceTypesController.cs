using System.Threading.Tasks;
using Haus.Core.Devices.Queries;
using Haus.Cqrs;
using Haus.Web.Host.Common.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace Haus.Web.Host.Devices
{
    [Route("api/device-types")]
    public class DeviceTypesController : HausBusController
    {
        public DeviceTypesController(IHausBus hausBus) : base(hausBus)
        {
        }

        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            return QueryAsync(new GetDeviceTypesQuery());
        } 
    }
}