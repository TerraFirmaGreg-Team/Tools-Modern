const fs = require("fs");

/* Im so sleepy. Welcome to Redeix's Multiblock Converter. Here's my cat.
                ▓░▒▒▒
                ▓▒▒░░░░                      ░▒▒
                ▓▒░░░░░▒░                  ░▒▒▓▓
                ▒▒░░░▒▒▒▒               ░▒▒▒▒▒▓
                ▓▒▒░▒▒▓▓█▓             ▒▒▒▒▒▒▓▒
                █▒░▓▓▓▓▓▓██▓▒        ▒░▒▓▓▓▓▓▓▓
                █▓░░░▒▓███▓▒░▒▒  ▒▒▒░░▒▓▓▓▓▓▓▓░
                ▓▒░░▓█████▓▓▒▒░░░▒▒▓▓▓▓███▓▓▓▒
              ░▓▓▒▓▒▒███▓▒░░ ░ ▒▒▓▒▒░▒▒▓▓████▓
              ░▒  ░▓█▓▒░░     ░▒▒▒▒ ░  ░▒▓████
                  ░░░          ▒▒▒  ░   ░▒▒▓█▓░
                      ░▒▒▒░░ ░░░░▒░▒▒░  ░░░▒▓▒░
                  ░░░░▒▒██▓░  ░░▒▒▒██▓▒▒░░ ░▒▒
                  ░ ░▒░░▒▒▓▓░  ░░▒▒▓▓██▓▒▒░░▒▒░░
                ░   ░░░░▒░     ░░▒▓▓▓▓▒▒▒▒▒▒▓▒░
                    ░ ░▒▒░░ ░░   ▒▒▒▒░▒▒▒▒▒▒▒▒▒▒
                ░ ░▒▒▒▒▒░░   ░   ▒▓▓▒▒▓▓▓▒▒▒▒▒▒▒░
                  ▒▓▓▓▓░    ░▓▓▒▓█▓▓▓▓▓▓▓▒▒▒▒▒▓▒░░
                  ▒▓▓▓▒░░▒▓▓▓▓▓▓▓▓▓▓▒▒▒▓▓▓▓▒▒▒░
                  ░▒▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▒▒▒▒░
                  ░▒▓▓▓▓▓███▓▒▒▓▓██████▓▒▒▒▒▒░░
                  ░░▒▒▓█████████████████▓▓▒▒░░░░
                  ░░▒▒▓██████████████████▓▓▒▒▒▒░
                ░ ░▒▒▓▓█████████████▓▓▓▓▒▒▒▒▒▒░
                  ░░▒▒▓██████████▓▓▓▓▒▒▒▒▒▒▒▒▒░
                  ░░ ░▒▓█▓██████▓▓▓▓▓▓▓▒▒▒▒▒▒▒░░
                ░░░░░▒▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▒▓▒▒░░░
                ░░  ░▒▒▒▒▒▒▒░▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒░░░▒░
                ░░  ░▒▒▒▒░░▒░▒▒░░▒▒▒▓▒▓▒▒▒░░░▒▒▓▒░
            ░░░░░░░░▒▒▓▒░░▒ ░░    ░░░░░░░░▒▒░▒▓▓▒
            ░░░ ░░░▒▒▒▒▒▒▓▒░    ░░░░░░░░░▒▒▒▒▒▓▒▒
            ░░▒░░▒▒▒▒▒▒▒▒▒▒░ ░ ░░░░▒▒▒▒▓▒▒▓▒▒▓▓▒▒
            ░░▒▒░░▒▒░▒▒▓▓▓▓▒▒░░░▒▒▓█▓█▓▓▓▓▓▓▓▓█▓▓▒
          ░░░▒▒░░░▒▒▒▓████▓▒░▒▓▓▓▓▓▓▓▓▓▓▒▓▓▓▓█▓▓▓▒
          ░░▒▒▓▒░░░▒▓▓██████▓▒▓▓▓█▓▓▒▒▒▒▒▒▓▓▓▓▓▓▓▒
          ░░░▒▒▓▓▒░▒▒▓▓▓███████▓███▓▓▒▒▒▒▒▓▓▓████▓▓░
          ░░▒▒▒▓▓▓▒░▒▒▓▓▓████████▓▓█▓▓▒░▒▒▓▓▓███▓▓▓▒
          ░░░░░▒▒▒░▒▓▓▓▓▓▓███████▒█▓▓▒▒▒▒▒▓████▓▓▓░  
      ░░░░░░░▒▒▒░▓▒▒▒▓▓▓▓▓▓██████▓▓▒▒▒▒▒▒▒▓██▓▓▓▓▓░
    ░▒▒▒▒▒▒▒▒▒▓▒▓▓▓▓▒▒░▒▒▒▓▓██▓█▓▒▒░░░░▒▒▓█▓█▓▓▓▓▒░ ░
    ▒▒▒▒▒▒▓▒▒▓▒▒▒▒▓▓▓▓▒░░▒▓████▓▓▓▒▒░░░░▒▓██▓▓▓▓▓▓▒░ ░
  ░▓▓▓▒░░▒▒▒▒▓▓▓▓▒▒▒▓▓▓▒░▒▓▓████▓▓▒░░░░▒▓███▓▓▓▓▓░░░
  ░▒▓▓█▓▓▓▒▒▒▒░░░░▒▓▒▓▓▒▒▒▒▓▓███▓▒▒▒░░▒▓████▓▓▓▒▒░░░
    ░████▓█▓▓▓▒▒▒░▒▓▓▓▓▒▓▓▒▓▓▓██▓▒▒▒▒▓▓▓▓█▓▓▓▓▒▒░░░
    ░▓▓▓▒░▒▒▓▓▒▒▒▒▒▒▒▓▒▒▒▒▒▒▒▒▓█▓▒▒▒▒▓▓▓███▓▓▒▒░░
    ░▒▒░░░░▒▓▓▒▒░ ░       ░░░░▒█▓▓▓▓██▓████▓▒▒▒
    ░░░░░▒▒▒▒▒▒▒▒▒░▒▒░░▒▒▒▓▓▓▒▓▓▓▓██████▓▓▒▒
      ░░░░░░▒▓▓▓▓▓▓▓▓▓▓▓▓▓████▓▓▓▓▓███▓▓▒▒
      ░░░▒▒▒▒▒▒▓▒░▓▓▓▓▓██▓▓▓▓▓▓▓▓▓██▓▓▒░
        ░░░▒▒▒▒▒▒▓▓▒▓▓▓█▓▓▓▓▓▒░   ░░                                        
              ░░░░░░░▒▒▒░░░░░ ░
                ░
*/

