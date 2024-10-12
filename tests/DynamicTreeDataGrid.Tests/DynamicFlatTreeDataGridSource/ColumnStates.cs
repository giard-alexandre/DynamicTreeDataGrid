using TUnit.Core;

namespace DynamicTreeDataGrid.Tests.DynamicFlatTreeDataGridSource;

public class ColumnStates {

	[Before(Test)]
	public async Task Setup()
	{
		await Task.CompletedTask;
	}

	[Test]
	public async Task MyTest() {
		var result = true;

		await Assert.That(result).IsEqualTo(true);
	}
}

internal record TestPerson(uint Id, string Name, uint Age);
