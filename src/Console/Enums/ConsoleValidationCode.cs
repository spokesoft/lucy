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

    /// <summary>
    /// Indicates that the provided project key is invalid.
    /// </summary>
    InvalidProjectKey,

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
}
