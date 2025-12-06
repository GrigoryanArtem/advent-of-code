using System.Text;

namespace Puzzles.Runner.Base.Table;

public class TableBuilder(TableOptions options) 
{ 
    private readonly List<Column> _columns = [];
    private readonly List<string[]> _rows = [];

    private string _vSeparator = "|";
    private string _hSeparator = "-";
    private string _cornerSeparator = "+";

    private int _margin = 0;    

    public TableBuilder AddColumn(Column column)
    {
        _columns.Add(column);
        return this;
    }

    public TableBuilder AddRow(params object[] values)
    {
        var row = new string[_columns.Count];

        for (int i = 0; i < _columns.Count; i++)
        {
            if (i < values.Length)
            {
                var format = _columns[i].Format;
                row[i] = format is not null ? string.Format(format, values[i]) : values[i]?.ToString() ?? String.Empty;
            }
            else
            {
                row[i] = String.Empty;
            }
        }
        _rows.Add(row);
        return this;
    }

    public string BuildMd()
    {
        var widths = CalculateColumnWidths();
        var sb = new StringBuilder();
       
        foreach (var (column, i) in _columns.WithIndex())
        {
            sb.Append("|");
            AppendStringFromColumn(sb, column, column.Header, widths[i]);
        }
        sb.AppendLine("|");

        foreach (var (column, i) in _columns.WithIndex())
        {
            sb.Append("|");
            AppendMdAlignString(sb, column.Align, widths[i] + 2 * _margin);
        }
        sb.AppendLine("|");

        foreach (var row in _rows)
        {
            foreach (var (cell, i) in row.WithIndex())
            {
                sb.Append("|");
                AppendStringFromColumn(sb, _columns[i], cell, widths[i]);
            }
            sb.AppendLine("|");
        }

        return sb.ToString();

    }

    public string Build()
    {
        var widths = CalculateColumnWidths();

        var sb = new StringBuilder();

        if (options.HasFlag(TableOptions.Borders))
            AppendHSeparator(sb, widths, options);

        if (options.HasFlag(TableOptions.Header))
        {
            if (options.HasFlag(TableOptions.Borders))
                sb.Append(_vSeparator);

            foreach (var (column, i) in _columns.WithIndex())
            {
                if (i > 0)
                    sb.Append(_vSeparator);

                AppendStringFromColumn(sb, column, column.Header, widths[i]);
            }

            if (options.HasFlag(TableOptions.Borders))
                sb.Append(_vSeparator);

            sb.AppendLine();
        }

        if (options.HasFlag(TableOptions.Separator))
            AppendHSeparator(sb, widths, options);

        foreach (var row in _rows)
        {
            if (options.HasFlag(TableOptions.Borders))
                sb.Append(_vSeparator);

            foreach (var (cell, i) in row.WithIndex())
            {
                if (i > 0)
                    sb.Append(_vSeparator);
                AppendStringFromColumn(sb, _columns[i], cell, widths[i]);
            }

            if (options.HasFlag(TableOptions.Borders))
                sb.Append(_vSeparator);

            sb.AppendLine();
        }

        if (options.HasFlag(TableOptions.Borders))
            AppendHSeparator(sb, widths, options);

        return sb.ToString();
    }

    public TableBuilder SetMargin(int margin)
    {
        _margin = margin;
        return this;
    }

    public TableBuilder SetVSeparator(string separator)
    {
        _vSeparator = separator;
        return this;
    }

    public TableBuilder SetHSeparator(string separator)
    {
        _hSeparator = separator;
        return this;
    }

    public TableBuilder SetCornerSeparator(string separator)
    {
        _cornerSeparator = separator;
        return this;
    }

    public static TableBuilder Create(TableOptions options = TableOptions.None)
        => new(options);

    #region Private methods

   

    private int[] CalculateColumnWidths()
    {
        var widths = new int[_columns.Count];

        for (int i = 0; i < _columns.Count; i++)
        {
            var header = _columns[i].Header.Length;
            var rows = _rows.Max(r => r[i].Length);
            var @default = _columns[i].Width ?? 0;

            widths[i] = Math.Max(Math.Max(header, rows), @default);
        }

        return widths;
    }

    private void AppendHSeparator(StringBuilder stringBuilder, int[] widths, TableOptions options)
    {
        if (options.HasFlag(TableOptions.Borders))
            stringBuilder.Append(_cornerSeparator);

        foreach (var (width, i) in widths.WithIndex())
        {
            if (i > 0)
                stringBuilder.Append(_cornerSeparator);

            stringBuilder.Append(new string(_hSeparator[0], width + 2 * _margin));
        }

        if (options.HasFlag(TableOptions.Borders))
            stringBuilder.Append(_cornerSeparator);

        stringBuilder.AppendLine();
    }

    private void AppendStringFromColumn(StringBuilder stringBuilder, Column column, string str, int width)
    {
        stringBuilder.Append(' ', _margin);

        switch (column.Align)
        {
            case Align.Left:
                stringBuilder.Append(str.PadRight(width));
                break;
            case Align.Center:
                int spaces = width - str.Length;
                int padLeft = spaces / 2 + str.Length;
                stringBuilder.Append(str.PadLeft(padLeft).PadRight(width));
                break;
            case Align.Right:
                stringBuilder.Append(str.PadLeft(width));
                break;
            default:
                stringBuilder.Append(str.PadRight(width));
                break;
        }
        stringBuilder.Append(' ', _margin);
    }

    private void AppendMdAlignString(StringBuilder builder, Align align, int width)
    {
        switch (align)
        {
            case Align.Left:
                builder.Append(':');
                builder.Append(new string('-', width - 1));
                break;
            case Align.Center:
                builder.Append(':');
                builder.Append(new string('-', width - 2));
                builder.Append(':');
                break;
            case Align.Right:
                builder.Append(new string('-', width - 1));
                builder.Append(':');
                break;
        }
    }

    #endregion
}
