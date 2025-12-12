namespace Lucy.Console.Enums;

/// <summary>
/// Enumeration of validation codes used for identifying specific validation errors.
/// </summary>
public enum ConsoleValidationCode
{
    #region Project Validation Codes

    /// <summary>
    /// Indicates that the command requires either a project key or ID to be
    /// specified.
    /// </summary>
    ProjectKeyOrIdRequired,

    /// <summary>
    /// Indicates that the project key is required.
    /// </summary>
    ProjectKeyRequired,

    /// <summary>
    /// Indicates that the project could not be found by the specified key.
    /// </summary>
    ProjectKeyNotFound,

    /// <summary>
    /// Indicates that the provided project key is invalid.
    /// </summary>
    InvalidProjectKey,

    /// <summary>
    /// Indicates that a project with the specified key already exists.
    /// </summary>
    ProjectAlreadyExists,

    #endregion

    #region Status Validation Codes

    /// <summary>
    /// Indicates that the command requires either a status key or ID to be
    /// specified.
    /// </summary>
    StatusKeyOrIdRequired,

    /// <summary>
    /// Indicates that the status could not be found by the specified key.
    /// </summary>
    StatusKeyNotFound,

    /// <summary>
    /// Indicates that when using status key, either project key or ID must be specified.
    /// </summary>
    ProjectKeyOrIdRequiredForStatusKey,

    #endregion

    #region Iteration Validation Codes

    /// <summary>
    /// Indicates that the command requires either an iteration key or ID to be
    /// specified.
    /// </summary>
    IterationKeyOrIdRequired,

    /// <summary>
    /// Indicates that the iteration could not be found by the specified ID.
    /// </summary>
    IterationNotFound,

    /// <summary>
    /// Indicates that the iteration could not be found by the specified key.
    /// </summary>
    IterationKeyNotFound,

    #endregion

    #region General Validation Codes

    /// <summary>
    /// Indicates that the start date must be before the end date.
    /// </summary>
    InvalidDateRange,

    #endregion

    #region Ticket Validation Codes

    /// <summary>
    /// Indicates that the command requires either a ticket key or ID to be
    /// specified.
    /// </summary>
    TicketKeyOrIdRequired,

    /// <summary>
    /// Indicates that the ticket could not be found.
    /// </summary>
    TicketNotFound,

    /// <summary>
    /// Indicates that both ticket key and ID were specified.
    /// </summary>
    TicketKeyAndIdMutuallyExclusive,

    #endregion

    #region Tag Validation Codes

    /// <summary>
    /// Indicates that the command requires either a tag key or ID to be
    /// specified.
    /// </summary>
    TagKeyOrIdRequired,

    /// <summary>
    /// Indicates that the tag could not be found.
    /// </summary>
    TagNotFound,

    /// <summary>
    /// Indicates that the tag key is required.
    /// </summary>
    TagKeyRequired,

    #endregion

    #region Comment Validation Codes

    /// <summary>
    /// Indicates that the comment content is required.
    /// </summary>
    CommentContentRequired,

    /// <summary>
    /// Indicates that the comment could not be found.
    /// </summary>
    CommentNotFound,

    /// <summary>
    /// Indicates that a target (Key, ProjectId, or TicketId) is required for the comment.
    /// </summary>
    CommentTargetRequired,

    /// <summary>
    /// Indicates that the target for the comment could not be found.
    /// </summary>
    CommentTargetNotFound,

    #endregion

    #region Iteration Validation Codes (Extended)

    /// <summary>
    /// Indicates that both iteration key and ID were specified.
    /// </summary>
    IterationKeyAndIdMutuallyExclusive,

    #endregion
}
