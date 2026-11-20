using System.Text.Json.Serialization;

namespace Common.Patchouli
{
	/// <summary>
	/// A .NET Abstract Class representing a Patchouli Page
	/// </summary>
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
	[JsonDerivedType(typeof(CraftingPage), "patchouli:crafting")]
	[JsonDerivedType(typeof(EmptyPage), "patchouli:empty")]
	[JsonDerivedType(typeof(EntityPage), "patchouli:entity")]
	[JsonDerivedType(typeof(ImagePage), "patchouli:image")]
	[JsonDerivedType(typeof(LinkPage), "patchouli:link")]
	[JsonDerivedType(typeof(MultiblockPage), "patchouli:multiblock")]
	[JsonDerivedType(typeof(QuestPage), "patchouli:quest")]
	[JsonDerivedType(typeof(RelationsPage), "patchouli:relations")]
	[JsonDerivedType(typeof(SmeltingPage), "patchouli:smelting")]
	[JsonDerivedType(typeof(SpotlightPage), "patchouli:spotlight")]
	[JsonDerivedType(typeof(TextPage), "patchouli:text")]
	[JsonDerivedType(typeof(TablePage), "tfg:table")]
	[JsonDerivedType(typeof(HeatRecipePage), "tfg:heat_recipe")]
	[JsonDerivedType(typeof(MultimultiblockPage), "tfg:multimultiblock")]
	public abstract class PatchouliPage
	{
		[JsonPropertyName("type")]
		public abstract string Type { get; }

		[JsonPropertyName("advancement")]
		public string? Advancement { get; set; }

		[JsonPropertyName("anchor")]
		public string? Anchor { get; set; }
	}
}
