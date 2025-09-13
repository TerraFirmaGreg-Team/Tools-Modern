const fs = require("fs");

// -------- Core Conversion: From input.json to letter-mapped structure. --------
// Im so sleepy.
function transformJson(inputData) {
  const statePosStr = inputData["statePosArrayList"] || "";

  // Extract blockstatemap section.
  const blockMapMatch = statePosStr.match(/blockstatemap:\[(.*?)\](?:,|})/);
  if (!blockMapMatch) throw new Error("No blockstatemap found");
  const blockstatemapStr = blockMapMatch[1];

  // List of block names.
  const blockNames = [...blockstatemapStr.matchAll(/Name:"(.*?)"/g)].map(m => m[1]);

  // Extract positions.
  const startMatch = statePosStr.match(/startpos:\{(.*?)\}/);
  const endMatch = statePosStr.match(/endpos:\{(.*?)\}/);
  if (!startMatch || !endMatch) throw new Error("No startpos or endpos found");

  // Parse coordinates into objects.
  function parsePos(posStr) {
    const pos = {};
    posStr.split(",").forEach(part => {
      const [key, value] = part.split(":");
      pos[key.trim()] = parseInt(value.trim(), 10);
    });
    return pos;
  }

  const startpos = parsePos(startMatch[1]);
  const endpos = parsePos(endMatch[1]);

  // Calculate dimensions.
  const xDim = endpos["X"] - startpos["X"] + 1;
  const yDim = endpos["Y"] - startpos["Y"] + 1;
  const zDim = endpos["Z"] - startpos["Z"] + 1;
  const totalBlocks = xDim * yDim * zDim;

  // Extract statelist.
  const listMatch = statePosStr.match(/statelist:\[I;(.*?)\]/);
  if (!listMatch) throw new Error("No statelist found");
  const statelistNums = listMatch[1]
    .split(",")
    .map(s => s.trim())
    .filter(s => s !== "")
    .map(Number);

  if (statelistNums.length !== totalBlocks) {
    throw new Error("Statelist count does not match dimensions");
  }

  // Assign letters to blocks.
  // We give air " " for better looking results, but sometimes people like it as "#"
  const blockToLetter = { "minecraft:air": " " };
  let nextLetterOrd = "A".charCodeAt(0);

  function assignLetter(blockType) {
    if (blockType === "minecraft:air") return " ";
    if (!(blockType in blockToLetter)) {
      blockToLetter[blockType] = String.fromCharCode(nextLetterOrd++);
    }
    return blockToLetter[blockType];
  }

  // Build structure. [z][y][x]
  const structure = [];
  let index = 0;
  for (let z = 0; z < zDim; z++) {
    const layer = [];
    for (let y = 0; y < yDim; y++) {
      const row = [];
      for (let x = 0; x < xDim; x++) {
        const blockIndex = statelistNums[index++];
        const blockType = blockNames[blockIndex];
        const letter = assignLetter(blockType);
        row.push(letter);
      }
      layer.push(row);
    }
    structure.push(layer);
  }

  // Build key mapping: letter -> block.
  const keys = {};
  for (const [block, letter] of Object.entries(blockToLetter)) {
    keys[letter] = block;
  }

  return { structure, keys };
}

// -------- Transformations --------

// Rotate aisles along vertical axis (z -> y or something idk).
function rotateAisles(structure) {
  const height = structure.length;       // number of aisles (z).
  const depth = structure[0].length;     // rows per aisle (y).
  const rotated = [];

  for (let z = 0; z < depth; z++) {
    const newAisle = [];
    for (let y = 0; y < height; y++) {
      newAisle.push([...structure[y][z]]);
    }
    rotated.push(newAisle);
  }
  return rotated;
}

// Flip vertically: reverse aisles order (z-axis).
function mirrorVertical(structure) {
  return [...structure].reverse();
}

// Flip horizontally: reverse characters in each row and reverse row order.
function mirrorHorizontal(structure) {
  return structure.map(layer =>
    [...layer].reverse().map(row => [...row].reverse())
  );
}

// Apply both mirrors together.
function mirrorBoth(structure) {
  return mirrorVertical(mirrorHorizontal(structure));
}

// Rotate 90° clockwise. (Different than turning rows to columns. Not used yet).
function rotate90(structure) {
  return structure.map(layer =>
    layer[0].map((_, i) => layer.map(row => row[i]).reverse())
  );
}

// -------- Output --------
function genOutput(data) {
  const outputLines = [];

  // Convert structure into .aisle(...) lines.
  for (const aisle of data.structure) {
    const rowsStr = aisle.map(row => row.join(""));
    const tmp = rowsStr.map(row => `"${row}"`).join(", ");
    outputLines.push(`.aisle(${tmp})`);
  }

  // Append key mappings (.where(...) lines).
  for (const [key, block] of Object.entries(data.keys)) {
    outputLines.push(`   .where("${key}", Predicates.blocks("${block}"))`);
  }

  return outputLines;
}

// -------- Main --------
function main() {
  // Mode comes from command-line argument (default: normal).
  const mode = process.argv[2] || "normal"; 
  const inputJson = JSON.parse(fs.readFileSync("input.json", "utf-8"));
  let data = transformJson(inputJson);

  // Apply transformation based on mode.
  switch (mode.toLowerCase()) {
    case "rotate":
      data.structure = rotateAisles(data.structure);
      break;
    case "mirrorv":
      data.structure = mirrorVertical(data.structure);
      break;
    case "mirrorh":
      data.structure = mirrorHorizontal(data.structure);
      break;
    case "mirrorvh":
        data.structure = mirrorBoth(data.structure);
        break;
    case "rotatemirrorh":
      data.structure = mirrorHorizontal(rotateAisles(data.structure));
      break;
    case "rotatemirrorv":
      data.structure = mirrorVertical(rotateAisles(data.structure));
      break;
    // Yeah... So this is just the same as doing no transformations lol
    // case "rotatemirrors":
    //     data.structure = mirrorVertical(mirrorHorizontal(rotate90(data.structure)));
    //     break;
    default:
      // Normal conversion (no transformation).
      break;
  }

  // Write results to output.js (overwrite cause its annoying to append).
  const lines = genOutput(data);
  fs.writeFileSync("output.js", lines.join("\n"), "utf-8");
  console.log(`Wrote output.js in mode: ${mode}`);
}

if (require.main === module) {
  main();
}