using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	public class TablePage : PatchouliPage
	{
		public override string Type => "tfg:table";

		[JsonPropertyName("strings")]
		public required object[] Strings { get; set; }

		[JsonPropertyName("title")]
		public string? Title { get; set; }

		[JsonPropertyName("legend")]
		public object? Legend { get; set; }

		[JsonPropertyName("columns")]
		public int Columns { get; set; }

		[JsonPropertyName("rows")]
		public int Rows { get; set; }

		[JsonPropertyName("first_column_width")]
		public int FirstColumnWidth { get; set; }

		[JsonPropertyName("column_width")]
		public int ColumnWidth { get; set; }

		[JsonPropertyName("row_height")]
		public int RowHeight { get; set; }

		[JsonPropertyName("left_buffer")]
		public int LeftBuffer { get; set; }

		[JsonPropertyName("top_buffer")]
		public int TopBuffer { get; set; }

		[JsonPropertyName("draw_background")]
		public bool DrawBackground { get; set; }
	}
}
