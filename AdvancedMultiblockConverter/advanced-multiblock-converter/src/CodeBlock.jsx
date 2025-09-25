import React from "react";

export default function CodeBlock({ title, value, onChange, readOnly }) {
  return (
    <div className="codeblock">
      <h2>{title}</h2>
      <textarea
        value={value}
        onChange={(e) => onChange && onChange(e.target.value)}
        readOnly={readOnly}
      />
    </div>
  );
}