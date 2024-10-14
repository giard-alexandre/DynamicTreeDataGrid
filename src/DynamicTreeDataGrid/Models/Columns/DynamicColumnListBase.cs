using System.Collections.ObjectModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Utilities;

namespace DynamicTreeDataGrid.Models.Columns;

/// <summary>
///     An implementation of <see cref="IColumns" /> that stores its columns in a list.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <remarks>
///     Basically a re-implementation of <see cref="ColumnList{TModel}" /> that enforces a unique <c>Name</c> key
/// </remarks>
public class DynamicColumnListBase<TModel> : NotifyingListBase<IDynamicColumn<TModel>>, IDynamicColumnsBase {
    private readonly HashSet<string> _nameSet = [];
    private double _viewportWidth;

    public event EventHandler? LayoutInvalidated;

    public Size CellMeasured(int columnIndex, int rowIndex, Size size) {
        var column = (IUpdateColumnLayout)this[columnIndex];
        return new Size(column.CellMeasured(size.Width, rowIndex), size.Height);
    }

    public (int index, double x) GetColumnAt(double x) {
        var start = 0.0;

        for (var i = 0; i < Count; ++i) {
            var column = this[i];
            var end = start + column.ActualWidth;
            if (x >= start && x < end)
                return (i, start);
            if (double.IsNaN(column.ActualWidth))
                return (-1, -1);
            start += column.ActualWidth;
        }

        return (-1, -1);
    }

    public double GetEstimatedWidth(double constraint) {
        var hasStar = false;
        var totalMeasured = 0.0;
        var measuredCount = 0;
        var unmeasuredCount = 0;

        for (var i = 0; i < Count; ++i) {
            var column = (IUpdateColumnLayout)this[i];

            if (column.Width.IsStar) {
                hasStar = true;
                totalMeasured += column.MinActualWidth;
            }
            else if (!double.IsNaN(column.ActualWidth)) {
                totalMeasured += column.ActualWidth;
                ++measuredCount;
            }
            else {
                ++unmeasuredCount;
            }
        }

        // If there are star columns, and all measured columns fit within the available space
        // then we will fill the available space.
        if (hasStar && !double.IsInfinity(constraint) && totalMeasured < constraint)
            return constraint;

        // If there are a mix of measured and unmeasured columns then use the measured columns
        // to estimate the size of the unmeasured columns.
        if (measuredCount > 0 && unmeasuredCount > 0) {
            var estimated = totalMeasured / measuredCount * unmeasuredCount;
            return totalMeasured + estimated;
        }

        return totalMeasured;
    }

    public void CommitActualWidths() => UpdateColumnSizes();

    public void SetColumnWidth(int columnIndex, GridLength width) {
        var column = this[columnIndex];

        if (width != column.Width) {
            ((IUpdateColumnLayout)column).SetWidth(width);
            LayoutInvalidated?.Invoke(this, EventArgs.Empty);
            UpdateColumnSizes();
        }
    }

    public void ViewportChanged(Rect viewport) {
        if (_viewportWidth != viewport.Width) {
            _viewportWidth = viewport.Width;
            UpdateColumnSizes();
        }
    }

    IDynamicColumn IReadOnlyList<IDynamicColumn>.this[int index] => this[index];
    IEnumerator<IDynamicColumn> IEnumerable<IDynamicColumn>.GetEnumerator() => GetEnumerator();
    IColumn IReadOnlyList<IColumn>.this[int index] => this[index];
    IEnumerator<IColumn> IEnumerable<IColumn>.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Adds the elements of the specified <see cref="IEnumerable{T}" /> to the end of this
    ///     <see cref="DynamicColumnListBase{TModel}" />
    /// </summary>
    /// <param name="items">The elements to add to this <see cref="DynamicColumnListBase{TModel}" /></param>
    public void AddRange(IEnumerable<IDynamicColumn<TModel>> items) {
        foreach (var item in items)
            Add(item);
    }

    /// <inheritdoc cref="Collection{T}.InsertItem" />
    /// <remarks>Overrides the implementation, adding unique-name logic</remarks>
    protected override void InsertItem(int index, IDynamicColumn<TModel> item) {
        if (!_nameSet.Add(item.Name)) throw new ArgumentException("Attempted to add column with duplicate Name.");
        base.InsertItem(index, item);
    }

    /// <inheritdoc cref="Collection{T}.SetItem" />
    /// <remarks>Overrides the implementation, adding unique-name logic</remarks>
    protected override void SetItem(int index, IDynamicColumn<TModel> item) {
        if (_nameSet.Contains(item.Name) && this[index].Name != item.Name)
            throw new ArgumentException("Attempted to add column with duplicate Name.");

        _nameSet.Add(item.Name);
        base.SetItem(index, item);
    }

    protected override void ClearItems() {
        base.ClearItems();
        _nameSet.Clear();
    }

    protected override void RemoveItem(int index) {
        var removedName = this[index].Name;
        _nameSet.Remove(removedName);
        base.RemoveItem(index);
    }

    private void UpdateColumnSizes() {
        var totalStars = 0.0;
        var availableSpace = _viewportWidth;
        var invalidated = false;

        // First commit the actual width for all non-star width columns and get a total of the
        // number of stars for star width columns.
        for (var i = 0; i < Count; ++i) {
            var column = (IUpdateColumnLayout)this[i];

            if (!column.Width.IsStar) {
                invalidated |= column.CommitActualWidth();
                availableSpace -= NotNaN(column.ActualWidth);
            }
            else {
                totalStars += column.Width.Value;
            }
        }

        if (totalStars > 0) {
            // Size the star columns.
            var starWidthWasConstrained = false;
            var used = 0.0;

            availableSpace = Math.Max(0, availableSpace);

            // Do a first pass to calculate star column widths.
            for (var i = 0; i < Count; ++i) {
                var column = (IUpdateColumnLayout)this[i];

                if (column.Width.IsStar) {
                    column.CalculateStarWidth(availableSpace, totalStars);
                    used += NotNaN(column.ActualWidth);
                    starWidthWasConstrained |= column.StarWidthWasConstrained;
                }
            }

            // If the width of any star columns was constrained by their min/max size, and we
            // actually had any space to distribute between star columns, then we need to update
            // the star width for the non-constrained columns.
            if (starWidthWasConstrained && MathUtilities.GreaterThan(availableSpace, 0)) {
                for (var i = 0; i < Count; ++i) {
                    var column = (IUpdateColumnLayout)this[i];

                    if (column.StarWidthWasConstrained) {
                        availableSpace -= column.ActualWidth;
                        totalStars -= column.Width.Value;
                    }
                }

                for (var i = 0; i < Count; ++i) {
                    var column = (IUpdateColumnLayout)this[i];
                    if (column.Width.IsStar && !column.StarWidthWasConstrained)
                        column.CalculateStarWidth(availableSpace, totalStars);
                }
            }

            // Finally commit the star column widths.
            for (var i = 0; i < Count; ++i) {
                var column = (IUpdateColumnLayout)this[i];

                if (column.Width.IsStar) invalidated |= column.CommitActualWidth();
            }
        }

        if (invalidated)
            LayoutInvalidated?.Invoke(this, EventArgs.Empty);
    }

    private static double NotNaN(double v) => double.IsNaN(v) ? 0 : v;
}
