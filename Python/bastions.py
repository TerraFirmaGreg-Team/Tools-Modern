import os
from typing import Set, Any, Tuple, NamedTuple, Literal, Union
from glob import glob

from nbtlib import nbt
from nbtlib.tag import String as StringTag, Int as IntTag

#STRUCTURES_DIR = '../../kubejs/data/minecraft/structures'
#STRUCTURES_DIR_MC = 'A:/minecraft/data/minecraft/structures'
STRUCTURES_DIR = 'C:/Users/Pyritie/source/repos/TerraFirmaGreg-Team/Modpack-Modern/kubejs/data/tfg/structures/trees'

def main():
	#bastion_structures = glob(STRUCTURES_DIR_MC + '/bastion/**/*.nbt', recursive=True)
	structures = glob(STRUCTURES_DIR + '/*.nbt', recursive=True)
	#find_blocks(structures)
	replace_blocks(structures)
	pass


def find_blocks(structures):
	blocks = {}
	for f in structures:
		struct = nbt.load(f)
		for block in struct['palette']:
			blocks[block['Name']] = "Found"
	print(blocks.keys())


def replace_blocks(structures):
	for f in structures:
		struct = nbt.load(f)
		dirty = False

		for block in struct['palette']:
			name = block['Name']
			if name == 'minecraft:chest':
				block['Name'] = StringTag('tfc:wood/chest/mangrove')
				dirty = True
			elif name == 'minecraft:basalt':
				block['Name'] = StringTag('minecraft:deepslate_bricks')
				dirty = True
			elif name == 'minecraft:polished_basalt':
				block['Name'] = StringTag('minecraft:polished_deepslate')
				dirty = True
			elif name == 'minecraft:chain':
				block['Name'] = StringTag('tfc:metal/chain/black_bronze')
				dirty = True
			elif name == 'minecraft:netherrack':
				block['Name'] = StringTag('minecraft:magma_block')
				dirty = True
			elif name == 'minecraft:soul_sand':
				block['Name'] = StringTag('tfc:thatch')
				dirty = True
			elif name == 'minecraft:spawner':
				block['Name'] = StringTag('minecraft:cave_air')
				dirty = True
			elif name == 'minecraft:nether_wart':
				block['Name'] = StringTag('minecraft:brown_mushroom')
				dirty = True

			elif name == 'betterend:mossy_glowshroom_bark':
				block['Name'] = StringTag('ad_astra:glacian_wood')
				dirty = True
			elif name == 'betterend:mossy_glowshroom_cap':
				block['Name'] = StringTag('species:alphacene_moss_block')
				dirty = True
			elif name == 'betterend:mossy_glowshroom_hymenophore':
				block['Name'] = StringTag('ad_astra:glacian_leaves')
				dirty = True
			elif name == 'betterend:mossy_glowshroom_fur':
				block['Name'] = StringTag('betterend:glacian_hymenophore')
				dirty = True
			elif name == 'betterend:mossy_glowshroom_log':
				block['Name'] = StringTag('ad_astra:glacian_log')
				dirty = True

		if dirty:
			print('Modified: ' + f)
			save(struct, f)


def save(struct, f):
	#result_dir = f.replace(STRUCTURES_DIR_MC, STRUCTURES_DIR)
	#result_folder = os.path.dirname(result_dir)
	#os.makedirs(result_folder, exist_ok=True)
	#struct.save(result_dir)
	struct.save(f)

if __name__ == '__main__':
	main()