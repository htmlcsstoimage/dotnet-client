namespace HtmlCssToImage.Models.Responses;

/// <summary>
/// Serves as a base class for paginated responses, encapsulating data and pagination details.
/// </summary>
/// <typeparam name="T">The type of data contained in the paginated response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Represents information about pagination details in a paginated response.
    /// </summary>
    public record PaginationInfo(long? next_page_start);

    /// <summary>
    /// Represents the collection of data items contained in the response.
    /// This property holds the elements retrieved in a paginated response.
    /// </summary>
    public T[] Data { get; init; } = null!;

    /// <summary>
    /// Represents pagination information for a response that includes paginated data.
    /// This property provides details about the paging state, such as the starting point for the next page of data.
    /// </summary>
    public PaginationInfo Pagination { get; init; } = null!;
}