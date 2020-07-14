using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Cqrs.Queries;
using Haus.Identity.Core.Common;
using Haus.Identity.Core.Users.Entities;
using Haus.Identity.Core.Users.Models;
using Haus.Models;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Core.Users.GetUsers
{
    public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, ListModel<HausUserModel>>
    {
        private readonly UserManager<HausUser> _userManager;
        private readonly IMapper _mapper;

        public GetUsersQueryHandler(UserManager<HausUser> userManager, IMapper mapper = null)
        {
            _userManager = userManager;
            _mapper = mapper ?? MapperFactory.CreateMapper();
        }

        public async Task<ListModel<HausUserModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken = default)
        {
            var query = _userManager.Users;
            if (request.HasSearchTerm)
                query = query.Where(u => u.UserName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase));
            
            return await query
                .ProjectTo<HausUserModel>(_mapper.ConfigurationProvider)
                .ToListModelAsync();
        }
    }
}