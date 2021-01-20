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
        public string Level => Parameters?.Level;
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
                var fileEntries = await GetLogEntriesFromFile(filePath, query, token);
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
            GetLogsQuery query, 
            CancellationToken token)
        {
            var lines = await File.ReadAllLinesAsync(filePath, token).ConfigureAwait(false);
            return lines
                .Select(_logEntryFactory.CreateFromLine)
                .Where(e => DoesEntryMatchQuery(e, query))
                .OrderByDescending(e => e.Timestamp);
        }
        
        private static IEnumerable<string> GetFilesFromLogDirectoryInDescendingOrder(string logsDirectory)
        {
            return Directory.GetFiles(logsDirectory)
                .OrderByDescending(GetLogFileNumber)
                .ToArray();
        }

        private static bool DoesEntryMatchQuery(LogEntryModel entry, GetLogsQuery query)
        {
            return DoesEntryMatchLevel(entry, query.Level)
                   && DoesEntryContainSearchTerm(entry, query.SearchTerm);
        }

        private static bool DoesEntryContainSearchTerm(LogEntryModel entry, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            return entry.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        private static bool DoesEntryMatchLevel(LogEntryModel entry, string level)
        {
            if (string.IsNullOrWhiteSpace(level))
                return true;

            return entry.Level.Equals(level, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetLogFileNumber(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var numberString = fileName.Contains("_")
                ? fileName.Split('_')[1]
                : "0";
            return int.TryParse(numberString, out var number)
                ? number
                : int.MinValue;
        }
    }
}