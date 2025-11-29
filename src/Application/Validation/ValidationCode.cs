namespace Lucy.Application.Validation;

/// <summary>
/// Enumeration of validation codes used for identifying specific validation errors.
/// </summary>
public enum ValidationCode
{
    #region Project Validation Codes

    /// <summary>
    /// Indicates that the specified project was not found.
    /// </summary>
    ProjectNotFound,

    /// <summary>
    /// Indicates that the project key is required but was not provided.
    /// </summary>
    ProjectKeyRequired,

    /// <summary>
    /// Indicates that the project key contains invalid characters.
    /// </summary>
    ProjectKeyInvalidCharacters,

    /// <summary>
    /// Indicates that the project key must start with a letter.
    /// </summary>
    ProjectKeyStartWithLetter,

    /// <summary>
    /// Indicates that the project key length is invalid.
    /// </summary>
    ProjectKeyLength,

    /// <summary>
    /// Indicates that the project key already exists.
    /// </summary>
    ProjectKeyExists,

    /// <summary>
    /// Indicates that the project name length is invalid.
    /// </summary>
    ProjectNameLength,

    /// <summary>
    /// Indicates that the project description length is invalid.
    /// </summary>
    ProjectDescriptionLength,

    /// <summary>
    /// Indicates that no data was provided for updating the project.
    /// </summary>
    ProjectNoDataToUpdate,

    #endregion

    #region Status Validation Codes

    /// <summary>
    /// Indicates that the specified status was not found.
    /// </summary>
    StatusNotFound,

    /// <summary>
    /// Indicates that the status key is required but was not provided.
    /// </summary>
    StatusKeyRequired,

    /// <summary>
    /// Indicates that the status key contains invalid characters.
    /// </summary>
    StatusKeyInvalidCharacters,

    /// <summary>
    /// Indicates that the status key must start with a letter.
    /// </summary>
    StatusKeyStartWithLetter,

    /// <summary>
    /// Indicates that the status key length is invalid.
    /// </summary>
    StatusKeyLength,

    /// <summary>
    /// Indicates that the status key already exists.
    /// </summary>
    StatusKeyExists,

    /// <summary>
    /// Indicates that the status name length is invalid.
    /// </summary>
    StatusNameLength,

    /// <summary>
    /// Indicates that the status description length is invalid.
    /// </summary>
    StatusDescriptionLength,

    /// <summary>
    /// Indicates that the status order is invalid.
    /// </summary>
    StatusOrderInvalid,

    /// <summary>
    /// Indicates that no data was provided for updating the status.
    /// </summary>
    StatusNoDataToUpdate,

    /// <summary>
    /// Indicates that the reassignment status was not found.
    /// </summary>
    ReassignStatusNotFound,

    /// <summary>
    /// Indicates that the reassignment status was not found using key.
    /// </summary>
    ReassignStatusKeyNotFound,

    #endregion


    #region Tag Validation Codes

    /// <summary>
    /// Indicates that the specified tag was not found.
    /// </summary>
    TagNotFound,

    /// <summary>
    /// Indicates that the tag key is required but was not provided.
    /// </summary>
    TagKeyRequired,

    /// <summary>
    /// Indicates that the tag key contains invalid characters.
    /// </summary>
    TagKeyInvalidCharacters,

    /// <summary>
    /// Indicates that the tag key must start with a letter.
    /// </summary>
    TagKeyStartWithLetter,

    /// <summary>
    /// Indicates that the tag key length is invalid.
    /// </summary>
    TagKeyLength,

    /// <summary>
    /// Indicates that the tag key already exists.
    /// </summary>
    TagKeyExists,

    /// <summary>
    /// Indicates that the tag name length is invalid.
    /// </summary>
    TagNameLength,

    /// <summary>
    /// Indicates that the tag description length is invalid.
    /// </summary>
    TagDescriptionLength,

    #endregion
    #region Ticket Validation Codes

    /// <summary>
    /// Indicates that the specified ticket was not found.
    /// </summary>
    TicketNotFound,

    /// <summary>
    /// Indicates that the ticket title is required but was not provided.
    /// </summary>
    TicketTitleRequired,

    /// <summary>
    /// Indicates that the ticket title length is invalid.
    /// </summary>
    TicketTitleLength,

    /// <summary>
    /// Indicates that the ticket description length is invalid.
    /// </summary>
    TicketDescriptionLength,

    /// <summary>
    /// Indicates that the status does not belong to the project.
    /// </summary>
    StatusNotInProject,

    /// <summary>
    /// Indicates that no data was provided for updating the ticket.
    /// </summary>
    TicketNoDataToUpdate,

    #endregion

    #region Comment Validation Codes

    /// <summary>
    /// Indicates that the specified comment was not found.
    /// </summary>
    CommentNotFound,

    /// <summary>
    /// Indicates that the comment content is required but was not provided.
    /// </summary>
    CommentContentRequired,

    /// <summary>
    /// Indicates that the comment content length is invalid.
    /// </summary>
    CommentContentLength,

    #endregion
}
