# Multiblock Converter

>The advanced converter is a heavily modified tool originally created by Phoenixvine.
>This tool takes structure information from the Building Gadget's mod and converts it into GT structure format. All files should remain in the same folder.

## Instructions

* Install [Node.js]( https://nodejs.org/en/download) if you don't have it.

### Option 1\) Running as Electron App

* Run `Advanced Multiblock Converter.exe`
* Paste json inside the input block.
* Select the start button.
* Apply transformations if needed.
* Copy JS output from the output block.

### Option 2\) Running as Batch

* Place your structure text into input.json and save the file.
* Use `Run.bat`
* Select reset to generate the initial structure.
* Select your desired transformations.
* Selecting reset again will restart the transformations.
* Result will go to output.js

### DEV Only\) Running the Packager

* Set the directory:

    `
cd <advanced-multiblock-converter location>
`
* Verify node installation:

    `
node -v
`
`
npm -v
`
* Install dependencies:

    `
npm install
`
* Build the files:

    `
npm run build
`
`
npm run dist
`
* Start the app:

    `
npm start
`
