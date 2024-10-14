namespace DynamicTreeDataGrid.Tests;

/// <summary>
/// Test Person used for testing purposes.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="DateOfBirth"></param>
internal record TestPerson(int Id = 0, string Name = "", DateTime DateOfBirth = new());
