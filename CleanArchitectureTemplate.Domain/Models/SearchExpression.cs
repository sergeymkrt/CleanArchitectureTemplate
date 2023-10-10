using CleanArchitectureTemplate.Domain.Enums;

namespace CleanArchitectureTemplate.Domain.Models;

/// <summary>
/// The expression for building custom search functionality.
/// </summary>
public class SearchExpression
{
    /// <summary>
    /// The column we are starting with, dot (.) separated.
    /// </summary>
    public string Column { get; set; }
    /// <summary>
    /// The join we are using for joining with child
    /// </summary>
    public SearchExpressionOperation? Join { get; set; }

    /// <summary>
    /// A reference to the children search expressions, if any,
    /// to OR together with this parent column.
    /// </summary>
    public ICollection<SearchExpression> Children { get; set; }

    /// <summary>
    /// The default constructor when there is no child for expression.
    /// </summary>
    /// <param name="column">
    /// The column we are building.
    /// Supports dot (.) notation for relations.
    /// </param>
    public SearchExpression(string column)
    {
        Column = column;
    }

    /// <summary>
    /// The constructor for building the expression when we have to join
    /// with a child expression.
    /// </summary>
    /// <param name="column">
    /// The column we are building.
    /// Supports dot (.) notation for relations.
    /// </param>
    /// <param name="join">
    /// The operation we are joining with the child.
    /// </param>
    /// <param name="children">
    /// The children expressions to join together with OR
    /// </param>
    public SearchExpression(string column, SearchExpressionOperation join, params SearchExpression[] children) : this(column)
    {
        Join = join;
        Children = children;
    }

    /// <summary>
    /// The constructor for building the expression using
    /// column names for children as string. This just shadows the method
    /// that receives instances of the <see cref="SearchExpression"/> as
    /// children by decoding names to <see cref="SearchExpression"/> instance.
    /// </summary>
    /// <param name="column">
    /// The column we are building.
    /// Supports dot (.) notation for relations.
    /// </param>
    /// <param name="join">
    /// The operation we are joining with the child.
    /// </param>
    /// <param name="children">
    /// The children columns to join together with OR
    /// </param>
    public SearchExpression(
        string column, SearchExpressionOperation join,
        params string[] children) : this(column, join,
        children.Select(c => new SearchExpression(c)).ToArray())
    { }
}
