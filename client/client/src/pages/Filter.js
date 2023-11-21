import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Grid, Alert, AlertTitle } from "@mui/material";
import SupplierChoice from "../components/SupplierChoice";
import TypeChoice from "../components/TypeChoice";
import LocationChoice from "../components/LocationChoice";
import "../styles/Filter.css";
import BannerImage from "../assets/filterBack.png";

const Filter = () => {
  const [selectedLocation, setSelectedLocation] = useState("");
  const [selectedType, setSelectedType] = useState("");
  const [selectedSupplier, setSelectedSuppliers] = useState([]);
  const [formSubmitted, setFormSubmitted] = useState(false);
  const [error, setError] = useState(false);

  const handleFormSubmit = (e) => {
    e.preventDefault();
    setFormSubmitted(true);
    setError(
      !(selectedLocation && selectedType && selectedSupplier.length > 0)
    );
  };

  const handleSupplierChange = (e) => {
    const supplier = e.target.value;
    if (e.target.checked) {
      setSelectedSuppliers([...selectedSupplier, supplier]);
    } else {
      setSelectedSuppliers(selectedSupplier.filter((s) => s !== supplier));
    }
    setError(
      !(selectedLocation && selectedType && selectedSupplier.length > 0)
    );
  };

  const handleLocationChange = (e) => {
    setSelectedLocation(e.target.value);
    setError(!(e.target.value && selectedType && selectedSupplier.length > 0));
  };

  const handleTypeChange = (e) => {
    setSelectedType(e.target.value);
    setError(
      !(selectedLocation && e.target.value && selectedSupplier.length > 0)
    );
  };

  return (
    <div
      className="submitChoice"
      style={{ backgroundImage: `url(${BannerImage})` }}
    >
      <form onSubmit={handleFormSubmit}>
        <h1>Select from the options below:</h1>
        <p>
          Please select the cloud providers you want to check, the location of
          the machine, and the machine type (CPU and Memory properties). Once
          you've made your selections, you can view the graphs and performance
          data for the selected providers.
        </p>
        <Grid container spacing={2} justifyContent="center" alignItems="center">
          <Grid item xs={10} sm={3} className="grid-item">
            <SupplierChoice
              selectedSupplier={selectedSupplier}
              onSupplierChange={handleSupplierChange}
            />
          </Grid>
          <Grid item xs={10} sm={3} className="grid-item">
            <LocationChoice
              selectedLocation={selectedLocation}
              onLocationChange={handleLocationChange}
            />
          </Grid>
          <Grid item xs={10} sm={3} className="grid-item">
            <TypeChoice
              selectedType={selectedType}
              onTypeChange={handleTypeChange}
            />
          </Grid>
        </Grid>
        {formSubmitted &&
          (!selectedLocation ||
            !selectedType ||
            selectedSupplier.length === 0) && (
            <div className="errorContainer">
              <Alert severity="error" sx={{ marginTop: "10px" }}>
                <AlertTitle>Error</AlertTitle>
                Please select from all three options
              </Alert>
            </div>
          )}
        <div className="buttonWrapper">
          {error ? (
            <button type="submit">Submit</button>
          ) : (
            <Link
              to={"/Graphs"}
              state={{
                location: selectedLocation,
                type: selectedType,
                suppliers: selectedSupplier,
              }}
            >
              <button type="submit">Submit</button>
            </Link>
          )}
        </div>
      </form>
    </div>
  );
};

export default Filter;
