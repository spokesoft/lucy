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
    /// Indicates that the project could not be found by the specified key.
    /// </summary>
    ProjectKeyNotFound,

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

    #endregion
}
