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

    #endregion
}
