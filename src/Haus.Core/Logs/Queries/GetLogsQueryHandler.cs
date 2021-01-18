using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Logs.Factories;
using Haus.Core.Models;
using Haus.Core.Models.Common;
using Haus.Core.Models.Logs;
using Haus.Cqrs.Queries;

namespace Haus.Core.Logs.Queries
{
    public record GetLogsQuery(string LogsDirectory, GetLogsParameters Parameters = null)
        : IQuery<ListResult<LogEntryModel>>
    {
        public int PageSize => Parameters?.PageSize ?? GetLogsParameters.DefaultPageSize;
        public int PageNumber => Parameters?.PageNumber ?? GetLogsParameters.DefaultPageNumber;
        public int SkipCount => (PageNumber - 1) * PageSize;
        public string SearchTerm => Parameters?.SearchTerm;
    }

    internal class GetLogsQueryHandler : IQueryHandler<GetLogsQuery, ListResult<LogEntryModel>>
    {
        private readonly ILogEntryModelFactory _logEntryFactory;

        public GetLogsQueryHandler(ILogEntryModelFactory logEntryFactory)
        {
            _logEntryFactory = logEntryFactory;
        }

        public async Task<ListResult<LogEntryModel>> Handle(
            GetLogsQuery request, 
            CancellationToken cancellationToken)
        {
            var pageSize = request.PageSize;
            var skipCount = (request.PageNumber - 1) * pageSize;
            var logFiles = GetFilesFromLogDirectoryInDescendingOrder(request.LogsDirectory);

            var entries = new List<LogEntryModel>();
            await foreach (var entry in GetLogEntriesFromFiles(logFiles, request, cancellationToken).WithCancellation(cancellationToken))
                entries.Add(entry);
            return entries.ToListResult();
        }

        private async IAsyncEnumerable<LogEntryModel> GetLogEntriesFromFiles(
            IEnumerable<string> files,
            GetLogsQuery query,
            [EnumeratorCancellation] CancellationToken token)
        {
            var numberOfLinesSkipped = 0;
            var numberOfEntriesReturned = 0;
            foreach (var filePath in files)
            {
                var fileEntries = await GetLogEntriesFromFile(filePath, query.SearchTerm, token);
                foreach (var entry in fileEntries)
                {
                    if (numberOfLinesSkipped == query.SkipCount)
                    {
                        yield return entry;
                        numberOfEntriesReturned++;
                    }
                    else
                        numberOfLinesSkipped++;

                    if (numberOfEntriesReturned == query.PageSize)
                        yield break;
                }
                
                if (numberOfEntriesReturned == query.PageSize)
                    yield break;
            }
        }
        
        private async Task<IEnumerable<LogEntryModel>> GetLogEntriesFromFile(
            string filePath, 
            string searchTerm, 
            CancellationToken token)
        {
            var lines = await File.ReadAllLinesAsync(filePath, token).ConfigureAwait(false);
            return lines
                .Where(line => DoesLineContainSearchTerm(line, searchTerm))
                .Select(_logEntryFactory.CreateFromLine)
                .OrderByDescending(e => e.Timestamp);
        }
        
        private static IEnumerable<string> GetFilesFromLogDirectoryInDescendingOrder(string logsDirectory)
        {
            return Directory.GetFiles(logsDirectory)
                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                .ToArray();
        }

        private static bool DoesLineContainSearchTerm(string line, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            return line.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}