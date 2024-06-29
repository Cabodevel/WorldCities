using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Dynamic.Core;

namespace WorldCities.Server.Data;

public class ApiResult<T>
{
    /// <summary>
    /// Private constructor called by the CreateAsync method.
    /// </summary>
    private ApiResult(
        List<T> data,
        int count,
        int skip, 
        int top,
        string? orderBy,
        string? filterQuery)
    {
        Data = data;
        Skip = skip;
        Top = top;
        TotalCount = count;
        OrderBy = orderBy;
        FilterQuery = filterQuery;
    }

    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int skip,
        int top,
        string? orderBy = null,
        string? filterQuery = null)
    {
        if(!string.IsNullOrEmpty(filterQuery))
        {
            source = source.Where(filterQuery);
        }

        var count = await source.CountAsync();
        if(!string.IsNullOrEmpty(orderBy))
        {
            source = source.OrderBy(orderBy);
        }
        source = source
            .Skip(skip)
            .Take(top);

        var data = await source.ToListAsync();

        return new ApiResult<T>(
            data,
            count,
            skip,
            top,
            orderBy,
            filterQuery);
    }

    /// <summary>
    /// The data result.
    /// </summary>
    public List<T> Data { get; private set; }
    /// <summary>
    /// Zero-based index of current page.
    /// </summary>
    public int Skip { get; private set; }
    /// <summary>
    /// Number of items contained in each page.
    /// </summary>
    public int Top { get; private set; }
    /// <summary>
    /// Total items count
    /// </summary>
    public int TotalCount { get; private set; }
    
    /// <summary>
    /// Sorting Order query
    /// </summary>
    public string? OrderBy { get; set; }
    
    public string? FilterQuery { get; set; }
}