//#region Json Transform
/**
 * Transforms JSON output from Building Gadgets' copy-paste gadget into GTCEU Multiblock format.
 *
 * @param {Object} inputData - The input data object.
 * @param {string} inputData.statePosArrayList - The encoded string with blockstatemap, statelist, startpos, and endpos.
 * @returns {{ structure: string[][][], keys: Object<string, string> }} An object containing:
 *   - structure: A 3D array [z][y][x] of block letters representing the structure.
 *   - keys: A mapping from block letter to block name kinda like shaped recipes.
 * @throws {Error} If required sections (blockstatemap, statelist, startpos, endpos) are missing or malformed,
 *   or if the statelist length does not match the calculated structure dimensions.
 */
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

//#region Transformations

/**
 * Rotates the array 90 degrees around the Z-axis.
 *
 * @param {Array<Array<Array<any>>>} structure - The 3D array  to rotate.
 * @returns {Array<Array<Array<any>>>} The rotated 3D array.
 */
function rotateAroundZ(structure) {
  return structure.map(layer =>
    layer[0].map((_, i) => layer.map(row => row[i]).reverse())
  );
}

/**
 * Rotates the array 90 degrees around the X-axis.
 *
 * @param {Array<Array<Array<any>>>} structure - The 3D array to rotate, structured as [z][y][x].
 * @returns {Array<Array<Array<any>>>} The rotated 3D array.
 */
