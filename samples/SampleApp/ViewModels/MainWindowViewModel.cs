using System;
using System.Collections.ObjectModel;

using Avalonia.Controls.Models.TreeDataGrid;

using Bogus;
using Bogus.DataSets;

using DynamicData.Binding;

using DynamicTreeDataGrid;

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
			.RuleFor(person => person.Height, faker => faker.Random.Double())
			.RuleFor(person => person.Gender, faker => faker.Person.Gender)
			.RuleFor(person => person.Money, faker => faker.Finance.Amount(-1000M, 1000M, 5))
			.Generate(300)).ToObservableChangeSet(person => person.Id);

		DataSource = new DynamicFlatTreeDataGridSource<Person,int>(data) {
			Columns = {
				new TextColumn<Person, int>("ID", person => person.Id),
				new TextColumn<Person, string>("Name", person => person.Name),
				new TextColumn<Person, DateTime>("DoB", person => person.DateOfBirth),
				new TemplateColumn<Person>("Height", "HeightCell"),
				new TextColumn<Person, double>("Raw Height", person => person.Height),
				new TextColumn<Person, Name.Gender>("Gender", person => person.Gender), // To Template
				new TextColumn<Person, decimal>("Money", person => person.Money),
			},
		};
	}
}

public record Person {
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime DateOfBirth { get; set; }
	public double Height { get; set; }
	public Name.Gender Gender { get; set; }
	public Decimal Money { get; set; }
}
