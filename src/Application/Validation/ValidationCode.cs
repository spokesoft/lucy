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
    /// Indicates that the provided project key contains non-alphanumeric characters.
    /// </summary>
    ProjectKeyAlphaNumeric,

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
}