function rotateAroundX(structure) {
  const zDim = structure.length;
  const yDim = structure[0].length;
  const xDim = structure[0][0].length;

  const rotated = [];
  for (let y = 0; y < yDim; y++) {
    const newLayer = [];
    for (let z = zDim - 1; z >= 0; z--) {
      newLayer.push([...structure[z][y]]);
    }
    rotated.push(newLayer);
  }
  return rotated;
}

/**
 * Rotates the array 90 degrees around the Y-axis.
 *
 * @param {Array<Array<Array<any>>>} structure - The 3D array to rotate, structured as [z][y][x].
 * @returns {Array<Array<Array<any>>>} The rotated 3D array.
 */
function rotateAroundY(structure) {
  const zDim = structure.length;
  const yDim = structure[0].length;
  const xDim = structure[0][0].length;

  const rotated = [];
  for (let z = 0; z < xDim; z++) {
    const newLayer = [];
    for (let y = 0; y < yDim; y++) {
      const row = [];
      for (let x = zDim - 1; x >= 0; x--) {
        row.push(structure[x][y][z]);
      }
      newLayer.push(row);
    }
    rotated.push(newLayer);
  }
  return rotated;
}

/**

 * Mirrors vertically by reversing the order of the rows in the structure array.
 *
 * @param {Array} structure - The 2D array to mirror vertically.
 * @returns {Array} A new 2D array with the rows in reversed order.
 */
function mirrorVertical(structure) {
  return [...structure].reverse();
}

/**
 * Mirrors horizontally by reversing the order of layers and the order of rows within each layer.
 *
 * @param {Array<Array<Array<any>>>} structure - The 3D array to mirror.
 * @returns {Array<Array<Array<any>>>} The horizontally mirrored 3D array.
 */
function mirrorHorizontal(structure) {
  return structure.map(layer =>
    [...layer].reverse().map(row => [...row].reverse())
  );
}
//#endregion

//#region Parsing output.js
/**
 * Parses a JavaScript file to extract structure and mappings.
 *
 * @param {string} filename - The path to the JavaScript file to parse.
 * @returns {null | {
 *   structure: string[][][],
 *   baseLines: string[],
 *   insertionIndex: number,
 *   whereMap: Record<string, string>,
 *   whereOrder: string[]
 * }} An object containing the parsed structure, base lines with `.aisle()` and `.where()` lines removed.
 *     The index at which aisles were first inserted, a mapping of keys to block names, and the order of keys.
 *     Returns `null` if the file does not exist or no aisles are found.
 */
