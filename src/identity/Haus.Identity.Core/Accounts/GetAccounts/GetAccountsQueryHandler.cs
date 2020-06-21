using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Common;
using Haus.Identity.Core.Common.Messaging;
using Haus.Identity.Core.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Haus.Identity.Core.Accounts.GetAccounts
{
    public class GetAccountsQueryHandler : IQueryHandler<GetAccountsQuery, ListModel<HausUserModel>>
    {
        private readonly UserManager<HausUser> _userManager;
        private readonly IMapper _mapper;

        public GetAccountsQueryHandler(UserManager<HausUser> userManager, IMapper mapper = null)
        {
            _userManager = userManager;
            _mapper = mapper ?? MapperFactory.CreateMapper();
        }

        public async Task<ListModel<HausUserModel>> Handle(GetAccountsQuery request, CancellationToken cancellationToken = default)
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