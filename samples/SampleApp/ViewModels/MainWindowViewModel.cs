using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

using Bogus;
using Bogus.DataSets;

using DynamicData;
using DynamicData.Binding;

using DynamicTreeDataGrid;
using DynamicTreeDataGrid.Columns;

using ReactiveUI;

namespace SampleApp.ViewModels;

public class MainWindowViewModel : ReactiveObject {
	public DynamicFlatTreeDataGridSource<Person, int> DataSource { get; set; }

	private string? _filterText;
	public string? FilterText
	{
		get => _filterText;
		set => this.RaiseAndSetIfChanged(ref _filterText, value);
	}

	public MainWindowViewModel() {
		//Set the randomizer seed to generate repeatable data sets.
		Randomizer.Seed = new Random(8675309);
		var data = new ObservableCollection<Person>(new Faker<Person>()
			.RuleFor(person => person.Id, faker => faker.IndexFaker)
			.RuleFor(person => person.Name, faker => faker.Name.FullName())
			.RuleFor(person => person.DateOfBirth, faker => faker.Date.Past(80))
			.RuleFor(person => person.Height, faker => faker.Random.Double())
			.RuleFor(person => person.Gender, faker => faker.Person.Gender)
			.RuleFor(person => person.Money, faker => faker.Finance.Amount(-1000M, 1000M, 5))
			.RuleFor(person => person.IsChecked, faker => faker.Random.Bool())
			.Generate(300)).ToObservableChangeSet(person => person.Id);


		var searchFilter = this.WhenValueChanged(t => t.FilterText)
			.Throttle(TimeSpan.FromMilliseconds(500))
			.Select(BuildSearchFilter);

		var filteredData = data.Filter(searchFilter);

		DataSource = new DynamicFlatTreeDataGridSource<Person, int>(filteredData, RxApp.MainThreadScheduler) {
			Columns = {
				new DynamicTextColumn<Person, int>("Id", "Id", person => person.Id),
				new DynamicTextColumn<Person, string>("Name", "Name", person => person.Name),
				new DynamicTextColumn<Person, DateTime>("Date-of-Birth", "DoB", person => person.DateOfBirth),
				new DynamicTemplateColumn<Person>("Height", "Height", "HeightCell"),
				new DynamicTextColumn<Person, double>("Height-Raw", "Raw Height", person => person.Height),
				new DynamicTextColumn<Person, Name.Gender>("Gender", "Gender", person => person.Gender), // To Template
				new DynamicTextColumn<Person, decimal>("Money", "Money", person => person.Money),
				new DynamicCheckBoxColumn<Person>("Checked", "Checked", person => person.IsChecked),
			},
		};
	}

	private static Func<Person, bool> BuildSearchFilter(string? text) {
		if (string.IsNullOrEmpty(text))
			return _ => true;

		return t => t.Name.Contains(text, StringComparison.OrdinalIgnoreCase);
	}
}

public record Person {
	public int Id { get; set; }
	public string Name { get; set; } = "";
	public DateTime DateOfBirth { get; set; }
	public double Height { get; set; }
	public Name.Gender Gender { get; set; }
	public decimal Money { get; set; }
	public bool IsChecked { get; set; }
}
