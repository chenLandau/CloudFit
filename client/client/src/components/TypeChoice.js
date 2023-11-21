import React, { useState } from "react";
import "../styles/TypeChoice.css";

const TypeChoice = (props) => {
  const { selectedType, onTypeChange } = props;
  const types = ["2cpu-4GB", "4cpu-16GB", "8cpu-32GB"];

  return (
    <div className="typeChoice">
      <h3>Machine type</h3>
      {types.map((type) => (
        <div key={type}>
          <label>
            <input
              type="checkbox"
              name="type"
              value={type}
              checked={selectedType === type}
              onChange={onTypeChange}
            />
            {type}
          </label>
        </div>
      ))}
    </div>
  );
};

export default TypeChoice;
