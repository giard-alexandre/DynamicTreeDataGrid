using System.Collections.ObjectModel;

using Bogus;

using DynamicData.Binding;

using DynamicTreeDataGrid.Columns;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace DynamicTreeDataGrid.Tests.DynamicFlatTreeDataGridSource;

public class Initialization {
    private const int ItemCount = 300;
    private DynamicFlatTreeDataGridSource<TestPerson, int> _source;

    private readonly TestScheduler _scheduler = new();

    [Before(Test)]
    public async Task Setup() {
        //Set the randomizer seed to generate repeatable data sets.
        Randomizer.Seed = new Random(8675309);
        var fakeData = new Faker<TestPerson>()
            .CustomInstantiator(f => new TestPerson(f.IndexFaker, f.Name.FullName(), f.Person.DateOfBirth))
            .Generate(ItemCount);
        var data = new ObservableCollection<TestPerson>(fakeData).ToObservableChangeSet(person => person.Id);


        _source = new DynamicFlatTreeDataGridSource<TestPerson, int>(data, _scheduler) {
            Columns = {
                new DynamicTextColumn<TestPerson,int>("Id", "Id", person => person.Id),
                new DynamicTextColumn<TestPerson,string>("Name", "Name", person => person.Name),
                new DynamicTextColumn<TestPerson,DateTime>("Date-of-Birth", "DoB", person => person.DateOfBirth),
            },
        };
        await Task.CompletedTask;
    }

    [After(Test)]
    public async Task CleanupAfterTests() {
        await Task.CompletedTask;
        _source.Dispose();
    }

    [Test]
    public async Task Constructor_ChangeSet_PopulatesItems() {
        await _scheduler.WithAsync(async testScheduler => {
            // Tick once to get ChangeSet data
            testScheduler.AdvanceBy(1);
            await Assert.That(_source.Items.Count).IsEqualTo(ItemCount);

        });

    }
}