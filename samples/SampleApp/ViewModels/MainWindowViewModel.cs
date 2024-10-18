using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;

using Avalonia.Controls.Models.TreeDataGrid;

using Bogus;
using Bogus.DataSets;

using DynamicData;
using DynamicData.Binding;

using DynamicTreeDataGrid;
using DynamicTreeDataGrid.Models.Columns;

using ReactiveUI;

namespace SampleApp.ViewModels;

public class MainWindowViewModel : ReactiveObject {
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerOptions.Default) { WriteIndented = true };

    private string? _filterText;

    private bool _preSortDescending = true;

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

        var preSortDirectionStream = this.WhenValueChanged(t => t.PreSortDescending)
            .Select(b => b ? SortDirection.Descending : SortDirection.Ascending)
            .Do(x => {
                if (DataSource is not null)
                    DataSource.Options.PreColumnSort =
                        new SortExpressionComparer<Person> {
                            new SortExpression<Person>(person => person.IsChecked, x),
                        };
            });

        DataSource =
            new DynamicFlatTreeDataGridSource<Person, int>(filteredData, RxApp.MainThreadScheduler,
                new DynamicTreeDataGridSourceOptions<Person> {
                    PreColumnSort = SortExpressionComparer<Person>.Descending(person => person.IsChecked),
                    PostColumnSort = SortExpressionComparer<Person>.Descending(person => person.Money),
                    Resorter = preSortDirectionStream.Select(_ => Unit.Default),
                }) {
                Columns = {
                    new DynamicTextColumn<Person, int>("Id", "Id", person => person.Id),
                    new DynamicTextColumn<Person, string>("Name", "Name", person => person.Name),
                    new DynamicTextColumn<Person, DateTime>("Date-of-Birth", "DoB", person => person.DateOfBirth),
                    new DynamicTemplateColumn<Person>("Height", "Height", "HeightCell",
                        options: new TemplateColumnOptions<Person> {
                            CompareAscending = (person, person1) => {
                                if (person.Height > person1.Height)
                                    return 1;

                                if (person.Height == person1.Height)
                                    return 0;

                                return -1;
                            },
                            CompareDescending = (person, person1) => {
                                if (person.Height > person1.Height)
                                    return -1;

                                if (person.Height == person1.Height)
                                    return 0;

                                return 1;
                            },
                        }),
                    new DynamicTextColumn<Person, double>("Height-Raw", "Raw Height", person => person.Height),
                    new DynamicTextColumn<Person, Name.Gender>("Gender", "Gender",
                        person => person.Gender), // To Template
                    new DynamicTextColumn<Person, decimal>("Money", "Money", person => person.Money),
                    new DynamicCheckBoxColumn<Person>("Checked", "Checked", person => person.IsChecked),
                },
            };
    }

    public DynamicFlatTreeDataGridSource<Person, int> DataSource { get; set; }

    public string? FilterText {
        get => _filterText;
        set => this.RaiseAndSetIfChanged(ref _filterText, value);
    }

    public bool PreSortDescending {
        get => _preSortDescending;
        set => this.RaiseAndSetIfChanged(ref _preSortDescending, value);
    }

    public void PrintColumnStates() {
        Console.WriteLine(JsonSerializer.Serialize(DataSource.GetGridState(), _jsonOptions));
    }

    /// <summary>
    ///     Randomly sets one of the columns sorting direction to <see cref="ListSortDirection.Ascending" />
    /// </summary>
    public void ApplyRandomState() {
        var currentState = DataSource.GetGridState();
        int randomColumn = new Random().Next(currentState.ColumnStates.Count);

        // Clear state columns sort
        foreach (var c in currentState.ColumnStates)
            c.SortDirection = null;
        currentState.ColumnStates[randomColumn].SortDirection = ListSortDirection.Ascending;
        DataSource.ApplyGridState(currentState);
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
