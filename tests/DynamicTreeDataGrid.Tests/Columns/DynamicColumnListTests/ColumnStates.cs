using DynamicTreeDataGrid.Columns;
using DynamicTreeDataGrid.State;

using Microsoft.Reactive.Testing;

namespace DynamicTreeDataGrid.Tests.Columns.DynamicColumnListTests;

public class ColumnStates {
    private readonly TestScheduler _scheduler = new();
    private readonly DynamicColumnList<TestPerson> _list = [];

    [Before(Test)]
    public async Task Setup() {
        await Task.CompletedTask;
        // Build the collection with specific indexes
        _list.Insert(0, new DynamicTextColumn<TestPerson, int>("Id", "Id", person => person.Id));
        _list.Insert(1, new DynamicTextColumn<TestPerson, string>("Name", "Name", person => person.Name));
        _list.Insert(2,
            new DynamicTextColumn<TestPerson, DateTime>("Date-of-Birth", "DoB", person => person.DateOfBirth));
    }

    [After(Test)]
    public async Task CleanupAfterTests() { await Task.CompletedTask; }

    [Test]
    public async Task Default_Collection_State() {
        await Assert.That(_list.Count).IsEqualTo(3);

        // Check that all items are in the right order and visible
        await Assert.That(_list[0].Name).IsEqualTo("Id");
        await Assert.That(_list[0].Visible).IsEqualTo(true);
        await Assert.That(_list[1].Name).IsEqualTo("Name");
        await Assert.That(_list[1].Visible).IsEqualTo(true);
        await Assert.That(_list[2].Name).IsEqualTo("Date-of-Birth");
        await Assert.That(_list[2].Visible).IsEqualTo(true);
    }

    [Test]
    public async Task GetColumnStates_ReturnsAllStates() {
        var result = _list.GetColumnStates();
        IList<ColumnState> expected = [
            new("Id") { Index = 0, Visible = true },
            new("Name") { Index = 1, Visible = true },
            new("Date-of-Birth") { Index = 2, Visible = true },
        ];

        // Check that all items are in the right order and visible
        await Assert.That(result.Count()).IsNotNull()
            .And. IsPositive()
            .And.IsEqualTo(3);

        await Assert.That(result).IsEquivalentCollectionTo(expected);
    }

    [Test]
    public async Task ApplyColumnStates_WithFullValidStates_AppliesProperly() {
        List<ColumnState> loadedStates = [
            new("Id") { Index = 2, Visible = true },
            new("Name") { Index = 0, Visible = true },
            new("Date-of-Birth") { Index = 1, Visible = false },
        ];
        var result = _list.ApplyColumnStates(loadedStates);

        // Check that all items are in the right order and visible
        await Assert.That(result).IsTrue();

        // Check that items are in the right order/state applied.
        await Assert.That(_list[0].Name).IsEqualTo("Name");
        await Assert.That(_list[0].Visible).IsEqualTo(true);
        await Assert.That(_list[1].Name).IsEqualTo("Date-of-Birth");
        await Assert.That(_list[1].Visible).IsEqualTo(false);
        await Assert.That(_list[2].Name).IsEqualTo("Id");
        await Assert.That(_list[2].Visible).IsEqualTo(true);
    }

    [Test]
    public async Task ApplyColumnStates_WithPartialValidStates_AppliesProperly() {
        List<ColumnState> loadedStates = [
            new("Id") { Index = 2, Visible = true },
            new("Date-of-Birth") { Index = 1, Visible = false },
        ];
        var result = _list.ApplyColumnStates(loadedStates);

        // Check that all items are in the right order and visible
        await Assert.That(result).IsTrue();

        // Check that items are in the right order/state applied.
        await Assert.That(_list[0].Name).IsEqualTo("Name");
        await Assert.That(_list[0].Visible).IsEqualTo(true);
        await Assert.That(_list[1].Name).IsEqualTo("Date-of-Birth");
        await Assert.That(_list[1].Visible).IsEqualTo(false);
        await Assert.That(_list[2].Name).IsEqualTo("Id");
        await Assert.That(_list[2].Visible).IsEqualTo(true);
    }

    [Test]
    public async Task ApplyColumnStates_WithSameIndexStates_AppliesThemInOrderOfAppearance() {
        List<ColumnState> loadedStates = [
            new("Id") { Index = 0, Visible = true },
            new("Date-of-Birth") { Index = 0, Visible = false },
        ];
        var result = _list.ApplyColumnStates(loadedStates);

        // Check that all items are in the right order and visible
        await Assert.That(result).IsTrue();

        // Check that items are in the right order/state applied.
        await Assert.That(_list[0].Name).IsEqualTo("Date-of-Birth");
        await Assert.That(_list[0].Visible).IsEqualTo(false);
        await Assert.That(_list[1].Name).IsEqualTo("Id");
        await Assert.That(_list[1].Visible).IsEqualTo(true);
        await Assert.That(_list[2].Name).IsEqualTo("Name");
        await Assert.That(_list[2].Visible).IsEqualTo(true);
    }
}