function parseOutputJs(filename) {
  if (!fs.existsSync(filename)) return null;
  const origLines = fs.readFileSync(filename, "utf-8").split("\n");

  const aisles = [];
  const whereMap = {};
  const whereOrder = [];
  let firstAisleIndex = -1;

  // First pass collect mappings.
  origLines.forEach((line, idx) => {
    const trimmed = line.trim();
    if (trimmed.startsWith(".aisle")) {
      if (firstAisleIndex === -1) firstAisleIndex = idx;
      const match = trimmed.match(/\.aisle\((.*?)\)(?:\.setRepeatable\((\d+)\))?/);
      if (match) {
        const rows = match[1]
          .split(",")
          .map(s => s.trim().replace(/^"|"$/g, ""));
        const repeat = match[2] ? parseInt(match[2], 10) : 1;
        for (let r = 0; r < repeat; r++) {
          aisles.push(rows.map(row => row.split("")));
        }
      }
      return;
    }

    if (trimmed.startsWith(".where")) {
      // Find key in quotes.
      const firstQuote = trimmed.indexOf('"');
      const secondQuote = trimmed.indexOf('"', firstQuote + 1);
      if (firstQuote === -1 || secondQuote === -1) return;
      const key = trimmed.substring(firstQuote + 1, secondQuote);

      const afterComma = trimmed.substring(trimmed.indexOf(",", secondQuote) + 1).trim();
      let blockName = null;
      let m = afterComma.match(/Predicates\.blocks\(\s*["'](.+?)["']\s*\)/);
      if (m) {
        blockName = m[1];
      } else {
        m = afterComma.match(/["'](.+?)["']/);
        if (m) blockName = m[1];
      }

      if (blockName != null && !(key in whereMap)) {
        whereMap[key] = blockName;
        whereOrder.push(key);
      }
      return;
    }
  });

  if (aisles.length === 0) return null;

  // Build a "base" set of lines that retain everything except .aisle/.where lines.
  const baseLines = [];
  let nonRemovedCountBeforeFirstAisle = 0;
  for (let i = 0; i < origLines.length; i++) {
    const trimmed = origLines[i].trim();
    if (trimmed.startsWith(".aisle") || trimmed.startsWith(".where")) {
      // Skip both .aisle and .where lines.
      continue;
    }
    baseLines.push(origLines[i]);
    if (firstAisleIndex !== -1 && i < firstAisleIndex) nonRemovedCountBeforeFirstAisle++;
  }
  const insertionIndex = nonRemovedCountBeforeFirstAisle;

  return {
    structure: aisles,
    baseLines,
    insertionIndex,
    whereMap,
    whereOrder
  };
}
//#endregion

//#region Output Helpers
/**
 * Generates a list of `.aisle()` lines with optional .setRepeatable(n)
 *
 * The function processes a structure represented as a 3D array of characters. By default:
 * - Each element is a "layer".
 * - Each layer is converted into .aisle("row1", "row2", ...).
 * - Consecutive identical layers are collapsed into a single `.aisle()` statement with `.setRepeatable(n)` appended.
 *
 * @param {string[][][]} structure - A 3D array representing the structure.
 * @returns {string[]} A list of formatted `.aisle()` lines
 */
function genAisleLines(structure) {
  const lines = [];
  let prev = null;
  let count = 0;

  function layerToStrings(layer) {
    return layer.map(row => row.join(""));
  }

  for (let i = 0; i < structure.length; i++) {
    const layer = structure[i];
    const layerStrRepr = JSON.stringify(layer);
    if (prev !== null && layerStrRepr === prev.layerStr) {
      count++;
    } else {
      if (prev !== null) {
        const rows = prev.layer.map(r => `"${r.join("")}"`).join(", ");
        let line = `.aisle(${rows})`;
        if (prev.count > 1) line += `.setRepeatable(${prev.count})`;
        lines.push(line);
      }
      prev = { layer: layerToStrings(layer).map(s => s.split("")), layerStr: layerStrRepr, count: 1 };
      count = 1;
      prev.count = 1;
    }
    if (prev && i > 0 && JSON.stringify(structure[i]) === prev.layerStr) {
      } else if (prev && i < structure.length - 1) {
    }
  }

  lines.length = 0;
  let i = 0;
  while (i < structure.length) {
    const layer = structure[i];
    // count how many identical consecutive layers.
    let run = 1;
    while (i + run < structure.length && JSON.stringify(structure[i + run]) === JSON.stringify(layer)) {
      run++;
    }
    const rows = layer.map(r => `"${r.join("")}"`).join(", ");
    let line = `.aisle(${rows})`;
    if (run > 1) line += `.setRepeatable(${run})`;
    lines.push(line);
    i += run;
  }

  return lines;
}
//#endregion

//#region Gen Where Lines
/**
 * Generates a list of `.where()` lines for a structure definition.
 *
 * @param {Object.<string, string>} whereMap - Mapping of character keys to block IDs.
 * @param {string[]} [whereOrder] - Optional array defining the desired order of keys.
 * @returns {string[]} List of `.where()` lines.
 */
function genWhereLines(whereMap, whereOrder) {
  const lines = [];
  const keys = whereOrder && whereOrder.length ? whereOrder : Object.keys(whereMap);
  for (const k of keys) {
    const block = whereMap[k];
    if (!block) continue;
    lines.push(`.where("${k}", Predicates.blocks("${block}"))`);
  }
  return lines;
}
//#endregion

//#region Main
/** Main function calls all inputs and generate output using transformations. */
/**
 * Main entry point for the AdvancedMultiblockConverter script.
 * 
 * This function processes command-line arguments to determine the mode of operation:
 * - "reset" or "default": Rebuilds the output from input.json, applying no transformations.
 * - "rotatez", "rotatex", "rotatey": Applies rotation transformations to the structure.
 * - "mirrorh", "mirrorv": Applies mirroring transformations to the structure.
 * - "reset/default": No transformation is applied.
 * 
 * The function reads and writes files as needed, applies transformations, and generates output.js.
 * It also merges mapping keys from input.json with existing mappings, preserving order.
 * 
 * @returns {void}
 */
function main() {
  const mode = process.argv[2] || "normal";

  // Reset: Force rebuild from input.json
  if (mode.toLowerCase() === "reset" || mode.toLowerCase() === "default") {
    const inputJson = JSON.parse(fs.readFileSync("input.json", "utf-8"));
    const data = transformJson(inputJson);
    const aisleLines = genAisleLines(data.structure);
    const whereLines = genWhereLines(data.keys, Object.keys(data.keys));
    const out = [...aisleLines, ...whereLines];
    fs.writeFileSync("output.js", out.join("\n"), "utf-8");
    console.log("Reset complete: fresh conversion from input.json.");
    return;
  }

  // Try parse existing output.js
  const parsed = parseOutputJs("output.js");

  if (!parsed) {
    // Initial run converts input.json without transformations.
    const inputJson = JSON.parse(fs.readFileSync("input.json", "utf-8"));
    const data = transformJson(inputJson);
    const aisleLines = genAisleLines(data.structure);
    const whereLines = genWhereLines(data.keys, Object.keys(data.keys));
    const out = [...aisleLines, ...whereLines];
    fs.writeFileSync("output.js", out.join("\n"), "utf-8");
    console.log("Initial conversion complete (no transformations applied).");
    return;
  }

  // Apply transformations over parsed.structure
  switch (mode.toLowerCase()) {
    case "reset/default":
      break;
    case "rotatez":
      parsed.structure = rotateAroundZ(parsed.structure);
      break;
    case "rotatex":
      parsed.structure = rotateAroundX(parsed.structure);
      break;
    case "rotatey":
      parsed.structure = rotateAroundY(parsed.structure);
      break;
    case "mirrorh":
      parsed.structure = mirrorHorizontal(parsed.structure);
      break;
    case "mirrorv":
      parsed.structure = mirrorVertical(parsed.structure);
      break;
    default:
      console.log(`No transformation for mode: ${mode}`);
      break;
  }

  // Generate new aisle lines (collapsed) and where lines (appended).
  const newAisleLines = genAisleLines(parsed.structure);

  /*
  * Merge existing whereMap + keys from input.json.
  * If you started from input.json originally, parsed.whereMap may not include every mapping.
  * Try to merge keys from input.json but do not overwrite existing mapping order.
  */
  let finalWhereMap = Object.assign({}, parsed.whereMap || {});
  let finalWhereOrder = Array.isArray(parsed.whereOrder) ? parsed.whereOrder.slice() : [];

  /*
  * Build new file lines by taking baseLines (original file with .aisle/.where removed),
  * inserting new aisle block at insertionIndex, then appending the unique .where() lines at the end.
  */
  const newFileLines = parsed.baseLines.slice();
  const insertAt = parsed.insertionIndex >= 0 ? parsed.insertionIndex : newFileLines.length;
  newFileLines.splice(insertAt, 0, ...newAisleLines);

  // Append .where lines at the end.
  const whereLines = genWhereLines(finalWhereMap, finalWhereOrder);
  newFileLines.push(...whereLines);

  fs.writeFileSync("output.js", newFileLines.join("\n"), "utf-8");
  console.log(`Applied transformation: ${mode}`);
}

main();
//endregion