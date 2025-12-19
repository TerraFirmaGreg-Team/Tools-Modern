const { contextBridge, ipcRenderer } = require("electron");

contextBridge.exposeInMainWorld("api", {
  runConverter: (mode, inputJson) => ipcRenderer.invoke("run-converter", mode, inputJson)
});