using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	/// <summary>
	/// A .NET represnetation of a Patchouli Entry
	/// <para></para>
	/// <a href="https://vazkiimods.github.io/Patchouli/docs/reference/entry-json">Patchouli Docs</a>
	/// </summary>
	[Serializable]
	public class PatchouliEntry
	{
		[JsonPropertyName("__credits__")]
		public string? Credits { get; set; }

		[JsonPropertyName("name")]
		public required string Name { get; set; }

		[JsonPropertyName("icon")]
		public required string Icon { get; set; }

		[JsonPropertyName("category")]
		public required string Category { get; set; }

		[JsonPropertyName("read_by_default")]
		public bool ReadByDefault { get; set; } = true;

		[JsonPropertyName("priority")]
		public bool Priority { get; set; } = false;

		[JsonPropertyName("advancement")]
		public string? RequiredAdvancement { get; set; }

		[JsonPropertyName("secret")]
		public bool IsSecret { get; set; } = false;

		[JsonPropertyName("pages")]
		public required List<PatchouliPage> Pages { get; set; }

		[JsonPropertyName("sortnum")]
		public int? SortNum { get; set; }

		[JsonPropertyName("extra_recipe_mappings")]
		public Dictionary<string, int>? ExtraRecipeMappings { get; set; }

		[JsonIgnore]
		public string? FileNameWithoutExtension { get; set; }
	}
}