import React, { useState } from "react";
import CodeBlock from "./CodeBlock";

export default function App() {
  const [input, setInput] = useState("{\n  \"statePosArrayList\": \"...\"\n}");
  const [output, setOutput] = useState("// Converted output will appear here");

  const run = async (mode) => {
    const result = await window.api.runConverter(mode, input);
    setOutput(result);
  };

  return (
    <div className="container">
      <CodeBlock title="Input JSON" value={input} onChange={setInput} />
      
      <div className="buttons">
        <button onClick={() => run("reset")}>Reset</button>
        <button onClick={() => run("rotatex")}>Rotate X</button>
        <button onClick={() => run("rotatey")}>Rotate Y</button>
        <button onClick={() => run("rotatez")}>Rotate Z</button>
        <button onClick={() => run("mirrorh")}>Mirror H</button>
        <button onClick={() => run("mirrorv")}>Mirror V</button>
      </div>

      <CodeBlock title="Output JS" value={output} readOnly />
    </div>
  );
}