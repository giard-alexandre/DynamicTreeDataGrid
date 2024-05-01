using System;
using System.Collections.ObjectModel;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using Bogus;

using DynamicData.Binding;

using DynamicExtensions.TreeDataGrid;

namespace AvaloniaApplication2.ViewModels;

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
				new TextColumn<Person, int>("ID", person => person.Id),
				new TextColumn<Person, string>("Name", person => person.Name),
				new TextColumn<Person, DateTime>("DoB", person => person.DateOfBirth),
			},
		};
	}
}

public record Person {
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime DateOfBirth { get; set; }
}
