import React, { useState, useEffect } from "react";
import Prism from "prismjs";
import "prismjs/components/prism-json.js";
import "prismjs/components/prism-javascript.js";
import "prismjs/themes/prism-tomorrow.css";

const { ipcRenderer } = window.require("electron");

export default function App() {
    const [inputCode, setInputCode] = useState("{\n}");
    const [outputCode, setOutputCode] = useState("");

    useEffect(() => {
        const outputEl = document.querySelector(".code-container .language-javascript");
        if (outputEl) Prism.highlightElement(outputEl);
    }, [outputCode]);

    const handleClick = async (option) => {
        if (option === "exit") {
            ipcRenderer.invoke("run-converter", { option });
            return;
        }

        try {
            const parsedInput = JSON.parse(inputCode);
            const result = await ipcRenderer.invoke("run-converter", {
                option,
                input: JSON.stringify(parsedInput),
            });

            setOutputCode(result.output || "");
            setInputCode(JSON.stringify(parsedInput, null, 2));
        } catch (err) {
            setOutputCode("Error: Invalid JSON\n" + err.message);
        }
    };

    const baseButtonStyle = {
    border: "1px solid #444",
    borderRadius: "4px",
    padding: "8px 16px",
    cursor: "pointer",
    background: "#333",
    color: "#fff",
    transition: "background 0.2s, border-color 0.2s",
    };

    const hoverStyle = {
    background: "#555",
    };

    const buttons = [
    { label: "Start/Reset", option: "reset" },
    { label: "Rotate Z", option: "rotatez", borderColor: "#1976d2" },
    { label: "Rotate X", option: "rotatex", borderColor: "#bb1717ff" },
    { label: "Rotate Y", option: "rotatey", borderColor: "#0e9c39ff" },
    { label: "Mirror Horizontal", option: "mirrorh" },
    { label: "Mirror Vertical", option: "mirrorv" },
    { label: "Exit", option: "exit", background: "#d32f2f", borderColor: "#b71c1c" },
    ];

    return (
        <div style={{ padding: 20, fontFamily: "sans-serif" }}>
            <h1>Multiblock Converter</h1>
            <div
            style={{
                marginBottom: 20,
                display: "flex",
                flexWrap: "wrap",
                gap: 10,
                justifyContent: "center",
                alignItems: "center",
            }}
            >
            {buttons.map((b) => (
                <button
                key={b.option}
                onClick={() => handleClick(b.option)}
                style={{
                    ...baseButtonStyle,
                    border: b.borderColor ? `2px solid ${b.borderColor}` : baseButtonStyle.border,
                    background: b.background || baseButtonStyle.background,
                }}
                onMouseEnter={(e) => (e.currentTarget.style.background = "#555")}
                onMouseLeave={(e) =>
                    (e.currentTarget.style.background = b.background || baseButtonStyle.background)
                }
                >
                {b.label}
                </button>
            ))}
            </div>

            <h2>Input JSON</h2>
            <textarea
                value={inputCode}
                onChange={(e) => setInputCode(e.target.value)}
                style={{
                    width: "100%",
                    height: "450px",
                    fontFamily: "'Fira Code', monospace",
                    background: "#1e1e1e",
                    color: "#ddd",
                    border: "1px solid #444",
                    borderRadius: "6px",
                    padding: "8px",
                    resize: "vertical",
                }}
            />

            <h2>Output JS</h2>
            <pre
                className="code-container"
                style={{
                    width: "100%",
                    height: "450px",
                    fontFamily: "'Fira Code', monospace",
                    background: "#1e1e1e",
                    color: "#ddd",
                    border: "1px solid #444",
                    borderRadius: "6px",
                    padding: "8px",
                    overflow: "auto",
                    boxSizing: "border-box",
                }}
            >
                <code className="language-javascript">{outputCode}</code>
            </pre>
        </div>
    );
}