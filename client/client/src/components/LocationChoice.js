import React, { useState } from "react";
import "../styles/LocationChoice.css";

const LocationChoice = (props) => {
  const { selectedLocation, onLocationChange } = props;
  const locations = ["Virginia", "UK", "Japan"];

  return (
    <div className="locationChoice">
      <h3>Machine location</h3>
      {locations.map((location) => (
        <div key={location}>
          <label>
            <input
              type="checkbox"
              name="type"
              value={location}
              checked={selectedLocation === location}
              onChange={onLocationChange}
            />
            {location}
          </label>
        </div>
      ))}
    </div>
  );
};

export default LocationChoice;
