using System;
using System.Collections.ObjectModel;

using Bogus;

using DynamicData.Binding;

using DynamicTreeDataGrid;
using DynamicTreeDataGrid.Columns;

namespace SampleApp.ViewModels;

public class MainWindowViewModel : ViewModelBase {
	public DynamicFlatTreeDataGridSource<Person, int> DataSource { get; set; }

	public MainWindowViewModel() {
		//Set the randomizer seed if you wish to generate repeatable data sets.
		Randomizer.Seed = new Random(8675309);
		var data = new ObservableCollection<Person>(new Faker<Person>()
			.RuleFor(person => person.Id, faker => faker.IndexFaker)
			.RuleFor(person => person.Name, faker => faker.Name.FullName())
			.RuleFor(person => person.DateOfBirth, faker => faker.Date.Past(80))
			.Generate(300)).ToObservableChangeSet(person => person.Id);

		DataSource = new DynamicFlatTreeDataGridSource<Person,int>(data) {
			Columns = {
				new DynamicTextColumn<Person, int>("ID", person => person.Id),
				new DynamicTextColumn<Person, string>("Name", person => person.Name),
				new DynamicTextColumn<Person, DateTime>("DoB", person => person.DateOfBirth),
			},
		};
	}
}

public record Person {
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime DateOfBirth { get; set; }
}
