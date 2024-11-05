using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Avalonia.Controls.Models.TreeDataGrid;

using Bogus;
using Bogus.DataSets;

using DynamicData;
using DynamicData.Binding;

using DynamicTreeDataGrid;
using DynamicTreeDataGrid.Models.Columns;
using DynamicTreeDataGrid.Models.State;

using ReactiveUI;

namespace SampleApp.ViewModels;

public class MainWindowViewModel : ReactiveObject {
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerOptions.Default) { WriteIndented = true };

    private string? _filterText;

    private bool _preSortDescending = true;

    public MainWindowViewModel() {
        var data = new ObservableCollection<Person>(GenerateFakes(3000)).ToObservableChangeSet(person => person.Id);


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
                    new DynamicTextColumn<Person, string>("FirstName", "FirstName", person => person.FirstName),
                    new DynamicTextColumn<Person, string>("LastName", "LastName", person => person.LastName),
                    new DynamicTextColumn<Person, DateTime>("Date-of-Birth", "DoB", person => person.DateOfBirth),
                    new DynamicTemplateColumn<Person>("Height", "Height", "HeightCell",
                        options: new TemplateColumnOptions<Person> {
                            CompareAscending = (a, b) => {
                                if (a.Height > b.Height)
                                    return 1;

                                if (a.Height == b.Height)
                                    return 0;

                                return -1;
                            },
                            CompareDescending = (a, b) => {
                                if (a.Height > b.Height)
                                    return -1;

                                if (a.Height == b.Height)
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

    public async Task StoreStateAsync(Stream stream) {
        var state = DataSource.GetGridState();
        await JsonSerializer.SerializeAsync(stream, state, _jsonOptions);
    }

    public async Task LoadStateAsync(Stream stream) {
        var result = await JsonSerializer.DeserializeAsync<GridState>(stream);
        Console.WriteLine("Result got got bro");
        if (result is not null) {
            DataSource.ApplyGridState(result);
        }
    }

    private static Func<Person, bool> BuildSearchFilter(string? text) {
        if (string.IsNullOrEmpty(text))
            return _ => true;

        return t => t.FirstName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
            t.LastName.Contains(text, StringComparison.OrdinalIgnoreCase);
    }

    private static List<Person> GenerateFakes(int amount) {
        //Set the randomizer seed to generate repeatable data sets.
        Randomizer.Seed = new Random(8675309);
        var faker = new Faker<Person>().RuleFor(p => p.Id, faker => faker.IndexFaker)
            .RuleFor(p => p.DateOfBirth, faker => faker.Date.Past(80))
            .RuleFor(p => p.Height, faker => faker.Random.Double())
            .RuleFor(p => p.Gender, faker => faker.Person.Gender)
            .RuleFor(p => p.Money, faker => faker.Finance.Amount(-1000M, 1000M, 5))
            .RuleFor(p => p.IsChecked, faker => faker.Random.Bool())
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(p => p.Address, f => f.Address.StreetAddress())
            .RuleFor(p => p.City, f => f.Address.City())
            .RuleFor(p => p.State, f => f.Address.State())
            .RuleFor(p => p.PostalCode, f => f.Address.ZipCode())
            .RuleFor(p => p.Country, f => f.Address.Country())
            .RuleFor(p => p.IsMarried, f => f.Random.Bool())
            .RuleFor(p => p.WeddingAnniversary,
                (f, p) => p.IsMarried ? f.Date.Past(10, p.DateOfBirth.AddYears(18)) : (DateTime?)null)
            .RuleFor(p => p.Hobbies,
                f => f.Make(3,
                    () => f.PickRandom("Fishing", "Cooking", "Gardening", "Reading", "Traveling", "Sports", "Art",
                        "Music")))
            .RuleFor(p => p.LanguagesSpoken, f => f.Make(2, () => f.Random.Word()));


        return faker.Generate(amount);
    }
}

public record Person {
    public int Id { get; set; }
    public DateTime DateOfBirth { get; set; }
    public double Height { get; set; }
    public Name.Gender Gender { get; set; }
    public decimal Money { get; set; }
    public bool IsChecked { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public bool IsMarried { get; set; }
    public DateTime? WeddingAnniversary { get; set; }
    public List<string> Hobbies { get; set; } = [];
    public List<string> LanguagesSpoken { get; set; } = [];
}
