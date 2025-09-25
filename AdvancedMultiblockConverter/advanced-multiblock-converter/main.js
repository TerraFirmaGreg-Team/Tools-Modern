const { app, BrowserWindow, ipcMain } = require("electron");
const path = require("path");

let mainWindow;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1440,
    height: 1090,
    webPreferences: {
      preload: path.join(__dirname, "preload.js"),
      nodeIntegration: true,
      contextIsolation: false,
    },
  });

  mainWindow.loadFile(path.join(__dirname, "dist", "index.html"));

  mainWindow.on("closed", () => (mainWindow = null));
}

const memoryFS = {};
function withMemoryFS(fn) {
  const fs = require("fs");

  const _readFileSync = fs.readFileSync;
  const _writeFileSync = fs.writeFileSync;
  const _existsSync = fs.existsSync;
  const _unlinkSync = fs.unlinkSync;

  try {
    fs.readFileSync = (file, enc) => {
      if (!(file in memoryFS)) throw new Error(`File not found: ${file}`);
      return memoryFS[file];
    };
    fs.writeFileSync = (file, data) => {
      memoryFS[file] = typeof data === "string" ? data : data.toString();
    };
    fs.existsSync = (file) => file in memoryFS;
    fs.unlinkSync = (file) => {
      if (file in memoryFS) delete memoryFS[file];
    };

    return fn();
  } finally {
    fs.readFileSync = _readFileSync;
    fs.writeFileSync = _writeFileSync;
    fs.existsSync = _existsSync;
    fs.unlinkSync = _unlinkSync;
  }
}

ipcMain.handle("run-converter", async (event, { option, input }) => {
  if (option === "exit") return app.quit();

  const commands = {
    reset: "reset",
    rotatez: "rotatez",
    rotatex: "rotatex",
    rotatey: "rotatey",
    mirrorh: "mirrorh",
    mirrorv: "mirrorv",
  };

  const command = commands[option];
  if (!command) return { input, output: "Unknown command" };

  const { runConversion } = require(path.join(__dirname, "MultiConverter.js"));

  let output;
  try {
    output = withMemoryFS(() => {
      const fs = require("fs");

      fs.writeFileSync("input.json", input || "{}", "utf8");

      runConversion(input, command);

      if (fs.existsSync("output.js")) {
        return fs.readFileSync("output.js", "utf8");
      }

      return "Error: output.js was not produced";
    });
  } catch (err) {
    output = `Error running conversion: ${err.message}`;
  }

  return { input, output };
});

app.whenReady().then(createWindow);

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") app.quit();
});

app.on("activate", () => {
  if (BrowserWindow.getAllWindows().length === 0) createWindow();
});
